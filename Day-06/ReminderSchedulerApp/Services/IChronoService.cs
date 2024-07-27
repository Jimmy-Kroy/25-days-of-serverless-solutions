using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderSchedulerApp.Services
{
    public interface IChronoService
    {
        Task<DateTime?> ExtractTimeStamp(string text, string timezone);
    }
}
