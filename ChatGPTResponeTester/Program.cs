using Azure.AI.OpenAI;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Headers;


namespace ChatTester
{
    class ChatTesterView
    {
        static async Task Main(string[] args)
        {
            string pre = "Na podstawie podanego przezemnie symulacyjnego zgłoszenia najpierw upewnij się najpierw czy to zgłoszenie jest poprawnym zgłoszeniem, jeżeli nie napisz poprostu Błędne zgłoszenie, jeżeli jest poprawne podaj kolejno w nastepnych punktach: " +
                "1. Z czym konkretnie jest problem (np. Ulica,Wodociąg,Spokój mieszkańców), " +
                "2. Co się stało (krótki opis dla słóżb miejskich co się stało), " +
                "3. Jak wpływa to na mieszkańców (krótki opis jak wpływa ten problem na innych mieszkańców), " +
                "4. Jak ważne jest rozwiązanie problemu (krótki opis piorytetu np pilne, bardzo pilne, malo pilne w jednym słowie), " +
                "5. Do kiedy problem powinien zostać rozwiązany (Konkretna data / ilość czasu badz informacja ze natychmiast w jednym słowie), " +
                "6. Kto powinien rozwiązać ten problem (Jaki organ służb miejskich powinien się zająć tym problemem w jedyn słowie) " +
                "Oto przykladowe podanie przezemnie zgłoszenie: ";

            string zapytanie = Console.ReadLine();

            string conversationJson = @"
            {
                ""conversation"": 
                [
                    {
                        ""role"": ""user"",
                        ""content"": ""{0}""
                    }
                ]
            }";

            conversationJson = conversationJson.Replace("{0}", pre+zapytanie);
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://texts-magic-api.p.rapidapi.com/chat"),
                Headers =
                {
                    { "X-RapidAPI-Key", "caba6da6d3mshbfa0cbfc5a27b56p1d6f8ajsn6d16064dae12" },
                    { "X-RapidAPI-Host", "texts-magic-api.p.rapidapi.com" },
                },
                Content = new StringContent(conversationJson)
                {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json")
                        }
                }
            };

            Console.WriteLine("send");

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            Console.WriteLine("await response");
            var body = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(body);
            Console.WriteLine("parse");
            var test = jsonResponse["output"]["content"];

            string output = "";

            foreach ( var item in test)
            {
                output += item.Type + " --- " + item.ToString() + " \n";
                output += "\n\n\n";
            }
            //var assistantmessagecontent = jsonResponse["output"][0]["content"].Value<string>();


            Console.WriteLine(test);


            Console.ReadKey();

        }
    }
}