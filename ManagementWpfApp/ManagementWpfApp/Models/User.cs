using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementWpfApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Nickname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [InverseProperty("Author")]
        public ICollection<Mission> AuthoredMissions { get; set; } = new List<Mission>();

        [InverseProperty("Performer")]
        public ICollection<Mission> AssignedMissions { get; set; } = new List<Mission>();

        public int UsingCount => (AuthoredMissions?.Count ?? 0) + (AssignedMissions?.Count ?? 0);

        public string DeleteControlsIsEnabled
        {
            get
            {
                if (RoleId == 3 || UsingCount > 0)
                    return "false";
                else
                    return "true";
            }
        }

        public string BackColor => RoleId == 3 ? "#b4d9ff" : "White";
    }
}