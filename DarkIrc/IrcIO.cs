using System;

namespace DarkIrc
{
    public class IrcIO
    {
        private IrcProtocol ircProtocol;

        public IrcIO(IrcProtocol ircProtocol)
        {
            this.ircProtocol = ircProtocol;
        }

        public void SendActionMessage(string target, string message)
        {
            ircProtocol.SendMessage("PRIVMSG " + target + " :" + (char)1 + "ACTION " + message);
        }

        public void SendCtcpMessage(string target, string message)
        {
            ircProtocol.SendMessage("PRIVMSG " + target + " :" + (char)1 + message);
        }

        public void SendMessage(string target, string message)
        {
            ircProtocol.SendMessage("PRIVMSG " + target + " :" + message);
        }

        public void JoinChannel(string target)
        {
            SendRaw("JOIN " + target);
        }

        public void PartChannel(string target)
        {
            SendRaw("PART " + target);
        }

        public void KickUser(string channel, string user)
        {
            SendRaw("KICK #" + channel + " " + user);
        }

        public void SendRaw(string rawMessage)
        {
            ircProtocol.SendMessage(rawMessage);
        }
    }
}

