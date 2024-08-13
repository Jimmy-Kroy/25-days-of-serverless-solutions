using DealSeekerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealSeekerApp.Services
{
    public interface IHtmlGeneratorService
    {
        string GenerateHtmlPage(IEnumerable<DealTableEntity> deals);
    }
}
