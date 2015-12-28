using System;
using System.Collections.Generic;
using System.Threading;

namespace DarkIrc
{
    public class IrcProtocol
    {
        private IrcConnection ircConnection;
        private IrcEvents ircEvents;
        private Dictionary<string, IMessageHandler> messageHandlers = new Dictionary<string, IMessageHandler>();
        private Queue<string> sendMessages;
        private AutoResetEvent sendEvent;

        internal void SetupDependancies(IrcConnection ircConnection, IrcEvents ircEvents, Queue<string> sendMessages, AutoResetEvent sendEvent)
        {
            this.ircConnection = ircConnection;
            this.ircEvents = ircEvents;
            this.sendMessages = sendMessages;
            this.sendEvent = sendEvent;
            //Setup events
            messageHandlers.Add("PING", new Messages.PingPong());
            messageHandlers.Add("JOIN", new Messages.JoinPart());
            messageHandlers.Add("PART", messageHandlers["JOIN"]);
            messageHandlers.Add("KICK", new Messages.Kick());
            messageHandlers.Add("PRIVMSG", new Messages.PrivMsg());
        }

        public void Connect(string username, string password)
        {
            if (password != null)
            {
                SendMessage("PASS " + password);
            }
            SendMessage("NICK " + username);
            SendMessage("USER " + username + " 8 * : " + username);
        }

        public void Disconnect()
        {
            SendMessage("QUIT");
        }

        public void HandleMessage(string ircMessage)
        {
            ircEvents.Log("Receiving: " + ircMessage);
            ircEvents.OnRawMessage(ircMessage);
            string command = null;
            string[] split = ircMessage.Split(' ');
            if (ircMessage.StartsWith(":") && ircMessage.Length > 1)
            {
                command = split[1];
            }
            else
            {
                command = split[0];
            }
            if (command != null && messageHandlers.ContainsKey(command))
            {
                messageHandlers[command].HandleMessage(ircMessage, ircConnection);
            }
        }

        public void SendMessage(string ircMessage)
        {
            lock (sendMessages)
            {
                sendMessages.Enqueue(ircMessage);
            }
            sendEvent.Set();
        }
    }
}


