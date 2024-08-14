using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("HttpListener started...");

        // Fetch the public IP address
        string publicIp = await GetPublicIp();

        String[] s3_facts = new String[] {
            "Scale storage resources to meet fluctuating needs with 99.999999999% (11 9s) of data durability.",
            "Store data across Amazon S3 storage classes to reduce costs without upfront investment or hardware refresh cycles.",
            "Protect your data with unmatched security, compliance, and audit capabilities.",
            "Easily manage data at any scale with robust access controls, flexible replication tools, and organization-wide visibility.",
            "Run big data analytics, artificial intelligence (AI), machine learning (ML), and high performance computing (HPC) applications.",
            "Meet Recovery Time Objectives (RTO), Recovery Point Objectives (RPO), and compliance requirements with S3’s robust replication features."
        };

        var listener = new HttpListener();
        listener.Prefixes.Add("http://*:8002/");
        listener.Start();

        try
        {
            while (true)
            {
                HttpListenerContext ctx = listener.GetContext();
                using HttpListenerResponse response = ctx.Response;

                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusDescription = "Status OK";

                Random rnd = new Random();
                int i = rnd.Next(0, s3_facts.Length);

                Console.WriteLine(i);
                Console.WriteLine(s3_facts[i]);

                // Include the public IP in the response
                string responseMessage = $"Public IP: {publicIp}\n" +
                                         $"{DateTime.Now.TimeOfDay} - {s3_facts[i]}";

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseMessage);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.ToString());
        }
        finally
        {
            listener.Close();
        }
    }

    private static async Task<string> GetPublicIp()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("https://api.ipify.org");
                response.EnsureSuccessStatusCode();
                
                string publicIp = await response.Content.ReadAsStringAsync();
                return publicIp;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving public IP: {ex.Message}");
            return "Unavailable";
        }
    }
}
