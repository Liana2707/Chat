using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Protocol;

namespace TcpClient
{
    class Client
    {
        private Socket socket;
        public bool al_den;
        public string name;

        public Client()
        {
            socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            al_den = false;
        }

        public void Connect(string sIp, int nPort)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(sIp), nPort);

            while (!socket.Connected)
            {
                try
                {
                    socket.Connect(ep);
                }
                catch (SocketException e)
                {
                    Thread.Sleep(50);
                }
            }
        }

        public void StartReceive()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadReceive));
        }

        void ThreadReceive(object ob)
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                // receive message
                int nRecv = socket.Receive(buffer);

                Packet packet = Packet.ParseBytes(buffer);
                ProcessPacket(packet);
            }
        }

        private void ProcessPacket(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.SimpleMessage:
                    {
                        string sName = packet.GetItem(0);
                        string sText = packet.GetItem(1);
                        Console.WriteLine(sName + ": " + sText);
                    }
                    break;
                case PacketType.ClientList:
                    {
                        Console.Write("\n Clients:");
                        for (int i = 0; i < packet.ItemCount; i++)
                        {
                            string sName = packet.GetItem(i);
                            Console.Write(sName + " ");
                        }
                        Console.Write("\n");
                    }
                    break;
                case PacketType.Login:
                    {
                        string boool = packet.GetItem(0);
                        if (boool == "allow")
                        {
                            al_den = true;
                        }
                    }
                    break;
            }
        }

        public void Send(Packet packet)
        {
            byte[] bufferSend = packet.ToBytes();
            socket.Send(bufferSend);
        }

        // для удобства отправки простого сообщения
        public void SendSimpleMessage(string sFrom, string sText)
        {
            Packet packet = new Packet(PacketType.SimpleMessage, 2);
            packet.SetItem(0, sFrom);
            packet.SetItem(1, sText);
            Send(packet);
        }
        // для удобства отправки имени серверу на проверку
        public void SendLogin(string sName)
        {
            Packet packet = new Packet(PacketType.Login, 1);
            packet.SetItem(0, sName);
            Send(packet);
        }

        public void Close()
        {
            socket.Close();
        }
    }
}
