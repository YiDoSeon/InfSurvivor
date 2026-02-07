using System;

namespace Shared
{
    public class RecvBuffer
    {
        #region 버퍼
        private ArraySegment<byte> buffer;
        private int readPos;
        private int writePos;
        #endregion

        #region 프로퍼티
        /// <summary>
        /// 처리가 필요한 데이터 크기
        /// </summary>
        public int DataSize => writePos - readPos;
        
        /// <summary>
        /// 현재 받을 수 있는 버퍼 크기
        /// </summary>
        public int FreeSize => buffer.Count - writePos;
        
        /// <summary>
        /// 읽을 수 있는 데이터 영역
        /// </summary>
        /// <typeparam name="byte"></typeparam>
        /// <returns></returns>
        public ArraySegment<byte> ReadSegment =>
            new ArraySegment<byte>(buffer.Array, buffer.Offset + readPos, DataSize);

        /// <summary>
        /// 쓸 수 있는 버퍼 영역
        /// </summary>
        /// <typeparam name="byte"></typeparam>
        /// <returns></returns>
        public ArraySegment<byte> WriteSegment =>
            new ArraySegment<byte>(buffer.Array, buffer.Offset + writePos, FreeSize);
        #endregion

        #region 생성자

        public RecvBuffer(int bufferSize)
        {
            buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }
        #endregion
        
        #region 메서드
        

        public void Clear()
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                // 처리가 필요한 데이터가 없으면 커서 위치만 초기화
                readPos = writePos = 0;
            }
            else
            {
                Array.Copy(
                    sourceArray: buffer.Array, // bufferArray의
                    sourceIndex: buffer.Offset + readPos, // buffer.Offset + readPos 부터
                    destinationArray: buffer.Array, // bufferArray의
                    destinationIndex: buffer.Offset, // buffer.Offset으로 복사
                    length: dataSize); // 현재 읽을 수 있는 크기만큼을
                readPos = 0;
                writePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            // 읽으려는 바이트 수가 더 큰 경우 (있어서는 안될 상황)
            if (numOfBytes > DataSize)
            {
                return false;
            }

            readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            // 쓸려는 바이트 수가 더 큰 경우 (있어서는 안될 상황)
            if (numOfBytes > FreeSize)
            {
                return false;
            }

            writePos += numOfBytes;
            return true;
        }
        #endregion
    }
}