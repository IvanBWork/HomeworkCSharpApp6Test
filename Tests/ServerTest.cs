using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeworkCSharpApp6;
using HomeworkCSharpApp6.Models;

namespace Tests
{
    public class ServerTest
    {
        [SetUp]
        public void SetUp()
        {
            using (var ctx = new Context())
            {
                ctx.Messages.RemoveRange(ctx.Messages);
                ctx.Users.RemoveRange(ctx.Users);
                ctx.SaveChanges();
            }
        }

        [TearDown]
        public void TearDown()
        {
            using (var ctx = new Context())
            {
                ctx.Messages.RemoveRange(ctx.Messages);
                ctx.Users.RemoveRange(ctx.Users);
                ctx.SaveChanges();
            }
        }

        [Test]
        public void Test()
        {
            var mock = new MockMessageSource();
            var srv = new Server(mock);
            mock.AddServer(srv);
            srv.Work();
            using (var ctx = new Context())
            {
                Assert.IsTrue(ctx.Users.Count() == 2, "Пользователи не созданы");
                var user1 = ctx.Users.FirstOrDefault(x => x.Name == "Вася");
                var user2 = ctx.Users.FirstOrDefault(x => x.Name == "Юля");
                Assert.IsNotNull(user1, "Пользователь не созданы");
                Assert.IsNotNull(user2, "Пользователь не созданы");
                Assert.IsTrue(user1.FromMessages.Count == 1);
                Assert.IsTrue(user2.FromMessages.Count == 1);
                Assert.IsTrue(user1.ToMessages.Count == 1);
                Assert.IsTrue(user2.ToMessages.Count == 1);
                var msg1 = ctx.Messages.FirstOrDefault(x => x.FromUser == user1 &&
                x.ToUser == user2);
                var msg2 = ctx.Messages.FirstOrDefault(x => x.FromUser == user2 &&
                x.ToUser == user1);
                Assert.AreEqual("От Юли", msg2.Text);
                Assert.AreEqual("От Васи", msg1.Text);
            }
        }
    }
}
