using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksETM.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<string>> GetDepartmentNamesAsync();

        Task<IEnumerable<string>> GetTaskStatusAsync();
    }
}
