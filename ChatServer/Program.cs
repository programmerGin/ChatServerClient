using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        public static Hashtable clientList = new Hashtable();
        static void Main(string[] args)
        {
          TcpListener serverSocket =   new TcpListener(7000);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine("Chat Server Started....");

            while(true)
            {
                Console.WriteLine("Chat Server Waiting....");

                clientSocket =  serverSocket.AcceptTcpClient();
                counter++;

                byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                string dataFromClient = null;

                NetworkStream networkStream =  clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
                dataFromClient = Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                clientList.Add(dataFromClient, clientSocket);
                Console.WriteLine(dataFromClient + " Joined ");

                broadcast(dataFromClient + " Joined", dataFromClient, false);

                handleClient client =  new handleClient();
                client.startClient(clientSocket, dataFromClient, clientList);

            
            }

        }

        private static void broadcast(string msg, string uName, bool flag)
        {
            TcpClient broadcastSocket;
            NetworkStream broadcastStream;
            byte[] broadcastBytes = null;
            foreach (DictionaryEntry item in clientList)
            {
                 broadcastSocket = (TcpClient)item.Value;
                 broadcastStream =  broadcastSocket.GetStream();
                if (flag)
                    broadcastBytes = Encoding.ASCII.GetBytes(uName + " says :" +msg);
                else 
                    broadcastBytes = Encoding.ASCII.GetBytes(msg);
                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }

        private class handleClient
        {
            TcpClient clientSocket;
            string clNo;
            Hashtable clientList;


            public handleClient()
            {
            }

            public void startClient(TcpClient clientSocket, string dataFromClient, Hashtable clientList)
            {
                this.clientSocket = clientSocket;
                this.clNo = dataFromClient;
                this.clientList = clientList;

               Thread clThread =  new Thread(doChat);
                clThread.Start();
            }

            private void doChat()
            {
                byte[] byteFrom = new byte[clientSocket.ReceiveBufferSize];
                string dataFromClient = null;
                NetworkStream networkStream;
                while(true)
                {
                    try
                    {
                        networkStream = clientSocket.GetStream();
                        networkStream.Read(byteFrom, 0, clientSocket.ReceiveBufferSize);
                        dataFromClient = Encoding.ASCII.GetString(byteFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                        Console.WriteLine("From CLient - ", clNo, " : " + dataFromClient);

                        Program.broadcast(dataFromClient, clNo, true);
                    }catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }

            }
        }
    }
}
