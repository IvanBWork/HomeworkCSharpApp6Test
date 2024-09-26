using HomeworkCSharpApp6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace HomeworkCSharpApp6
{
    public class Server
    {
        Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();

        IMessageSource messageSource;

        public Server(IMessageSource source)
        {
            messageSource = source;
        }

        public void Register(MessageUDP message, IPEndPoint fromep)
        {
            Console.WriteLine("Message Register, name = " + message.FromName);
            clients.Add(message.FromName, fromep);
            using (var ctx = new Context())
            {
                if (ctx.Users.FirstOrDefault(x => x.Name == message.FromName)
                != null) return;
                ctx.Add(new User { Name = message.FromName });
                ctx.SaveChanges();
            }
        }

        public void ConfirmMessageReceived(int? id)
        {
            Console.WriteLine("Message confirmation id=" + id);
            using (var ctx = new Context())
            {
                var msg = ctx.Messages.FirstOrDefault(x => x.Id == id);
                if (msg != null)
                {
                    msg.Received = true;
                    ctx.SaveChanges();
                }
            }
        }

        public void RelyMessage(MessageUDP message)
        {
            int? id = null;
            if (clients.TryGetValue(message.ToName, out IPEndPoint ep))
            {
                using (var ctx = new Context())
                {
                    var fromUser = ctx.Users.First(x => x.Name == message.FromName);
                    var toUser = ctx.Users.First(x => x.Name == message.ToName);
                    var msg = new Message
                    {
                        FromUser = fromUser,
                        ToUser = toUser,
                        Received = false,
                        Text = message.Text
                    };
                    ctx.Messages.Add(msg);
                    ctx.SaveChanges();
                    id = msg.Id;
                }

                var forwardMessage = new MessageUDP()
                {
                    Id = id,
                    Command = Command.Message,
                    ToName = message.ToName,
                    FromName = message.FromName,
                    Text = message.Text
                };

                messageSource.Send(forwardMessage, ep);
                Console.WriteLine($"Message Relied, from = {message.FromName} to = { message.ToName}");
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }
        }
        public void ProcessMessage(MessageUDP message, IPEndPoint fromep)
        {
            Console.WriteLine($"Получено сообщение от {message.FromName} для { message.ToName} с командой { message.Command}:");
            Console.WriteLine(message.Text);
            if (message.Command == Command.Register)
            {
                Register(message, new IPEndPoint(fromep.Address, fromep.Port));
            }
            if (message.Command == Command.Confirmation)
            {
                Console.WriteLine("Confirmation receiver");
                ConfirmMessageReceived(message.Id);
            }
            if (message.Command == Command.Message)
            {
                RelyMessage(message);
            }
        }

        public void Work()
        {
            Console.WriteLine("UDP Клиент ожидает сообщений...");
            while (true)
            {
                try
                {
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    var message = messageSource.Receive(ref remoteEndPoint);
                    ProcessMessage(message, remoteEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                }
            }
        }

        public void Stop()
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
