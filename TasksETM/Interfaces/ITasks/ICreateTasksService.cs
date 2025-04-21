using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksETM.Models;

namespace TasksETM.Interfaces.ITasks
{
    public interface ICreateTasksService
    {
        Task CreateTaskAsync(TaskModel taskModel);
    }
}
