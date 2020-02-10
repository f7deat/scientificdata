using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Log
    {
        public int LogId { get; set; }
        [StringLength(100)]
        [Display(Name = "Email")]
        public string UserId { get; set; }
        [Display(Name = "Thời gian")]
        public DateTime LogTime { get; set; }
        [Display(Name = "Phân loại")]
        public LogType LogType { get; set; }
        [StringLength(2000)]
        [Display(Name = "Nội dung")]
        public string LogContent { get; set; }
    }
    public enum LogType
    {
        Info,
        Warning,
        Success,
        Error,
        Exception
    }
}
