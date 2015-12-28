using System;

namespace DarkIrc.Messages
{
    public class PingPong : IMessageHandler
    {
        public void HandleMessage(string rawText, IrcConnection ircConnection)
        {
            string pingID = rawText.Substring(rawText.IndexOf(":"));
            ircConnection.IrcIO.SendRaw("PONG " + pingID);
        }
    }
}

