using EasyNetQ;
using Messages.Commands;

var AMQP = "amqps://xwugqrcr:VJHyIwkBhFsn8s5OBkGRkXD1ZGjcqyiJ@diligent-wheat-koala.rmq4.cloudamqp.com/xwugqrcr";
var bus = RabbitHutch.CreateBus(AMQP);

bus.SendReceive.Receive<DisplayNotification>("ironbark-easynet-send-receive-demo", 
    command => {
        Console.WriteLine($"Received: {command.Content}");
        Thread.Sleep(TimeSpan.FromSeconds(2));
    }
);

Console.WriteLine("Receiving messages; press Enter to quit");
Console.ReadLine();
