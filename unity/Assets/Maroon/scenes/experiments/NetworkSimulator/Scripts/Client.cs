using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{

    public  String clientIp;
    public  int portNr;
    private void Start()
    {
        setupConnection();
    }

    private  void setupConnection()
    {
        // Design the client a bit
        Console.Title = "Socket Client";

        Console.Write("Enter the IP of the server: ");
        IPAddress clientIP = IPAddress.Parse(clientIp);
        String message = String.Empty;

        while (true)
        {
            Console.Write("Enter the message to send: ");
            // The messsage to send
            // message = Console.ReadLine();
            message = "Hello I am the client!";

            IPEndPoint clientEP = new IPEndPoint(clientIP, portNr);

            // Setup the socket
            Socket clientSock = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            // Attempt to establish a connection to the server
            Console.Write("Establishing connection to the server... ");
            try
            {
                clientSock.Connect(clientEP);

                // Send the message
                clientSock.Send(Encoding.UTF8.GetBytes(message));
                clientSock.Shutdown(SocketShutdown.Both);
                clientSock.Close();
                Console.Write("Message sent successfully.\n\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
