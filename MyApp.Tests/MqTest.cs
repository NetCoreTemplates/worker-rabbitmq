using System.Threading.Tasks;
using MyApp.ServiceModel;
using NUnit.Framework;
using ServiceStack.Messaging;
using ServiceStack.RabbitMq;

namespace MyApp.Tests
{
    public class MqTest
    {
        private readonly RabbitMqMessageFactory MqFactory;
        public MqTest()
        {
            MqFactory = new RabbitMqMessageFactory("localhost:5672");
        }

        [Test] // requires running Host MQ Server project
        public void Can_send_Request_Reply_message()
        {
            using (var mqClient = MqFactory.CreateMessageQueueClient())
            {
                var replyToMq = mqClient.GetTempQueueName();

                mqClient.Publish(new Message<Hello>(new Hello { Name = "MQ Worker" })
                {
                    ReplyTo = replyToMq,
                });

                var responseMsg = mqClient.Get<HelloResponse>(replyToMq);
                mqClient.Ack(responseMsg);
                Assert.That(responseMsg.GetBody().Result, Is.EqualTo("Hello, MQ Worker!"));
            }
        }
    }
}