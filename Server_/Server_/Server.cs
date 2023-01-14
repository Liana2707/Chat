using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Protocol;

namespace TcpServer
{
    class Server
    {
        private Socket socketServer;
        protected List<ClientOnServer> clients;

        public Server()
        {
            socketServer = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            clients = new List<ClientOnServer>();
        }

        public void Bind(string sIp, int nPort)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(sIp), nPort);
            socketServer.Bind(ep);
            socketServer.Listen(30);
        }

        public void Close()
        {
            socketServer.Close();
        }

        public void StartAccept()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadAccept));
        }

        private void ThreadAccept(object ob)
        {
            while (true)
            {
                Socket socketClient = socketServer.Accept();
                ClientOnServer client = new ClientOnServer(socketClient, this, "");
                client.StartReceive();
                clients.Add(client);
                Console.WriteLine("client #" + clients.Count.ToString() + " connected");
            }
        }

        public void ProcessPacket(Packet packet, ClientOnServer client)
        {
            switch (packet.Type)
            {
                case PacketType.SimpleMessage:
                    {
                        string Name = packet.GetItem(0);
                        string sText = packet.GetItem(1);
                        for (int i = 0; i < clients.Count; i++)
                            clients[i].SendSimpleMessage(Name, sText);
                    }
                    break;
                case PacketType.Login:
                    {
                        string sName = packet.GetItem(0);
                        bool flag = false;
                        for (int i = 0; i < clients.Count; i++)
                        {
                            if (sName == clients[i].sName)
                                flag = true;
                        }
                        client.SendLoginResult(flag);
                        if (flag == false)
                            client.sName = sName;

                        for (int i = 0; i < clients.Count; i++)
                            clients[i].SendList(clients);
                    }
                    break;
            }
        }
    }
}

