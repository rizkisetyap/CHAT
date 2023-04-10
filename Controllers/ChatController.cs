using Chat.Data;
using Chat.Hubs;
using Chat.Models;
using Chat.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;


namespace Chat.ViewModels
{
    public class ChatVM
    {
        public string Sender { get; set; }
        public DateTime Date { get; set; }
        public string SenderId { get; set; }
        public string SenderEmail { get; set; }
        public Guid MessageId { get; set; }
        public string Message { get; set; }
    }
}
namespace Chat.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IConfiguration _config;
        public ChatController(ApplicationDbContext context, IHubContext<ChatHub> hubContext, IConfiguration config)
        {
            _context = context;
            _hubContext = hubContext;
            _config = config;
        }
        [HttpPost]
        [Authorize]
        public IActionResult SendGroupMessage(string Message, Guid GroupId)
        {
            var user = _context.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            GroupMessages msg = new GroupMessages();
            msg.SenderId = user.Id;
            msg.Message = Message;
            msg.CreateDate = DateTime.Now;
            _context.Add(msg);
            _context.SaveChanges();
            ChatHistory chatHistory = new ChatHistory();
            chatHistory.MessageId = msg.Id;
            chatHistory.GroupId = GroupId;
            _context.Add(chatHistory);
            var count = _context.SaveChanges();

            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var sql = "select U.Nama as Sender, M.CreateDate as Date, M.SenderId as SenderId, " +
                    "U.Email as SenderEmail, M.Id, M.Message from ChatHistory CH " +
                    "join Groups G on G.Id = CH.GroupId " +
                    "join GroupMessages M on M.Id = CH.MessageId " +
                    "join AspNetUsers U on U.Id = M.SenderId " +
                    "Where G.Id = @GroupId";
                var msgs = conn.Query<ChatVM>(sql, new { GroupId = GroupId });
                _hubContext.Clients.Group(GroupId.ToString()).SendAsync("ReceiveMessage", msgs);
            }

            return Ok(true);
        }
        [Authorize]
        public IActionResult GetGroupChatMessage(Guid GroupId)
        {
            void _dependencyOnChange(object sender, SqlNotificationEventArgs e)
            {
                if (e.Type == SqlNotificationType.Change)
                {
                    _hubContext.Clients.Group(GroupId.ToString()).SendAsync("ReceiveMessage");
                }
            }
            var user = _context.Users.Where(x => x.Email == User.Identity!.Name).FirstOrDefault();
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var sql = "select U.Nama as Sender, M.CreateDate as Date, M.SenderId as SenderId, " +
                    "U.Email as SenderEmail, M.Id as MessageId, M.Message from ChatHistory CH " +
                    "join Groups G on G.Id = CH.GroupId " +
                    "join GroupMessages M on M.Id = CH.MessageId " +
                    "join AspNetUsers U on U.Id = M.SenderId " +
                    "Where G.Id = @GroupId";
                var cmd = new SqlCommand(sql, conn);
                var msgs = conn.Query<ChatVM>(sql, new { GroupId = GroupId });
                SqlDependency sqlDependency = new SqlDependency(cmd);
                sqlDependency.OnChange += new OnChangeEventHandler(_dependencyOnChange);
                return Ok(msgs);
            }
        }
        //private void _dependencyOnChange(object sender, SqlNotificationEventArgs e)
        //{
        //    if (e.Type == SqlNotificationType.Change)
        //    {
        //        _hubContext.Clients.All.SendAsync("ReceiveMessage");
        //    }
        //}


    }
}
