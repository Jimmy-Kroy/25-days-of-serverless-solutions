using ReminderSchedulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderSchedulerApp.Services
{
    public interface ISlackClient
    {
        Task<bool> PostAsync(Reminder reminder);
    }
}
