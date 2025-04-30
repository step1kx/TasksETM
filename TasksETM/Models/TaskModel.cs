using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksETM.Models
{
    public class TaskModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _taskNumber;
        private string _fromDepart = string.Empty;
        private string _toDepart = string.Empty;
        private bool? _accepted;
        private bool? _taskCompleted;
        private byte[] _screenshotPath;
        private string _taskView = string.Empty;
        private string _taskDescription = string.Empty;
        private string _taskDate = string.Empty;
        private string _taskDeadline;
        private bool? _isAR;
        private bool? _isVK;
        private bool? _isOV;
        private bool? _isSS;
        private bool? _isES;

        public int TaskNumber
        {
            get => _taskNumber;
            set { _taskNumber = value; OnPropertyChanged(nameof(TaskNumber)); }
        }

        public string FromDepart
        {
            get => _fromDepart;
            set { _fromDepart = value; OnPropertyChanged(nameof(FromDepart)); }
        }

        public string ToDepart
        {
            get => _toDepart;
            set { _toDepart = value; OnPropertyChanged(nameof(ToDepart)); }
        }

        public bool? Accepted
        {
            get => _accepted;
            set { _accepted = value; OnPropertyChanged(nameof(Accepted)); }
        }

        public bool? TaskCompleted
        {
            get => _taskCompleted;
            set { _taskCompleted = value; OnPropertyChanged(nameof(TaskCompleted)); }
        }

        public byte[] ScreenshotPath
        {
            get => _screenshotPath;
            set { _screenshotPath = value; OnPropertyChanged(nameof(ScreenshotPath)); }
        }

        public string TaskView
        {
            get => _taskView;
            set { _taskView = value; OnPropertyChanged(nameof(TaskView)); }
        }

        public string TaskDescription
        {
            get => _taskDescription;
            set { _taskDescription = value; OnPropertyChanged(nameof(TaskDescription)); }
        }

        public string TaskDate
        {
            get => _taskDate;
            set { _taskDate = value; OnPropertyChanged(nameof(TaskDate)); }
        }

        public string TaskDeadline
        {
            get => _taskDeadline;
            set { _taskDeadline = value; OnPropertyChanged(nameof(TaskDeadline)); }
        }

        public bool? IsAR
        {
            get => _isAR;
            set { _isAR = value; OnPropertyChanged(nameof(IsAR)); }
        }

        public bool? IsVK
        {
            get => _isVK;
            set { _isVK = value; OnPropertyChanged(nameof(IsVK)); }
        }

        public bool? IsOV
        {
            get => _isOV;
            set { _isOV = value; OnPropertyChanged(nameof(IsOV)); }
        }

        public bool? IsSS
        {
            get => _isSS;
            set { _isSS = value; OnPropertyChanged(nameof(IsSS)); }
        }

        public bool? IsES
        {
            get => _isES;
            set { _isES = value; OnPropertyChanged(nameof(IsES)); }
        }
    }
}
