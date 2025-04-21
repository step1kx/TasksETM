using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksETM.Models;

namespace TasksETM.Interfaces.ITasks
{
    public interface ITaskService
    {
        public Task<DataTable> GetTasksByProjectAsync(string projectName);
    }
}
