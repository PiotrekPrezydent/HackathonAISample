using Server.Views;
using System.Text;

namespace Client.Views
{
    class ClientView
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Napisz zgłosznie:");
            string textToSend = Console.ReadLine();

            byte[] bytesToSend = Encoding.ASCII.GetBytes(textToSend);

            ServerView.SendBytesToPort(bytesToSend, ServerView.ClientServerPort);
            Console.WriteLine("Awaiting callback from server");

            string callback = await ServerView.AwaitStringFromPort(ServerView.ClientServerCallbackPort);
            Console.WriteLine("Catched callback from server:\n" + callback);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}