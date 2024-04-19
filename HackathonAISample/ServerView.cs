using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Views
{
    public class ServerView
    {
        public const string IP = "127.0.0.1";

        public const int ClientServerPort = 5555;
        public const int ReceiverServerPort = 5556;
        public const int ClientServerCallbackPort = 5557;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Server start");

            Console.WriteLine("awaiting response from client");
            string clientResponse = await AwaitStringFromPort(ClientServerPort);

            Console.WriteLine("catched message from client: " + clientResponse);


            string callback = "";

            Console.WriteLine("sending message with prefix to ai bot");
            string aiResponse = await SampleChatCommunication.GetResponseFromAiBotOnMessageAsync(clientResponse);
            
            if (!IsClientRequestValid(aiResponse))
            {
                //debug
                Console.WriteLine(aiResponse);

                callback = "Zle podane zgłoszenie, niezostanie ono wyslane do odbiorcy";
                SendBytesToPort(Encoding.ASCII.GetBytes(callback), ClientServerCallbackPort);
                return;
            }

            string[] scrappedData = ScrapResponse(aiResponse);

            callback = "Dziekujemy za zgloszenie, \n" +
                "Twoje zgloszenie otrzymalo piorytet: " + scrappedData[3] + "\n" +
                "Zajmie sie nim: " + scrappedData[5] + "\n" +
                "Problem zostanie rozwiazany w przeciagu: " + scrappedData[4];

            SendBytesToPort(Encoding.ASCII.GetBytes(callback), ClientServerCallbackPort);

            string sendedMessage = "Te informacje dostanie: " + scrappedData[5] + "\n\n\n" +
                "Wystapil problem z: " + scrappedData[0] + "\n" +
                "Opis: " + scrappedData[1] + "\n" +
                "Piorytet: " + scrappedData[3] + "\n" +
                "Termin wykonania: " + scrappedData[4];

            Console.WriteLine("Sending parsed message to receiver");
            SendBytesToPort(Encoding.ASCII.GetBytes(sendedMessage), ReceiverServerPort);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        public static void SendBytesToPort(byte[] bytesToSend, int port)
        {
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(IP, port);
            sender.Send(bytesToSend);
        }

        public async static Task<string> AwaitStringFromPort(int port)
        {
            Console.WriteLine("Starting to listening to port: " + port);
            byte[] buffer = new byte[1024];
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Server started. Waiting for connections...");
            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(1);

            Socket handler = await listener.AcceptAsync();
            Console.WriteLine("Connection accepted. started to reading bytes");
            int bytesRead = await handler.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

            Console.WriteLine("Received string from port:"+ port);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        static bool IsClientRequestValid(string aiResponse)
        {
            //ugly i know but due to limitation from api i dont have time to make it simpler
            return aiResponse.Contains("1.") || aiResponse.Contains("2.") || aiResponse.Contains("3.") || aiResponse.Contains("4.") || aiResponse.Contains("5.") || aiResponse.Contains("6.");
        }
        static string[] ScrapResponse(string response)
        {
            string[] returnedValue = new string[6];
            int start;
            int end;

            start = response.IndexOf("1.")+2;
            end = response.IndexOf("2.");
            returnedValue[0] = response.Substring(start, end-start);

            start = response.IndexOf("2.")+2;
            end = response.IndexOf("3.");
            returnedValue[1] = response.Substring(start, end-start);

            start = response.IndexOf("3.")+2;
            end = response.IndexOf("4.");
            returnedValue[2] = response.Substring(start, end - start);

            start = response.IndexOf("4.")+2;
            end = response.IndexOf("5.");
            returnedValue[3] = response.Substring(start, end - start);

            start = response.IndexOf("5.")+2;
            end = response.IndexOf("6.");
            returnedValue[4] = response.Substring(start, end - start);

            start = response.IndexOf("6.")+2;
            end = response.Length-1;
            returnedValue[5] = response.Substring(start, end - start);

            return returnedValue;
        }

    }



}