using EasyNetQ;
using Messages.Commands;

var AMQP = "amqps://xwugqrcr:VJHyIwkBhFsn8s5OBkGRkXD1ZGjcqyiJ@diligent-wheat-koala.rmq4.cloudamqp.com/xwugqrcr";

var bus = RabbitHutch.CreateBus(AMQP);

Console.WriteLine("Press any key to send a command:");
while (true)
{
    Console.ReadKey(true);
    var command = new DisplayNotification
    {
        Content = $"Hello World from {Environment.MachineName} at {DateTime.UtcNow}"
    };
    bus.SendReceive.Send("ironbark-easynet-send-receive-demo", command);
    Console.WriteLine($"Sent: {command.Content}");
}
