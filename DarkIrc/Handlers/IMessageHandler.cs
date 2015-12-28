using System;

namespace DarkIrc
{
    public interface IMessageHandler
    {
        void HandleMessage(string rawText, IrcConnection ircConnection);
    }
}

