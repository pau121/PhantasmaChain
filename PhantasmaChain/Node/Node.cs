﻿using Phantasma.Network;
using Phantasma.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Phantasma.Node
{
    public sealed class Node
    {
        public const int Port = 9600;
        public const int MaxConnections = 64;

        public Node(IEnumerable<Endpoint> seeds)
        {
            var listener = new EventBasedNetListener();
            var server = new NetManager(listener, MaxConnections, "Phantasma");

            if (seeds.Any())
            {
                foreach (var seed in seeds)
                {
                    server.Connect(seed);
                }
            }
            else
            {
                server.Start(Port);
            }

            listener.PeerConnectedEvent += peer =>
            {
                Log.Message($"Got connection: {peer.EndPoint}"); // Show peer ip
                var writer = new NetDataWriter();
                writer.Put("Hello client!");                                // Put some string
                peer.Send(writer, SendOptions.ReliableOrdered);             // Send with reliability
            };

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }

            server.Stop();
        }
    }
}