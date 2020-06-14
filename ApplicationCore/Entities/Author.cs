using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class Author
    {
        public int AuthorId { get; set; }
        [StringLength(200)]
        [Display(Name = "Họ và Tên")]
        public string Name { get; set; }
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Giới tính")]
        public bool? Gender { get; set; }
        public int? DepartmentId { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(200)]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Tiểu sử")]
        public string Bio { get; set; }
        [StringLength(500)]
        [Display(Name = "Địa chỉ")]
        public string Location { get; set; }
        [StringLength(1000)]
        [Display(Name = "Website/Blog")]
        public string Social { get; set; }
        [StringLength(1000)]
        [Display(Name = "Ảnh đại diện")]
        public string Avatar { get; set; }
        [StringLength(200)]
        public string UserId { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Ngày sửa đổi")]
        public DateTime ModifiedDate { get; set; }
        [Display(Name = "Học hàm")]
        public string AcademicRank { get; set; }
        [Display(Name = "Học vị")]
        public string Degree { get; set; }
        [Display(Name = "Đơn vị công tác")]
        public string Deparment { get; set; }
        [Display(Name = "Chuyên nghành")]
        public string Specialized { get; set; }
        public int? Order { get; set; }
        public int Status { get; set; }

        public virtual Department Department { get; set; }

        public virtual ICollection<AuthorTopic> AuthorTopics { get; set; }
    }
}
