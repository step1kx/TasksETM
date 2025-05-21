using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notify.Models;

namespace Notify.Interfaces
{
    public interface ITaskService
    {
        public Task<List<TaskModel>> GetTasksByUserAsync(string departmentName);

        public Task<Dictionary<string, bool>> GetNotifyStatusFromProjectsAsync();
    }
}
