using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using NUnit.Framework;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Should;

namespace Tests
{
    [TestFixture]
    public class PublishTests
    {
        public class Message
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime Birthday { get; set; }
        }

        [Test]
        public void should_publish_message()
        {
            ClearQueue();

            PublishMessage();

            var message = DequeueMessage();

            message.Name.ShouldEqual("Rod");
            message.Age.ShouldEqual(55);
            message.Birthday.ShouldEqual(new DateTime(1985, 10, 26));
        }

        private static void ClearQueue()
        {
            using (var connection = new ConnectionFactory { Uri = "amqp://localhost/" }.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueuePurge("testqueue");
            }
        }

        private static Message DequeueMessage()
        {
            using (var connection = new ConnectionFactory { Uri = "amqp://localhost/" }.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume("testqueue", false, "", consumer);
                var json = Encoding.UTF8.GetString(consumer.Queue.Dequeue().Body);
                Console.WriteLine(json);
                return new JavaScriptSerializer().Deserialize<Message>(json);
            }
        }

        private static void PublishMessage()
        {
            try
            {
                new WebClient().DownloadString("http://localhost/RabbitMQTestHarness/publish.asp");
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
