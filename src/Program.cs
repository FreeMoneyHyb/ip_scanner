using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Random random = new Random();

        Console.WriteLine("Scanning IP addresses...");

        Console.Write("Enter the first 3 octets of your IP address: ");
        string baseIP = Console.ReadLine();

        int start = 1;
        int end = 255;
        int maxThreads = 100;

        ConcurrentBag<string> onlineIPs = new ConcurrentBag<string>();

        Parallel.For(start, end + 1, new ParallelOptions { MaxDegreeOfParallelism = maxThreads },
            i =>
            {
                int randomIP = random.Next(start, end + 1);
                string ipAddress = baseIP + "." + randomIP.ToString();

                Ping ping = new Ping();
                PingReply reply = ping.Send(ipAddress);

                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine(ipAddress + " is online. Ping time: " + reply.RoundtripTime + "ms");
                    onlineIPs.Add(ipAddress);
                }
                else
                {
                    Console.WriteLine(ipAddress + " is offline.");
                }
            });

        string fileName = "online_ips.txt";
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (string ipAddress in onlineIPs)
            {
                writer.WriteLine(ipAddress);
            }
        }

        Console.WriteLine();
        Console.WriteLine("Scan complete. Results:");
        Console.WriteLine("Online IPs: " + onlineIPs.Count);
        Console.WriteLine("Offline IPs: " + (end - onlineIPs.Count));
        Console.WriteLine("Output written to " + fileName);

        Console.ReadKey();
    }
}