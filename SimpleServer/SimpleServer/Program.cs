using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer
{
    class Program
    {
        static string GetHttpResponse(string statusCode, byte[] content)
        {
            return $"HTTP/1.1 {statusCode}\r\n" +
                   "Server: PMFST\r\n" +
                   $"Content-Length: {content.Length}\r\n" +
                   "Content-Type: text/html\r\n" +
                   "Connection: Closed\r\n" +
                   "\r\n";
        }
        
        static void Main(string[] args)
        {
            var port = 9000;
            var address = IPAddress.Loopback;
            var startPath = "Files/";

            Console.WriteLine("Starting up...");

            TcpListener tcpListener = new TcpListener(address, port);
            tcpListener.Start();

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                string msg = "";

                using (var stream = tcpClient.GetStream())
                {
                    var reader = new StreamReader(stream);

                    string line = "";

                    while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                    {
                        msg = $"{msg}{line}\n";
                    }
                    
                    Console.WriteLine(msg);

                    string[] lines = msg.Split('\n');
                    string requestLine = lines[0];
                    string url = requestLine.Split(' ')[1];
                    
                    Console.WriteLine($"PUTANJA: {url}");
                    
                    if (url == "/")
                    {
                        var content = File.ReadAllBytes($"{startPath}kostur.html");
                        var httpResponse = GetHttpResponse("200 OK", content);

                        var response = Encoding.ASCII.GetBytes(httpResponse);
                        
                        stream.Write(response);
                        stream.Write(content);
                    }
                    else if (url == "/favicon.ico")
                    {
                        var httpResponse = GetHttpResponse("404 NOT FOUND", new byte[0]);
                        var response = Encoding.ASCII.GetBytes(httpResponse);
                        
                        stream.Write(response);
                    }
                }
                
                tcpClient.Close();
            }

            tcpListener.Stop();

            Console.WriteLine("Shutting down...");
        }
    }
}