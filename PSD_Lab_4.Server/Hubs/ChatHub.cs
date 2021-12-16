using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PSD_Lab_4.Server.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSD_Lab_4.Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            this.logger = logger;
        }

        public static List<string> Users = new();

        public static int Q = Generator.GenerateRandomQ(1000, 10000);
        public static int A = Generator.GenerateA(Q);
        public static int Depth = 1;

        public async Task GetChatParams()
        {            
            await Clients.Caller.SendAsync("GetChatParams", A, Q);
        }
        
        public async Task Connect()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("GetChatParams", A, Q);

            //add to users
            if (!Users.Contains(Context.ConnectionId))
            {
                Users.Add(Context.ConnectionId);
            }

            //Call 'reset' on all clients
            if (Users.Count > 1)
            {
                if (Users.Count > 2)
                {
                    await Clients.All.SendAsync("Reset");
                    logger.LogWarning($"===========================");

                    Q = Generator.GenerateRandomQ(1000, 10000);
                    A = Generator.GenerateA(Q);

                    await Clients.All.SendAsync("GetChatParams", A, Q);

                    Depth = Users.Count - 1;
                }

                for (int i = 0; i < Users.Count; i++)
                {
                    await Clients.Client(Users[i]).SendAsync("ReceiveKey", 0, 0);
                }                
            }            
        }

        public async Task SendKey(int key, int depth)
        {
            var curUser = Context.ConnectionId;

            var i = Users.IndexOf(curUser);

            var receiverId = (i + 1) % Users.Count;

            var receiver = Users[receiverId];

            logger.LogInformation($"{i}: {Context.ConnectionId} -> {receiverId}: {receiver} Depth: {depth} Key: {key}");            

            if (depth == Depth)
            {
                await Clients.Client(receiver).SendAsync("ReceiveFinalKey", key);
            } else
            {
                await Clients.Client(receiver).SendAsync("ReceiveKey", key, depth);
            }
        }

        public async Task Send(string user, string message)
        {
            if (!Users.Contains(Context.ConnectionId))
            {
                Users.Add(Context.ConnectionId);
            }

            await Clients.Others.SendAsync("Send", user, message);
        }
    }
}
