using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksETM.Models
{
    public class NotifyModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isNotify;



        public bool IsNotify
        {
            get => _isNotify;
            set
            {
                _isNotify = value;
                OnPropertyChanged(nameof(IsNotify)); // если используешь INotifyPropertyChanged
            }
        }

        
    }
}
