using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace PokemonTCG
{   
    class Network
    {
        TcpListener listener;

       public Network(string dns, int port)
       {
           listener = new TcpListener(IPAddress.Parse(dns), 8000);
           listener.Start();

       }
    }
}
