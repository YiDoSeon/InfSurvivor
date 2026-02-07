using System;
using System.IO;
using Shared.Packet;

namespace PacketManagerGenerator;


class Program
{
    private static string clientRegister;
    private static string serverRegister;

    private const string SERVER_PATH = "Server/Server/Packet";
    private const string DUMMY_CLIENT_PATH = "Server/DummyClient/Packet";
    private const string CLIENT_PATH = "Client/Assets/InfSurvivor/Scripts/Runtime/Network/Packet";

    private static void Main(string[] args)
    {
        string cwd = Directory.GetCurrentDirectory();
        cwd = Path.GetFullPath(Path.Combine(cwd, "..", "..", "..", "..", "..", ".."));
        if (args.Length > 0)
        {
            cwd = args[0];
        }

        string[] enumNames = Enum.GetNames(typeof(PacketId));

        foreach (string enumName in enumNames)
        {
            // ex) S_SIMPLE_MSG => S, SIMPLE, MSG
            // ex) C_SIMPLE_MSG => C, SIMPLE, MSG
            string[] words = enumName.Split("_");

            string msgName = "";
            foreach (string word in words)
            {
                // SSimpleMsg
                // CSimpleMsg
                msgName += FirstCharToUpper(word);
            }

            string packetName = string.Empty;
            if (enumName.StartsWith("S_"))
            {
                // S_SimpleMsg
                packetName = $"S_{msgName.Substring(1)}";
                // S_SIMPLE_MSG, S_SimpleMsg
                AddRegister(ref clientRegister, enumName, packetName);
            }
            else if (enumName.StartsWith("C_"))
            {
                // C_SimpleMsg
                packetName = $"C_{msgName.Substring(1)}";
                // C_SIMPLE_MSG, C_SimpleMsg
                AddRegister(ref serverRegister, enumName, packetName);
            }
        }

        string serverManagerText = PacketFormat.managerFormat.Replace("{REGISTER_BODY}", serverRegister);
        string serverPath = $"{cwd}/{SERVER_PATH}";
        if (Directory.Exists(serverPath) == false)
        {
            Directory.CreateDirectory(serverPath);
        }
        File.WriteAllText($"{serverPath}/ServerPacketManager.cs", serverManagerText);
        string clientManagerText = PacketFormat.managerFormat.Replace("{REGISTER_BODY}", clientRegister);
        string unityClientPath = $"{cwd}/{CLIENT_PATH}";
        if (Directory.Exists(unityClientPath) == false)
        {
            Directory.CreateDirectory(unityClientPath);
        }
        File.WriteAllText($"{unityClientPath}/ClientPacketManager.cs", clientManagerText);
        //File.WriteAllText($"{cwd}/{DUMMY_CLIENT_PATH}/ClientPacketManager.cs", clientManagerText);
        Console.WriteLine("완료!");
    }

    private static void AddRegister(ref string register, string enumName, string packetName)
    {
        register += string.Format(
            PacketFormat.managerRegisterFormat,
            enumName, // C_SIMPLE_MSG
            packetName); // C_SimpleMsg
    }

    private static string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";
        return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
    }
}