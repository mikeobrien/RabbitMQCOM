using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace RabbitMQ
{
    [ComVisible(true), Guid("8ECF7EBC-BC37-4DD6-BF6E-E6152E543635")]
    [ProgId("UltravioletCatastrophe.RabbitMQ")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Connection
    {
        public void Publish(string message)
        {
            File.WriteAllText("c:\\temp\\result.txt", message);
        }
    }
}
