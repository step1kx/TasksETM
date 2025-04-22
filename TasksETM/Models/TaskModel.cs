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
        public string FromDepart { get; set; } = string.Empty;
        public string ToDepart { get; set; } = string.Empty;
        public bool? Accepted { get; set; }
        public bool TaskCompleted { get; set; }
        public byte [] ScreenshotPath { get; set; } 
        public string TaskView { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string TaskDate { get; set; } = string.Empty;
        public string TaskDeadline { get; set; }
    }
}
