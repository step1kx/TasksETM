using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksETM.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<string>> GetAllProjectNamesAsync();


        public Task<bool> GetNotifyStatusByProjectNameAndUserLogin(string projectName, string userLogin);

        public Task UpdateNotifyStatusForUserAndProject(string projectName, string userLogin, bool isNotify);

    }
}
