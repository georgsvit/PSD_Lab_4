using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Numerics;

namespace PSD_Lab_4.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var username = Console.ReadLine();
            int A = -1;
            int Q = -1;

            int PrivateKey = -1;
            int SharedKey = -1;
            int Delta = -1;

            var connection = new HubConnectionBuilder().WithUrl("https://localhost:5001/chat").Build();

            connection.StartAsync().Wait();
            connection.InvokeAsync("Connect");

            Console.WriteLine(connection.ConnectionId);

            connection.On<string, string>("Send", async (user, text) =>
            {
                string decodedText = EncoderDecoder.Decode(text, Delta);
                Console.ForegroundColor = ConsoleColor.Yellow;
                PrintChatMessage(user, decodedText);
                Console.ForegroundColor = ConsoleColor.White;
                PrintSystemInfo($"Encoded text: {text}");
            });

            connection.On("Reset", async () =>
            {
                Console.Clear();
                Console.WriteLine(username);
                Console.WriteLine(connection.ConnectionId);
            });

            connection.On<int, int>("GetChatParams", async (a, q) =>
            {
                A = a;
                Q = q;

                PrintSystemInfo("A & Q received");
                PrintSystemInfo($"A: {A} Q: {Q}");

                PrivateKey = new Random().Next(1, Q - 1);
            });

            connection.On<int, int>("ReceiveKey", async (key, depth) =>
            {
                PrintSystemInfo($"Key received: {key}");

                var tempKey = BigInteger.Zero;

                //form key
                if (key == 0)
                {
                    tempKey = BigInteger.ModPow(A, PrivateKey, Q);
                }
                else
                {
                    tempKey = BigInteger.ModPow(key, PrivateKey, Q);
                }

                PrintSystemInfo($"Calculated key: {tempKey}");

                key = (int)tempKey;

                //send it
                depth++;

                await connection.InvokeAsync("SendKey", key, depth);
            });

            connection.On<int>("ReceiveFinalKey", async (key) =>
            {
                PrintSystemInfo($"Key received: {key}");

                var tempKey = BigInteger.ModPow(key, PrivateKey, Q);

                SharedKey = (int)tempKey;
                Delta = SharedKey % 26;

                PrintSystemInfo($"Final key: {SharedKey}");
            });

            //ReceiveFinalPair(depth) => SendKey(key, depth++) => ReceiveFinalKey(key)

            //ReceiveKey(key, depth) => SendKey(key, depth++) calc receiver => ReceiveKey(key) => SendKey(key)

            while (true)
            {
                string message = Console.ReadLine();

                string encodedText = EncoderDecoder.Encode(message, Delta);
                PrintSystemInfo($"Encoded text: {encodedText}");

                connection.InvokeAsync("Send", username, encodedText);
            }

            Console.ReadLine();
        }

        private static void PrintSystemInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"[System]: {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void PrintChatMessage(string user, string message)
        {
            Console.WriteLine($"[{user}]: {message}");
        }
    }
}
