using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using NUnit.Framework;
using System.IO;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Should;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace Tests
{
    [TestFixture]
    public class RpcTests
    {
        public class Message
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime Birthday { get; set; }
        }

        [Test]
        public void should_do_rpc()
        {
            RespondToRpc();

            var message = Call();

            message.Name.ShouldEqual("Rod");
            message.Age.ShouldEqual(55);
            message.Birthday.ShouldEqual(new DateTime(1985, 10, 26));
        }

        private static void RespondToRpc()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                using (var connection = new ConnectionFactory { Uri = "amqp://localhost/" }.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    var subscription = new Subscription(channel, "testqueue");

                    var server = new EchoRpcServer(subscription);

                    server.MainLoop();
                }
            });
        }

        internal class EchoRpcServer : SimpleRpcServer
        {
            public EchoRpcServer(Subscription subscription)
                : base(subscription) { }

            public override byte[] HandleSimpleCall(
                bool isRedelivered, IBasicProperties requestProperties,
                byte[] body, out IBasicProperties replyProperties)
            {
                replyProperties = requestProperties;
                replyProperties.MessageId = Guid.NewGuid().ToString();
                return body;
            }
        }

        private static Message Call()
        {
            try
            {
                return new JavaScriptSerializer().Deserialize<Message>(new WebClient().DownloadString("http://localhost/RabbitMQTestHarness/rpc.asp"));
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
