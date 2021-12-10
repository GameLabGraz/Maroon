
using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Security;
using UnityEngine;

public class Server : MonoBehaviour
{
    public  String serverIp;
    public  int portNr;

    private void Start()
    {
        SetUpServer();
    }

    private  void SetUpServer()
    {
        Console.Title = "Socket Server";
        Console.WriteLine("Listening for messages...");

        Socket serverSock = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);

        IPAddress ip = IPAddress.Parse(serverIp);
        IPAddress serverIP = IPAddress.Any;
        IPEndPoint serverEP = new IPEndPoint(ip, portNr);
      //  SocketPermission perm = new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "98.112.235.18", 33367);
        serverSock.Bind(serverEP);
        serverSock.Listen(10);

        while (true)
        {
            Socket connection = serverSock.Accept();

            Byte[] serverBuffer = new Byte[8];
            String message = String.Empty;

            while (connection.Available > 0)
            {
                int bytes = connection.Receive(
                    serverBuffer,
                    serverBuffer.Length,
                    0);

                message += Encoding.UTF8.GetString(
                    serverBuffer,
                    0,
                    bytes);
            }

            Console.WriteLine(message);
            connection.Close();
        }
    }
}
