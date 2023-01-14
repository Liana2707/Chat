namespace TcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Bind("127.0.0.1", 2023);
            server.StartAccept();

            while (true) System.Threading.Thread.Sleep(100);
            server.Close();
        }
    }
}
