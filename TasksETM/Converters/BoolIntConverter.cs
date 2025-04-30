using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksETM.Converters
{
    public static class BoolIntConverter
    {
        public static int ToInt(bool value) => value ? 1 : 0;
        public static bool ToBool(int value) => value == 1;
    }
}
