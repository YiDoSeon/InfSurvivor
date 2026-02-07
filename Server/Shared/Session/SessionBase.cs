using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MessagePack;
using Shared.Packet;

namespace Shared.Session
{
    public abstract class PacketSession : SessionBase
    {
        public static readonly int HeaderSize = 2;

        public void Send<T>(T packet) where T : IPacket
        {
            byte[] body = MessagePackSerializer.Serialize(packet);

            ushort size = (ushort)body.Length;
            byte[] sendBuffer = new byte[size + 4]; // 최대 65,535

            Array.Copy(
                BitConverter.GetBytes((ushort)(size + 4)), 0, // 보낼 버퍼 크기를 byte[] 변환 후 0번 부터
                sendBuffer, 0, sizeof(ushort)); // sendBuffer 0번부터 1번까지 복사
            Array.Copy(
                BitConverter.GetBytes((ushort)packet.PacketId), 0, // 보낼 PacketId를 byte[] 변환 후 0번부터
                sendBuffer, 2, sizeof(ushort)); // sendBuffer 2번부터 3번까지 복사
            Array.Copy(body, 0, sendBuffer, 4, size); // 나머지 복사
            uint hex = BitConverter.ToUInt16(sendBuffer, 2);

#if DEBUG_INTERNAL
            Console.WriteLine($"[송신1]: {packet.PacketId} {hex}");
#endif

            SendBufferImpl(new ArraySegment<byte>(sendBuffer));
        }

        public virtual void SendBufferImpl(ArraySegment<byte> sendBuffer)
        {
            SendBuffer(sendBuffer);
        }

        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;

            while (true)
            {
                if (buffer.Count < HeaderSize)
                {
                    break;
                }

                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);

                if (buffer.Count < dataSize)
                {
                    break;
                }

                // 버퍼에서 완성된 패킷만 처리
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

                processLen += dataSize;
                // 사용한 버퍼만큼 이동 및 줄이기
                buffer = new ArraySegment<byte>(
                    buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class SessionBase
    {
        #region 소켓 관련 필드
        private Socket socket;
        protected Socket Socket => socket;
        private int disconnected = 0;
        #endregion

        #region 수신 관련 필드
        /// <summary>
        /// 64KB 버퍼 <br/>
        /// /// 만약에 64KB 버퍼를 넘어가면 넘어간 패킷들은 <br/>
        /// 커널 수신 버퍼에서 대기하다가 <br/>
        /// Socket.ReceiveAsync를 통해 recvBuffer에 넣어준다.
        /// </summary>
        /// <returns></returns>
        private RecvBuffer recvBuffer = new RecvBuffer(65535);
        private SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();            
        #endregion

        #region 송신 관련 필드
        private Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>();
        private List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();
        private SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        private object _lock = new object();
        #endregion

        #region 추상 메서드
        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        #endregion

        #region 메서드        
        public void Start(Socket socket)
        {
            this.socket = socket;
            recvArgs.Completed += OnRecvCompleted;
            sendArgs.Completed += OnSendCompleted;

            RegisterRecv();
        }

        protected void Disconnect()
        {
            // 동시에 Disconnect가 발생하더라도 한번만 처리
            if (Interlocked.Exchange(ref disconnected, 1) == 1)
            {
                return;
            }

            if (socket == null)
            {
                return;
            }

            OnDisconnected(socket.RemoteEndPoint);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            Clean();
        }

        private void Clean()
        {
            lock (_lock)
            {
                // 송신 처리 중인게 있었으면 마저 처리하고 초기화
                sendQueue.Clear();
                pendingList.Clear();
            }
        }
        #endregion

        #region 송신 메서드

        public void SendBuffer(List<ArraySegment<byte>> sendBuffList)
        {
            if (sendBuffList.Count == 0)
            {
                return;
            }

            lock (_lock)
            {
                foreach (ArraySegment<byte> sendBuff in sendBuffList)
                {
                    sendQueue.Enqueue(sendBuff);
                }

#if DEBUG_INTERNAL
                Console.WriteLine($"[Semi Send Check] Total Packets: {sendQueue.Count}");
                foreach (ArraySegment<byte> buff in sendQueue)
                {
                    if (buff.Count >= 4) // 최소 헤더 크기(Size 2 + ID 2) 확인
                    {
                        // 헤더에서 ID 추출 (Size가 앞의 2바이트, ID가 뒤의 2바이트라고 가정)
                        ushort size = BitConverter.ToUInt16(buff.Array, buff.Offset);
                        ushort id = BitConverter.ToUInt16(buff.Array, buff.Offset + 2);

                        Console.WriteLine($" -> Packet ID: {id}, Size: {size}, Offset: {buff.Offset}");
                    }
                    else
                    {
                        Console.WriteLine($" -> Invalid Buffer Size: {buff.Count}");
                    }
                }
                Console.WriteLine("------------------------------");
#endif

                if (pendingList.Count == 0)
                {
                    RegisterSend();
                }
            }
        }

        public void SendBuffer(ArraySegment<byte> sendBuff)
        {
            // 컨텐츠에서 동시 다발적으로 Send를 시도할 수 있음
            // SendAsync에 의해 OnSendCompleted도 동시에 실행될 수 있어서 같이 lock처리함
            // Recv는 Session안에서 등록되고 대기 후 재귀 호출 방식이므로 lock 처리가 필요없음.
            lock (_lock)
            {
                // 빠르게 sendBuff를 큐에 넣는다
                sendQueue.Enqueue(sendBuff);

                // 대기중인 sendBuffer가 없을 때 다시 보내기 시작
                if (pendingList.Count == 0)
                {
                    RegisterSend();
                }
            }
        }
        
        private void RegisterSend()
        {
            if (disconnected == 1)
            {
                return;
            }

            while (sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = sendQueue.Dequeue();
                pendingList.Add(buff);
            }

            sendArgs.BufferList = pendingList;

#if DEBUG_INTERNAL
            Console.WriteLine($"[Final Send Check] Total Packets: {pendingList.Count}");
            foreach (ArraySegment<byte> buff in pendingList)
            {
                if (buff.Count >= 4) // 최소 헤더 크기(Size 2 + ID 2) 확인
                {
                    // 헤더에서 ID 추출 (Size가 앞의 2바이트, ID가 뒤의 2바이트라고 가정)
                    ushort size = BitConverter.ToUInt16(buff.Array, buff.Offset);
                    ushort id = BitConverter.ToUInt16(buff.Array, buff.Offset + 2);

                    Console.WriteLine($" -> Packet ID: {id}, Size: {size}, Offset: {buff.Offset}");
                }
                else
                {
                    Console.WriteLine($" -> Invalid Buffer Size: {buff.Count}");
                }
            }
            Console.WriteLine("------------------------------");
#endif

            try
            {
                bool pending = socket.SendAsync(sendArgs);
                if (pending == false)
                {
                    OnSendCompleted(null, sendArgs);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"{nameof(RegisterSend)} Failed {e}");
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        // 송신이 완료되었기 때문에 초기화 해준다.
                        sendArgs.BufferList = null;
                        pendingList.Clear();

                        OnSend(sendArgs.BytesTransferred);

                        if (sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine($"{nameof(OnSendCompleted)} Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        #endregion

        #region 수신 메서드
        private void RegisterRecv()
        {
            if (disconnected == 1)
            {
                return;
            }

            recvBuffer.Clear();
            ArraySegment<byte> segment = recvBuffer.WriteSegment;
            // 커널에 쌓인 버퍼를 RecvBuffer의 buffer에 offset부터 count까지 받겠다고 설정
            recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {

                bool pending = socket.ReceiveAsync(recvArgs);
                if (pending == false)
                {
                    OnRecvCompleted(null, recvArgs);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"{nameof(RegisterRecv)} Failed {e}");
            }
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    if (recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        // 방어 코드
                        Disconnect();
                        return;
                    }

                    // 컨텐츠 쪽에 넘긴 후 처리된 크기 반환
                    int processLen = OnRecv(recvBuffer.ReadSegment);
                    if (processLen < 0 || processLen > recvBuffer.DataSize)
                    {
                        Disconnect();
                        return;
                    }

                    if (recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // 다음 수신 등록
                    RegisterRecv();
                }
                catch (System.Exception e)
                {
                    Console.WriteLine($"{nameof(OnRecvCompleted)} Failed {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }

        #endregion
    }
}