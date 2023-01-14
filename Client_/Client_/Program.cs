using System;
using System.Text;


namespace TcpClient
{
    class Program
    {
        static void Main()
        {
            Client client = new Client();
            client.Connect("127.0.0.1", 2021);
            Console.WriteLine("connected to server");
            client.StartReceive();

            while (client.al_den != true)
            {
                Console.Write("enter your login: ");
                string sName = Console.ReadLine();
                client.SendLogin(sName);
                System.Threading.Thread.Sleep(100);
                if (client.al_den == true)
                    client.name = sName;
            }

            while (true)
            {
                Console.Write("enter message text: ");
                string sText = Console.ReadLine();
                client.SendSimpleMessage(client.name, sText);
                System.Threading.Thread.Sleep(100);
            }
            client.Close();
        }


    }
}

