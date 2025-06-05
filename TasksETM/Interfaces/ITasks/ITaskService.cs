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
        public Task<List<TaskModel>> GetTasksByProjectAsync(string projectName);

        public Task UpdateTaskAssignmentsAsync(int taskNumber, bool isAR, bool isVK, bool isOV, bool isSS, bool isES, bool IsGIP);

        //public Task UpdateTaskCompletedAsync(int taskNumber, bool isCompleted);

        public Task UpdateTaskCompletedAsync(int taskNumber, bool isAR, bool isVK, bool isOV, bool isSS, bool isES, bool isGIP);

        public Task UpdateTaskCommentAsync(int taskNumber, string comment);
    }
}
