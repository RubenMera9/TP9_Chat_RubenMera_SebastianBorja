using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private static TcpClient client;
    private static NetworkStream stream;

    static void Main(string[] args)
    {
        try
        {
            client = new TcpClient("172.20.11.15", 8888);
            stream = client.GetStream();

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessages));
            receiveThread.Start();

            Console.WriteLine("Cliente conectado. Puede empezar a enviar mensajes...");

            while (true)
            {
                string message = Console.ReadLine();
                SendMessage(message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        finally
        {
            stream.Close();
            client.Close();
        }
    }

    private static void SendMessage(string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        stream.Write(buffer, 0, buffer.Length);
    }

    private static void ReceiveMessages()
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Mensaje recibido: " + message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }
}