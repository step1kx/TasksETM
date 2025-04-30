using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksETM.Interfaces.ITasks
{
    public interface IFilterTasksService
    {
        public Task SaveFilterSettingsAsync();

        public Task LoadFilterSettingsAsync();
    }
}
