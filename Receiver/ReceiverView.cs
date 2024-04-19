using Server.Views;

namespace Receiver.Views
{
    class ReceiverView
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Receiver start");

            string receivedString = await ServerView.AwaitStringFromPort(ServerView.ReceiverServerPort);
            Console.WriteLine("Received string!");

            Console.WriteLine(receivedString);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}