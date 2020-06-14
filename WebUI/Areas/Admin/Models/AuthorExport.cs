using System;

namespace WebUI.Areas.Admin.Models
{
    public class AuthorExport
    {
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string Social { get; set; }
        public string Avatar { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string AcademicRank { get; set; }
        public string Degree { get; set; }
        public string Deparment { get; set; }
        public string Specialized { get; set; }
        public int TotalTopic { get; set; }
    }
}
