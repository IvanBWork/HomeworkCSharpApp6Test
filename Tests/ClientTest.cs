using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HomeworkCSharpApp6;
using HomeworkCSharpApp6.Models;
using Moq;

namespace Tests
{
    public class ClientTest
    {
        private MessageUDP message;
        private IPEndPoint ipEndPoint;

        [SetUp]
        public void Setup()
        {
            message = new MessageUDP();
            ipEndPoint = new IPEndPoint(IPAddress.Parse("123.123.123.123"), 11111);
        }

        [Test]
        public void Test1()
        {
            var mock = new Mock<MockMessageSource>();

            mock.Setup(x => x.GetServerIPEndPoint()).Returns(ipEndPoint);

            var client = new Client("Name", "127.0.0.1", 12345);

            client.Register(ipEndPoint);

            mock.Verify(x => x.Send(It.IsAny<MessageUDP>(), ipEndPoint));
        }

        [Test]
        public void Test2()
        {
            var mock = new Mock<MockMessageSource>();

            var client = new Client("Name", "127.0.0.1", 12345);

            client.Confirm(message, ipEndPoint);

            mock.Verify(x => x.Send(It.IsAny<MessageUDP>(), ipEndPoint));
        }

        [Test]
        public void Test4()
        {
            var mock = new Mock<MockMessageSource>();

            mock.Setup(x => x.CreateNewIPEndPoint()).Returns(ipEndPoint);
            mock.Setup(x => x.Receive(ref ipEndPoint)).Returns(message);

            var client = new Client("Name", "127.0.0.1", 12345);

            Task.Run(() => client.ClientListener()).Wait(1);

            mock.Verify(x => x.Send(It.IsAny<MessageUDP>(), ipEndPoint));
        }
    }
}
