using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SpinDreidelApp.Models
{
    public class DreidelSide
    {
        public DreidelSide(char symbol, string name, string instructions, string image_Url)
        {
            TimeCreated = DateTime.Now;
            Symbol = symbol;
            Name = name;
            Instructions = instructions;
            Image_Url = image_Url;
        }

        public DateTime TimeCreated { get; set; }

        public char Symbol { get; private set; }

        public string Name { get; private set; }

        public string Instructions { get; private set; }

        public string Image_Url { get; private set; }
    }
}
