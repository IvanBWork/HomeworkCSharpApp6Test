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
    public interface IMessageSource
    {
        public void Send(MessageUDP message, IPEndPoint ep);
        MessageUDP Receive(ref IPEndPoint ep);
    }
}
