using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System;

public class TCPEchoServerSocket : MonoBehaviour
{

    //http://codeplanet.eu/tutorials/csharp/4-tcp-ip-socket-programmierung-in-csharp.html?start=1
    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            throw new System.ArgumentException("Parameters: [<Port>]");
        }

        int servPort = (args.Length == 1) ? Int32.Parse(args[0]) : 7;

        Socket servSock = null;

        try
        {
            // Verwende explizit IPv6
            servSock = new Socket(AddressFamily.InterNetworkV6,
                                  SocketType.Stream,
                                  ProtocolType.Tcp);

            // Assoziiert den Socket mit einem lokalen Endpunkt
            servSock.Bind(new IPEndPoint(IPAddress.IPv6Any, servPort));

            // Versetzt den Socket in den aktiven Abhörmodus 
            servSock.Listen(BACKLOG);
        }
        catch (SocketException se)
        {
            Console.WriteLine(se.ErrorCode + ": " + se.Message);
            Environment.Exit(se.ErrorCode);
        }

        byte[] rcvBuffer = new byte[BUFSIZE];
        int bytesRcvd;

        // Lässt den Server in einer Endlosschleife laufen
        for (; ; )
        {
            Socket client = null;

            try
            {
                client = servSock.Accept();

                Console.Write("Handling client at " + client.RemoteEndPoint + " - ");

                // Empfange bis der client die Verbindung schließt, das geschieht indem 0
                // zurückgegeben wird
                int totalbytesEchoed = 0;
                while ((bytesRcvd = client.Receive(rcvBuffer, 0, rcvBuffer.Length,
                                                   SocketFlags.None)) > 0)
                {
                    client.Send(rcvBuffer, 0, bytesRcvd, SocketFlags.None);
                    totalbytesEchoed += bytesRcvd;
                }
                Console.WriteLine("echoed {0} bytes.", totalbytesEchoed);

                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client.Close();
            }
        }
    }

    private const int BUFSIZE = 32;
    private const int BACKLOG = 5;
}
