using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI.WebControls;
using NUnit.Framework;
using RestSharp.Serializers;
using Should;
using System.IO;

namespace Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        public class Message
        {
            public string Body { get; set; }
        }

        [Test]
        public void should()
        {
            const string path = "c:\\temp\\result.txt";
            if (File.Exists(path)) File.Delete(path);
            Post(new Message { Body = "hai" });
            File.ReadAllText(path).ShouldEqual("yada4hai");
        }

        private static void Post<T>(T message)
        {
            try
            {
                var client = new WebClient();
                client.Headers.Add("Content-Type", "application/xml");

                client.UploadData("http://localhost/RabbitMQTestHarness/rabbitmq.asp", "POST", 
                    Encoding.ASCII.GetBytes(new XmlSerializer().Serialize(message)));
            }
            catch (WebException exception)
            {
                Console.WriteLine(Regex.Replace(new StreamReader(exception.Response.GetResponseStream()).ReadToEnd(), @"<[^>]*>", String.Empty));
                Console.WriteLine();
                throw;
            }
        }
    }
}
