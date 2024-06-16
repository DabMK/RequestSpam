namespace RequestSpam
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Dictionary<string, string> auth = new()
            {
                { "PHPSESSID", "" }
            };

            // LEVEL PASSWORD
            string? pwd = null;
            while (string.IsNullOrWhiteSpace(pwd))
            {
                Console.Write("\nPassword of level: ");
                pwd = Console.ReadLine();
            }

            // PHP SESSION ID
            string? ID = null;
            while (string.IsNullOrWhiteSpace(ID))
            {
                Console.Write("PHP Session ID: ");
                ID = Console.ReadLine();
            }
            auth["PHPSESSID"] = ID;

            // DELAY
            string? delayStr = string.Empty; int delay = 1000;
            Console.Write("Delay between requests in ms (default is 1000): ");
            delayStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(delayStr) && int.TryParse(delayStr, out _))
            {
                delay = int.Parse(delayStr);
            }

            // OUTPUT
            string? output = string.Empty; bool records = false;
            Console.Write("File where to output records (leave empty for no file output): ");
            output = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(output) && Directory.Exists(Path.GetDirectoryName(output)))
            {
                if (File.Exists(output))
                {
                    Console.Write($"- The file \"{output}\" already exists. Do you want to override the content (Y/N)?");
                    char selection = Console.ReadKey().KeyChar;
                    if (selection == 'y')
                    {
                        File.WriteAllText(output, string.Empty);
                        Console.Write($"- The file \"{output}\" will be overwritten");
                        records = true;
                    }
                    else
                    {
                        Console.Write("- No file output will be generated");
                        records = false;
                    }
                }
                else
                {
                    records = true;
                }
            }
            else
            {
                Console.Write("- The directory doesn't exist. No file output will be generated");
            }

            Console.WriteLine(Environment.NewLine);
            int attempt = 1;
            while (true)
            {
                HttpResponseMessage? response = Request.Get($"https://ae27ff.com/{pwd}.php", null, auth).GetAwaiter().GetResult();
                string content;
                if (response == null)
                {
                    content = "Error";
                }
                else
                {
                    content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if ((!content.Contains("<pre>") || !content.Contains("</pre>")) || content.Contains("couldn't be found"))
                    {
                        content = "Not Authenticated";
                    }
                    else
                    {
                        content = content.Split("<pre>")[1].Split("</pre>")[0];
                        content = content.Substring(2, content.Length - 2);
                    }
                }

                string record = $"ATTEMPT {attempt}, UTC TIME: {DateTime.UtcNow.ToString().Split(' ')[1]}{Environment.NewLine}{content}";
                Console.WriteLine(record);
                if (records)
                {
                    File.AppendAllText(output, record + Environment.NewLine);
                }
                Thread.Sleep(delay);
                attempt++;
            }
        }
    }
}