using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasCardApp.Services
{
    public interface IChristmasCardService
    {
        string GetChristmasCard(string markdownContent);
    }
}
