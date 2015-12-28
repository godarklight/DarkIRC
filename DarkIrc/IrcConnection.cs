using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DarkIrc
{
    public class IrcConnection
    {
        private string username;
        public string GetName()
        {
            return username;
        }
        private string password;
        private string serverAddress;
        private int port = 6667;
        private TcpClient connection;
        private byte[] buffer = new byte[1024];
        private int bufferPos = 0;
        private object disconnectLock = new object();
        private Thread sendThread;
        private Thread receiveThread;
        private AutoResetEvent sendEvent = new AutoResetEvent(false);
        private Queue<string> sendMessages = new Queue<string>();
        private IrcProtocol ircProtocol = new IrcProtocol();
        private IrcEvents ircEvents = new IrcEvents();
        public IrcEvents IrcEvents
        {
            get
            {
                return ircEvents;
            }
        }

        private IrcIO ircIO;

        public IrcIO IrcIO
        {
            get
            {
                return ircIO;
            }
        }

        public bool Connected
        {
            private set;
            get;
        }

        public IrcConnection(string serverAddress, string username)
        {
            this.serverAddress = serverAddress;
            this.username = username;
            SetupDependancies();
        }

        public IrcConnection(string serverAddress, string username, string password)
        {
            this.serverAddress = serverAddress;
            this.username = username;
            this.password = password;
            SetupDependancies();
        }

        public IrcConnection(string serverAddress, int port, string username)
        {
            this.serverAddress = serverAddress;
            this.port = port;
            this.username = username;
            SetupDependancies();
        }

        public IrcConnection(string serverAddress, int port, string username, string password)
        {
            this.serverAddress = serverAddress;
            this.port = port;
            this.username = username;
            this.password = password;
            SetupDependancies();
        }

        private void SetupDependancies()
        {
            ircProtocol.SetupDependancies(this, ircEvents, sendMessages, sendEvent);
            ircIO = new IrcIO(ircProtocol);
        }

        public bool Connect()
        {
            if (Connected)
            {
                return true;
            }
            connection = new TcpClient(AddressFamily.InterNetworkV6);
            try
            {
                IAsyncResult ar = connection.BeginConnect(serverAddress, port, null, null);
                ar.AsyncWaitHandle.WaitOne(10000);
                if (ar.IsCompleted)
                {
                    connection.EndConnect(ar);
                }
                else
                {
                    connection.Close();
                    ircEvents.Log("Connection failed: Timeout");
                    return false;
                }
            }
            catch (Exception e)
            {
                ircEvents.Log("Connection failed: " + e.Message);
                Disconnect();
                return false;
            }
            receiveThread = new Thread(new ThreadStart(ReadLoop));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            sendThread = new Thread(new ThreadStart(SendLoop));
            sendThread.IsBackground = true;
            sendThread.Start();
            ircProtocol.Connect(username, password);
            ircEvents.OnConnect();
            return true;
        }

        public void Disconnect()
        {
            lock (disconnectLock)
            {
                if (!Connected)
                {
                    return;
                }
                ircProtocol.Disconnect();
                sendThread = null;
                receiveThread = null;
                if (connection != null && connection.Connected)
                {
                    ircEvents.Log("Connection closed");
                    try
                    {
                        connection.Close();
                    }
                    catch
                    {
                        //Don't care.
                    }
                }
                if (connection != null)
                {
                    connection = null;
                }
                Connected = false;
                ircEvents.Log("Disconnected");
                ircEvents.OnDisconnect();
            }
        }

        private void ReadLoop()
        {
#if !DEBUG
            try
            {
#endif
            while (connection != null)
            {
                int bytesRead = connection.GetStream().Read(buffer, bufferPos, 1);
                if (bytesRead == 0)
                {
                    ircEvents.Log("Connection closed");
                    Disconnect();
                    return;
                }
                else
                {
                    bufferPos += bytesRead;
                    if (buffer[bufferPos - 1] == 10)
                    {
                        //Also strip /r out
                        if (buffer[bufferPos - 2] == 13)
                        {
                            bufferPos--;
                        }
                        string receiveMessage = UTF8Encoding.UTF8.GetString(buffer, 0, bufferPos - 1);
                        ircProtocol.HandleMessage(receiveMessage);
                        bufferPos = 0;
                    }
                    if (bufferPos == 1024)
                    {
                        ircEvents.Log("Connection error: IRC protocol length >1024");
                        Disconnect();
                        return;
                    }
                }
            }
#if !DEBUG
            }
            catch (Exception e)
            {
                IrcEvents.Log("Connection error: " + e.Message);
                Disconnect();
            }
#endif
        }

        private void SendLoop()
        {
            try
            {
                while (connection != null)
                {
                    string sendMessage = null;
                    lock (sendMessages)
                    {
                        if (sendMessages.Count > 0)
                        {
                            sendMessage = sendMessages.Dequeue();
                        }
                    }
                    if (sendMessage != null)
                    {
                        ircEvents.Log("Sending: " + sendMessage);
                        //byte[] sendBytes = UTF8Encoding.UTF8.GetBytes(sendMessage + "\r\n");
                        byte[] sendBytes = UTF8Encoding.UTF8.GetBytes(sendMessage + "\n");
                        if (sendBytes.Length > 512)
                        {
                            ircEvents.Log("Protocol error: Tried to send a message longer than 512 bytes");
                            Disconnect();
                            return;
                        }
                        connection.GetStream().Write(sendBytes, 0, sendBytes.Length);
                    }
                    else
                    {
                        sendEvent.WaitOne(1000);
                    }
                }
            }
            catch (Exception e)
            {
                ircEvents.Log("Connection error: " + e.Message);
                Disconnect();
            }
        }
    }
}

