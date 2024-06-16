using System;
using System.ComponentModel;
using System.Net;
using System.Security.Authentication;
using System.Text;
using RequestSpam;

namespace RequestSpam
{
    internal class Request
    {
        // SEND A "GET" WEB REQUEST
        public static async Task<HttpResponseMessage?> Get(string? URI, Dictionary<string, string>? headers = null, Dictionary<string, string>? cookies = null)
        {
            HttpClient client;
            HttpResponseMessage response = new();
            HttpClientHandler handler = new();
            CookieContainer container = new();
            if (string.IsNullOrWhiteSpace(URI)) { return new(); }

            // FORGING REQUEST
            using (HttpRequestMessage request = new(new("GET"), URI))
            {
                // HEADERS
                if (headers != null)
                {
                    for (int i = 0; i < headers.Count; i++)
                    {
                        request.Headers.Add(headers.Keys.ToList()[i], headers.Values.ToList()[i]);
                    }
                }

                // COOKIES
                if (cookies != null)
                {
                    handler.UseCookies = true;
                    for (int i = 0; i < cookies.Count; i++)
                    {
                        container.Add(new Uri(URI), new Cookie(cookies.ElementAt(i).Key, cookies.ElementAt(i).Value));
                    }
                }
                else
                {
                    handler.UseCookies = false;
                }

                // SEND
                handler.CookieContainer = container;
                client = new(handler, true);
                try
                {
                    response = await client.SendAsync(request);
                }
                catch (Win32Exception e)
                {
                    Console.Write(e.Message);
                    return null;
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    return null;
                }
            }
            return response;
        }
    }
}