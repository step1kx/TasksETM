using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksETM.Models
{
    public class TaskModel
    {
        public int TaskNumber { get; set; }
        public string FromSection { get; set; } = string.Empty;
        public string ToSection { get; set; } = string.Empty;
        public bool? Accepted { get; set; } 
        public bool TaskCompleted { get; set; }
        public string ScreenshotPath { get; set; } = string.Empty;
        public string TaskView { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public DateTime TaskDate { get; set; }
        public DateTime TaskDeadline { get; set; }
    }
}
