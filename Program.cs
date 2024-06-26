using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static List<TcpClient> clients = new List<TcpClient>();

    static void Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        server.Start();
        Console.WriteLine("Servidor iniciado...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            Console.WriteLine("Cliente conectado...");

            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Mensaje recibido: " + message);
                BroadcastMessage(message, client);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        finally
        {
            clients.Remove(client);
            client.Close();
        }
    }

    private static void BroadcastMessage(string message, TcpClient senderClient)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);

        foreach (var client in clients)
        {
            if (client != senderClient)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}