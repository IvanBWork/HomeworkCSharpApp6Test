using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HomeworkCSharpApp6;
using HomeworkCSharpApp6.Models;

namespace Tests
{
    public class MockMessageSource : IMessageSource
    {
        private Queue<MessageUDP> messages = new();
        private Server server;
        private IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        public MockMessageSource()
        {
            messages.Enqueue(new MessageUDP
            {
                Command = Command.Register,
                FromName = "Вася"
            });
            messages.Enqueue(new MessageUDP
            {
                Command = Command.Register,
                FromName = "Юля"
            });
            messages.Enqueue(new MessageUDP
            {
                Command = Command.Message,
                FromName = "Юля",
                ToName = "Вася",
                Text = "От Юли"
            });
            messages.Enqueue(new MessageUDP
            {
                Command = Command.Message,
                FromName = "Вася",
                ToName = "Юля",
                Text = "От Васи"
            });
        }
        public void AddServer(Server srv)
        {
            server = srv;
        }
        public MessageUDP Receive(ref IPEndPoint ep)
        {
            ep = endPoint;
            if (messages.Count == 0)
            {
                server.Stop();
                return null;
            }
            var msg = messages.Dequeue();
            return msg;
        }
        public void Send(MessageUDP message, IPEndPoint ep)
        {
            //throw new NotImplementedException();
        }

        public IPEndPoint GetServerIPEndPoint()
        {
            return endPoint;
        }

        public IPEndPoint CreateNewIPEndPoint()
        {
            return null!;
        }
    }
}
