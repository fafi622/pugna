using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace Server
{
    class Program
    {
        static Socket MainSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        static Socket SendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket RecSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static IPEndPoint MainAddress = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 25567);
        static IPEndPoint SendAddress = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 25568);
        static IPEndPoint RecAddress = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 25569);

        static byte[] RecBuff = new byte[1024];

        static List<User> Users = new List<User>();

        static void Main(string[] args)
        {
            InitSockets();
           

            while (true)
            {
               
            }
        }


        static void InitSockets()
        {
            //Main Socket
            MainSocket.Bind(MainAddress);
            MainSocket.Listen(100);
            MainSocket.BeginAccept(MainSocketOnAccept, MainSocket);
            //Send Socket
            SendSocket.Bind(SendAddress);
            SendSocket.Listen(100);
            SendSocket.BeginAccept(SendSocketOnSend, SendSocket);
            //Rec Socket
            RecSocket.Bind(RecAddress);
            RecSocket.Listen(100);
            RecSocket.BeginAccept(RecSocketOnAccept, RecSocket);
            

        }

        //===========================================Main Socket Code=========================================================================================================
        static void MainSocketOnAccept(IAsyncResult ar)
        {
            Socket client = ((Socket)ar.AsyncState).EndAccept(ar);
            User user = new User();
            user.SetMainSocket(client);
            user.GenerateID();
            foreach(User u in Users)
            {
                if(u.GetID() != user.GetID())
                {
                    u.SendMessaage("2|" + user.GetID());
                }
            }
            user.SendMessaage("0|" + user.GetID());
            Users.Add(user);
            MainSocket.BeginAccept(MainSocketOnAccept, MainSocket);
        }
        //====================================================================================================================================================================

        //===========================================Send Socket Code=========================================================================================================
      

        static void SendSocketOnSend(IAsyncResult ar)
        {
            Console.WriteLine("Message Sent");

        }
        //====================================================================================================================================================================

        //===========================================Rec Socket Code=========================================================================================================
        static void RecSocketOnAccept(IAsyncResult ar)
        {
            Socket client = ((Socket)ar.AsyncState).EndAccept(ar);
            client.BeginReceive(RecBuff, 0, RecBuff.Length, SocketFlags.None, RecSocketOnRec, client);
        }

        static void RecSocketOnRec(IAsyncResult ar)
        {
            Socket client = ((Socket)ar.AsyncState);
            User user = null;
            
            Console.WriteLine("Message Rececived");
            string a = Encoding.Unicode.GetString(RecBuff).TrimEnd('\0');
            string[] message = a.Split('|');

            for (int i = 0; i < Users.Count; i++)
            {
                if (message[1] == Users[i].GetID().ToString())
                {
                    user = Users[i];
                }
            }
            switch (message[0])
            {
                case "0":
                    for (int i = 0; i < Users.Count; i++)
                    {
                        if (Users[i].GetID().ToString() == message[1])
                        {
                            Users[i].SetUsername(message[2]);

                            break;
                        }
                    }
                    break;

                case "1":
                    Console.WriteLine("Code 1 used");
                    Console.WriteLine(a);
                    for (int i = 0; i < Users.Count; i++)
                    {
                        if (Users[i].GetID().ToString() == message[1])
                        {
                          
                            
                            Users[i].SendMessaage("1|0|500|0");
                            Console.WriteLine("Found user");
                            break;
                        }
                    }
                    break;
                case "2":
                    Console.WriteLine("Code 2 used");
                    for (int i = 0; i < Users.Count; i++)
                    {
                        if (Users[i].GetID().ToString() == message[1])
                        {

                            Users[i].SetPosition(message[2]);
                            
                            Console.WriteLine(Users[i].GetUsername() + ": " + Users[i].GetPosition());
                            break;
                        }
                    }
                    break;
            }
            
         
          
            Array.Clear(RecBuff, 0, RecBuff.Length);
            client.BeginReceive(RecBuff, 0, RecBuff.Length, SocketFlags.None, RecSocketOnRec, client);

        }
        //====================================================================================================================================================================

    }
}
