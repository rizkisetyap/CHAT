using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Chat.Models
{
    public class User : IdentityUser
    {
        public string Nama { get; set; }
        public ICollection<UserConnection> Connections { get; set; }
        public ICollection<Group> Groups { get; set; }
    }

    public class UserConnection
    {
        [Key]
        public string ConnectionID { get; set; }
        public string UserAgent { get; set; }
        public bool Connected { get; set; }
        public DateTime ConnectedDate { get; set; } = DateTime.Now;
    }
}
