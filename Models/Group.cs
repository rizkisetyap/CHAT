using System.ComponentModel.DataAnnotations;

namespace Chat.Models
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }

    }
    public class GroupMember
    {
        public Guid Id { get; set; }
        public Group Group { get; set; }
        public Guid GroupId { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public DateTime JoinDate { get; set; }

    }
    public class ChatHistory
    {
        public Guid Id { get; set; }
        public Group Group { get; set; }
        public Guid GroupId { get; set; }
        public GroupMessages Message { get; set; }
        public Guid MessageId { get; set; }
    }
    public class GroupMessages
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public User Sender { get; set; }
        public string SenderId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

    }

}
