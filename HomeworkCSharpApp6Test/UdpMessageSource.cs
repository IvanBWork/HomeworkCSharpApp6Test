using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkCSharpApp6
{
    public class UdpMessageSource : IMessageSource
    {
        private UdpClient udpClient;
        public UdpMessageSource()
        {
            udpClient = new UdpClient(12345);
        }
        public MessageUDP Receive(ref IPEndPoint ep)
        {
            byte[] receiveBytes = udpClient.Receive(ref ep);
            string receivedData = Encoding.ASCII.GetString(receiveBytes);
            return MessageUDP.FromJson(receivedData);
        }
        public void Send(MessageUDP message, IPEndPoint ep)
        {
            byte[] forwardBytes = Encoding.ASCII.GetBytes(message.ToJson());
            udpClient.Send(forwardBytes, forwardBytes.Length, ep);
        }
    }
}
