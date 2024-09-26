namespace HomeworkCSharpApp6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                IMessageSource source;
                source = new UdpMessageSource();
                Server server = new Server(source);
                server.Work();
            }
            else
            {
                Client client = new Client("Ivan", "127.0.0.1", 12345);
                client.Start();
            }
        }
    }
}
