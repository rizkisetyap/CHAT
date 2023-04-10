using Chat.Data;
using Chat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace Chat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public override Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            var user = _context.Users.Include(x => x.Connections).Where(x => x.Email == name).FirstOrDefault();
            if (user != null)
            {
                var UserConnection = new UserConnection();
                UserConnection.Connected = true;
                UserConnection.ConnectionID = Context.ConnectionId;
                UserConnection.UserAgent = Context.GetHttpContext().Request.Headers["User-Agent"];
                user.Connections.Add(UserConnection);
                _context.SaveChanges();
            }
            return base.OnConnectedAsync();

        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var conn = _context.UserConnections.Where(x => x.ConnectionID == Context.ConnectionId).FirstOrDefault();
            _context.Remove(conn!);
            _context.SaveChanges();
            return base.OnDisconnectedAsync(exception);
        }
        public Task SendMessage(string groupName, string Message)
        {
            var name = Context.User.Identity.Name;

            return Clients.Group(groupName).SendAsync("ReceiveMessage", Message);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}");

        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has Left the group {groupName}");
        }

        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }
    }
}
