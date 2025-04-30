using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksETM.Interfaces.ITasks;

namespace TasksETM.Service.Tasks
{
    public class FilterTasksService : IFilterTasksService
    {
        public async Task LoadFilterSettingsAsync()
        {
            Properties.Settings.Default.Reload();
            await Task.CompletedTask;
        }

        public async Task SaveFilterSettingsAsync()
        {
            Properties.Settings.Default.Save();
            await Task.CompletedTask;
        }
    }
}
