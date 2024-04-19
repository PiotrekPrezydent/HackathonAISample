using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal static class SampleChatCommunication
    {
        const string MessagePrefix = "Na podstawie podanego przezemnie symulacyjnego zgłoszenia podanego jako mieszkaniec sredniego miasta, podaj kolejno w nastepnych punktach: " +
                "1. Z czym konkretnie jest problem (np. Ulica,Wodociąg,Spokój mieszkańców), " +
                "2. Co sie stalo (krótki opis dla słóżb miejskich co się stało), " +
                "3. Jak wpływa to na mieszkańców (krótki opis jak wpływa ten problem na innych mieszkańców), " +
                "4. Jak ważne jest rozwiązanie problemu (krótki opis piorytetu np pilne, bardzo pilne, malo pilne w jednym słowie), " +
                "5. Do kiedy problem powinien zostać rozwiązany (Konkretna data / ilość czasu badz informacja ze natychmiast w jednym słowie), " +
                "6. Kto powinien rozwiązać ten problem (Jaki organ służb miejskich powinien się zająć tym problemem w jedyn słowie) " +
                "(podaj poprostu już głowne odpowiedzi kolejno 1. odpowiedz, 2. odpowiedz itd)" +
                "Oto przykladowe podanie przezemnie zgłoszenie: ";

        internal static async Task<string> GetResponseFromAiBotOnMessageAsync(string message)
        {
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
            conversationJson = conversationJson.Replace("{0}", MessagePrefix + message);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://texts-magic-api.p.rapidapi.com/chat"),

                //this sample api can only response 30 times, use this site to create new key if needed https://rapidapi.com/LuSrodri/api/texts-magic-api -> create organization -> use new organization -> get new key
                //Not used keys:
                //2172f9c88emsh1f2dc19b5827a5ap1f361ajsnd8fb95b0ee3b
                //993b37ec7dmsh2060ff5d80f308fp13820djsn1e947185b1ab
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

            var client = new HttpClient();
            HttpResponseMessage response;
            //send exception handle
            try
            {
                Console.WriteLine("Trying to send message to ai bot");
                response = await client.SendAsync(request);
                Console.WriteLine("Succes!");
            }catch (Exception ex)
            {
                Console.WriteLine("There was an error while trying to send message to ai bot: " + ex.Message);
                Console.ReadKey();
                return null;
            }

            string body = "";
            //response exception handle
            try
            {
                Console.WriteLine("Trying to receive message from ai bot");
                body = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Succes!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error while trying to receive message from ai bot: " + ex.Message);
                Console.ReadKey();
            }

            var jsonResponse = JObject.Parse(body);
            return jsonResponse["output"]["content"].ToString();
        }
    }
}
