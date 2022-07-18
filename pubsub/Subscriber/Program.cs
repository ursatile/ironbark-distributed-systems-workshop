using EasyNetQ;
using Messages.Commands;
using Messages.Events;

var AMQP = "amqps://xwugqrcr:VJHyIwkBhFsn8s5OBkGRkXD1ZGjcqyiJ@diligent-wheat-koala.rmq4.cloudamqp.com/xwugqrcr";
var bus = RabbitHutch.CreateBus(AMQP);
bus.PubSub.Subscribe<KeyWasPressed>("SUBSCRIBER", evt => {
    Console.WriteLine(evt.Key);
});

// bus.SendReceive.Receive<DisplayNotification>("ironbark-easynet-send-receive-demo", 
//     command => {
//         Console.WriteLine($"Received: {command.Content}");
//         Thread.Sleep(TimeSpan.FromSeconds(2));
//     }
// );

Console.WriteLine("Subscribed for KeyWasPressed events; press Enter to quit");
Console.ReadLine();
