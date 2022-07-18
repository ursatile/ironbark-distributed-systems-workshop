using EasyNetQ;
using Messages.Commands;
using Messages.Events;

var AMQP = "amqps://xwugqrcr:VJHyIwkBhFsn8s5OBkGRkXD1ZGjcqyiJ@diligent-wheat-koala.rmq4.cloudamqp.com/xwugqrcr";
var bus = RabbitHutch.CreateBus(AMQP);

Console.WriteLine("Press any key to publish an event:");
while (true)
{
    var key = Console.ReadKey(true);
    var evt = new KeyWasPressed { Key = key.KeyChar.ToString() };
    bus.PubSub.Publish(evt);
    Console.WriteLine($"Published: KeyWasPressed (key: {evt.Key})");
}



    // var command = new DisplayNotification
    // {
    //     Content = $"Hello World from {Environment.MachineName} at {DateTime.UtcNow}"
    // };
    // bus.SendReceive.Send("ironbark-easynet-send-receive-demo", command);
