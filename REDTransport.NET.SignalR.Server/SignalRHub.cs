using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace REDTransport.NET.SignalR.Server
{
    public class SignalRHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}