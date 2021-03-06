﻿using System;
using DarkIrc;

namespace IrcTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            IrcConnection ircC = new IrcConnection("irc.esper.net", "darkbot");
            //ircC.IrcEvents.Logger = Console.WriteLine;
            ircC.IrcEvents.JoinEvent += (channel, user) =>
            {
                Console.WriteLine(user + " joined " + channel);
            };
            ircC.IrcEvents.KickEvent += (channel, user) =>
            {
                Console.WriteLine(user + " kicked from " + channel);
                if (user == "darkbot" && channel == "#DMP")
                {
                    System.Threading.Thread.Sleep(1000);
                    ircC.IrcIO.JoinChannel("#DMP");
                }
            };
            ircC.IrcEvents.PartEvent += (channel, user) =>
            {
                Console.WriteLine(user + " parted from " + channel);
            };
            ircC.IrcEvents.DisconnectEvent += () =>
            {
                System.Threading.Thread.Sleep(1000);
                ircC.Connect();
            };
            ircC.IrcEvents.ChannelMessageEvent += (channel, user, message) =>
            {
                Console.WriteLine(channel + " <" + user + "> " + message);
            };
            ircC.IrcEvents.ChannelActionMessageEvent += (channel, user, message) =>
            {
                Console.WriteLine(channel + " *" + user + " " + message);
            };
            ircC.IrcEvents.ChannelCtcpMessageEvent += (channel, user, message) =>
            {
                Console.WriteLine(channel + " CTCP " + user + " " + message);
            };
            ircC.IrcEvents.PrivateMessageEvent += (user, message) =>
            {
                Console.WriteLine("<" + user + "> " + message);
            };
            ircC.IrcEvents.PrivateActionMessageEvent += (user, message) =>
            {
                Console.WriteLine("*" + user + " " + message);
            };
            ircC.IrcEvents.PrivateCtcpMessageEvent += (user, message) =>
            {
                Console.WriteLine("CTCP " + user + " " + message);
            };
            ircC.Connect();
            System.Threading.Thread.Sleep(5000);
            ircC.IrcIO.JoinChannel("#DMP");
            ircC.IrcIO.JoinChannel("#kspmodders");
            string currentLine = null;
            string currentChannel = "#kspmodders";
            while ((currentLine = Console.ReadLine()) != "QUIT")
            {
                if (currentLine.ToUpper().StartsWith("/SWITCH "))
                {
                    currentChannel = currentLine.Substring(currentLine.IndexOf(" ") + 1);
                }
                if (currentLine.ToUpper().StartsWith("/JOIN "))
                {
                    ircC.IrcIO.JoinChannel(currentLine.Substring(currentLine.IndexOf(" ") + 1));
                }
                if (currentLine.ToUpper().StartsWith("/PART "))
                {
                    ircC.IrcIO.PartChannel(currentLine.Substring(currentLine.IndexOf(" ") + 1));
                }
                if (currentLine.ToUpper().StartsWith("/ME "))
                {
                    ircC.IrcIO.SendActionMessage(currentChannel, currentLine.Substring(currentLine.IndexOf(" ") + 1));
                }
                if (currentLine.ToUpper().StartsWith("/CTCP "))
                {
                    string target = currentLine.Substring(6);
                    target = target.Substring(0, target.IndexOf(" "));
                    string message = currentLine.Substring(7 + target.Length);
                    ircC.IrcIO.SendCtcpMessage(target, message);
                }
                if (currentLine == "/DEBUG")
                {
                    if (ircC.IrcEvents.Logger == null)
                    {
                        ircC.IrcEvents.Logger = Console.WriteLine;
                    }
                    else
                    {
                        ircC.IrcEvents.Logger = null;
                    }
                }
                if (!currentLine.StartsWith("/"))
                {
                    ircC.IrcIO.SendMessage(currentChannel, currentLine);
                }
            }
        }
    }
}
