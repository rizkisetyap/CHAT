using Chat.Data;
using Chat.Hubs;
using Chat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Controllers
{
    public class GroupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public GroupController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(Guid? Id)
        {
            var user = User.Identity.Name;
            var group = _context.Groups.Where(x => x.Id == Id).FirstOrDefault();
            if (group == null)
            {
                return NotFound();
            }
            var groupMembers = _context.GroupMembers
                .Include(x => x.User)
                .Where(x => x.GroupId == group.Id)
                .Select(x => new
                {
                    x.UserId,
                    x.User.UserName,
                    x.User.Email,
                })
                .ToList();
            bool isMembers = groupMembers.Any(x => x.Email == user);
            if (!isMembers)
            {
                return Forbid();
            }
            var groupMessages = _context.ChatHistory
                .Include(x => x.Message)
                .Include(x => x.Message.Sender)
                .Where(x => x.GroupId == group.Id)
                .Select(x => new
                {
                    Sender = x.Message.Sender.Nama,
                    Date = x.Message.CreateDate,
                    SenderId = x.Message.SenderId,
                    SenderEmail = x.Message.Sender.Email,
                    MessageId = x.MessageId,
                    Message = x.Message.Message,
                })
                .ToList();
            var groupVM = new Dictionary<string, dynamic>();
            groupVM.Add("members", groupMembers);
            groupVM.Add("chats", groupMessages);
            groupVM.Add("groupId", group.Id);

            return View(groupVM);
        }
        public IActionResult ListGroup()
        {
            var groups = _context.Groups.ToList();
            return Ok(groups);
        }
        public IActionResult CreateGroup(string GroupName)
        {
            Group group = new Group() { GroupName = GroupName, CreateDate = DateTime.Now, Description = "-" };
            _context.Add(group);
            _context.SaveChanges();

            return Ok(true);
        }
        [Authorize]
        public async Task<IActionResult> JoinGroup(Guid Id)
        {

            var user = User!.Identity!.Name;
            var isAlreadyMembers = _context.GroupMembers
                .Include(x => x.User)
                .Where(x => x.GroupId == Id)
                .Select(x => new
                {
                    x.User.Email,
                })
                .Any(x => x.Email == user);
            if (isAlreadyMembers)
            {
                return RedirectToAction("Index", "Group", new
                {
                    Id = Id
                });
            }

            var groupMembers = new GroupMember();
            groupMembers.UserId = _context.Users.Where(x => x.Email == user).FirstOrDefault().Id;
            groupMembers.JoinDate = DateTime.Now;
            groupMembers.GroupId = Id;
            _context.Add(groupMembers);
            _context.SaveChanges();
            await _hubContext.Clients.All.SendAsync("AddToGroup", Id.ToString());
            return RedirectToAction("Index", "Group", new
            {
                Id = Id
            });

        }
    }
}
