using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace Server
{
    class User
    {
        private Socket MainSocket;
        private Socket SendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public byte[] recbuff = new byte[1024];

        private int ID;
        private string Username;

        private float[] Position = { 0, 0, 0 };

        public void SendMessaage(string Input)
        {
            byte[] SendBuff = new byte[1024];
            SendBuff = Encoding.Unicode.GetBytes(Input);
            MainSocket.BeginSend(SendBuff, 0, SendBuff.Length, SocketFlags.None, onSend, MainSocket);
        }
       
        public void SetMainSocket(Socket socket)
        {
            MainSocket = socket;
            IPEndPoint remoteIpEndPoint = MainSocket.RemoteEndPoint as IPEndPoint;
            IPEndPoint g = new IPEndPoint(IPAddress.Parse(remoteIpEndPoint.Address.ToString()), remoteIpEndPoint.Port);
            
            SendSocket.Bind(g);
            
        }
        public Socket GetMainSocket()
        {
            return MainSocket;
        }

        public void SetSendSocket(Socket socket)
        {
            SendSocket = socket;
        }
        public Socket GetSendSocket(Socket socket)
        {
            return SendSocket;
        }

        public void SetUsername(string input)
        {
            Username = input;
        }
        public string GetUsername()
        {
            return Username;
        }

        public int GetID()
        {
            return ID;
        }

        public void GenerateID()
        {
            Random rn = new Random();
            ID = rn.Next(100);
           
        }

        public void onSend(IAsyncResult ar)
        {
            Console.WriteLine("MESSAGE SENT");
        }

        public void SetPosition(string a)
        {
            string[] message = a.Split('$');
            Position[0] = float.Parse(message[0]);
            Position[1] = float.Parse(message[1]);
            Position[2] = float.Parse(message[2]);
        }
        public String GetPosition()
        {
            return (Position[0] + "$" + Position[1] + "$" + Position[2]);
        }
    }
}
