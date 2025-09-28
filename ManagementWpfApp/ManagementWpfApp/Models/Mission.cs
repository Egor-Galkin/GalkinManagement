using System;

namespace ManagementWpfApp.Models
{
    public class Mission
    {
        public int MissionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Completed { get; set; }
        public int AuthorId { get; set; }
        public User Author { get; set; }
        public int? PerformerId { get; set; }
        public User Performer { get; set; }

        public string AuthorNickname => Author?.Nickname ?? string.Empty;

        public string PerformerNickname => Performer?.Nickname ?? "Не назначен";

        public string CompletedStatus => Completed ? "завершено" : "в процессе";

        public string BackColor => Completed ? "#b4d9ff" : "White";

        public string AdminControlsVisibility => (App.CurrentUser != null && App.CurrentUser.RoleId != 1) ? "Visible" : "Collapsed";

        public string UserControlsVisibility => (App.CurrentUser != null && App.CurrentUser.RoleId == 1) ? "Visible" : "Collapsed";

        public string CompleteControlsIsEnabled => Completed ? "false" : "true";
    }
}