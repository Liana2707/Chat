using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Protocol;

namespace TcpServer
{
    class ClientOnServer
    {
        protected Socket socket;
        protected Server server;
        public string sName;

        public ClientOnServer(Socket _socket, Server _server, string _sname)
        {
            socket = _socket;
            server = _server;
            sName = _sname;
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
                server.ProcessPacket(packet, this);
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

        // для удобства отправки результата проверки имени
        public void SendLoginResult(bool bAllow)
        {
            Packet packet = new Packet(PacketType.Login, 1);
            string sResult = (bAllow) ? "deny" : "allow";
            if (sResult == "allow")
                packet.SetItem(0, sResult);
            Send(packet);
        }

        public void SendList(List<ClientOnServer> clients)
        {
            Packet packet = new Packet(PacketType.ClientList, clients.Count);
            for (int i = 0; i < clients.Count; i++)
                packet.SetItem(i, clients[i].sName);
            Send(packet);
        }

        public void Close()
        {
            socket.Close();
        }
    }
}
