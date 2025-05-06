using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksETM.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string departmentName, string password);

        Task<bool> CheckSavedLoginAsync(string login);
    }
}
