using HomeworkCSharpApp6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkCSharpApp6
{
    public class Client
    {
        string name;
        string address;
        int port;
        public Client(string n, string a, int p)
        {
            this.name = n;
            this.address = a;
            this.port = p;
        }

        UdpClient udpClientClient = new UdpClient();

        public void ClientListener()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(address), 12345);
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(address), 12345);
            while (true)
            {
                try
                {
                    byte[] receiveBytes = udpClientClient.Receive(ref remoteEndPoint);
                    string receivedData = Encoding.ASCII.GetString(receiveBytes);
                    var messageReceived = MessageUDP.FromJson(receivedData);
                    Console.WriteLine($"Получено сообщение от { messageReceived.FromName}:");
                    Console.WriteLine(messageReceived.Text);
                    Confirm(messageReceived, remoteEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при получении сообщения: " +
                    ex.Message);
                }
            }
        }

        public void Confirm(MessageUDP m, IPEndPoint remoteEndPoint)
        {
            var messageJson = new MessageUDP()
            {
                FromName = name,
                ToName = null,
                Text = null,
                Id = m.Id,
                Command = Command.Confirmation
            }.ToJson();

            byte[] messageBytes = Encoding.ASCII.GetBytes(messageJson);
            udpClientClient.Send(messageBytes, messageBytes.Length,
            remoteEndPoint);
        }

        public void Register(IPEndPoint remoteEndPoint)
        {
            var messageJson = new MessageUDP()
            {
                FromName = name,
                ToName = null,
                Text = null,
                Command = Command.Register
            }.ToJson();

            byte[] messageBytes = Encoding.ASCII.GetBytes(messageJson);
            udpClientClient.Send(messageBytes, messageBytes.Length,
            remoteEndPoint);
        }

        public void ClientSender()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(address), 12345);
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(address), 12345);
            Register(remoteEndPoint);
            while (true)
            {
                try
                {
                    Console.WriteLine("UDP Клиент ожидает ввода сообщения");
                    Console.Write("Введите имя получателя и сообщение и нажмите Enter: ");
                    var messages = Console.ReadLine().Split(' ');
                    var messageJson = new MessageUDP()
                    {
                        Command = Command.Message,
                        FromName = name,
                        ToName = messages[0],
                        Text = messages[1]
                    }.ToJson();

                    byte[] messageBytes = Encoding.ASCII.GetBytes(messageJson);
                    udpClientClient.Send(messageBytes, messageBytes.Length, remoteEndPoint);
                    Console.WriteLine("Сообщение отправлено.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения: " +
                    ex.Message);
                }
            }
        }

        public void Start()
        {
            udpClientClient = new UdpClient(port);
            new Thread(() => ClientListener()).Start();
            ClientSender();
        }
    }
}
