using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Net.Http.Json;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Emit;

namespace SecurityFootageTester
{
    public class Program
    {
        private static readonly string Url = "http://localhost:5291/footageaccess/retrieve";
        private static readonly string HeaderName = "NickName";
        private static readonly string RequestBody = """
        {
            "Country" : "Texas",
            "City" : "Bay Street Park"
        }
        """;

        private static readonly Random untilStart = new();

        public static async Task Main(string[] args)
        {
            
            Console.WriteLine("Welcome to security footage tester for footage demand.");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                DateTime start = GetTime();
                while (true)
                {
                    DateTime current = GetTime();
                    TimeSpan diff = current - start;

                    if ((int)diff.TotalSeconds >= untilStart.Next(3, 11))
                    {
                        break;
                    }
                }
                int decideUser = untilStart.Next(0, 11);

                if (decideUser <= 2)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Jeca demands footage..\n");

                    HttpResponseMessage response = await CallFootage(RequestBody, "Jeca");

                    if (response.IsSuccessStatusCode)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Accept!");
                        Console.WriteLine($"Jeca get footage on following location : {FormatStringForConsole(RequestBody)}\n\n");
                    }
                    else
                    {
                        HandleBadCodes(response, "Jeca");
                    }
                }
                else if (decideUser > 2 && decideUser <= 6)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Drki demands footage..");

                    HttpResponseMessage response = await CallFootage(RequestBody, "Drki");

                    if (response.IsSuccessStatusCode)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Accept!");
                        Console.WriteLine($"Drki get footage on following location : {FormatStringForConsole(RequestBody)}\n\n");
                    }
                    else
                    {                      
                        HandleBadCodes(response, "Drki");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Dzoni demands footage...");

                    HttpResponseMessage response = await CallFootage(RequestBody, "Dzoni");

                    if (response.IsSuccessStatusCode)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Accept!");
                        Console.WriteLine($"Dzoni get footage on following location : {FormatStringForConsole(RequestBody)}\n\n");
                    }
                    else
                    {
                        HandleBadCodes(response, "Dzoni");
                    }
                }

            }


        }

        public static string FormatStringForConsole(string str)
        {
            return str.Replace("/r", "").Replace("/n", " ").Trim();
        }
        
        public static HttpStatusCode HandleBadCodes(HttpResponseMessage response, string nickname)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Forbidden:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"You do not have access to this footage {nickname}!\n\n");
                    return HttpStatusCode.Forbidden;
                case HttpStatusCode.InternalServerError:
                    Console.WriteLine("Server error.");
                    return HttpStatusCode.InternalServerError;
                case HttpStatusCode.ServiceUnavailable:
                    Console.WriteLine("Service unavailable");
                    return HttpStatusCode.ServiceUnavailable;
                case HttpStatusCode.RequestTimeout:
                    Console.WriteLine("Timeout...");
                    return HttpStatusCode.RequestTimeout;
                default:
                    Console.WriteLine($"{response.StatusCode}");
                    return response.StatusCode;
            }


        }

        public static DateTime GetTime()
        {
            return DateTime.Now;
        }

        public static async Task<HttpResponseMessage> CallFootage(string? requestBody, string nickname, CancellationToken ct = default)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, Url);

                    request.Headers.Add(HeaderName, nickname);
                    request.Content = new StringContent(RequestBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.SendAsync(request, ct);


                    return response;
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Network error..");
                var msg = new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable);
                return msg;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Timeout...");
                var msg = new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout);
                return msg;
            }

        }

    }
}
