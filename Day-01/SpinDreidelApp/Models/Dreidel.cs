using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinDreidelApp.Models
{
    internal class Dreidel
    {
        const int TotalDreidelSides = 4;
        const int NumberOfDreidelSpins = 1;
        private readonly DreidelSide[] _dreidelSides;

        public Dreidel()
        {
            _dreidelSides = new DreidelSide[TotalDreidelSides];

            /* Initialize DreidelSides */
            _dreidelSides[0] = new DreidelSide('נ', "Nun", "Do nothing.", "");
            _dreidelSides[1] = new DreidelSide('ג', "Gimmel", "Take everything in the pot.", "");
            _dreidelSides[2] = new DreidelSide('ה', "Hay", "Take half of the pot.", "");
            _dreidelSides[3] = new DreidelSide('ש', "Shin", "Add to the pot.", "");
        }

        public DreidelSide Spin()
        {
            DreidelSide randomSide = Random.Shared.GetItems(_dreidelSides, NumberOfDreidelSpins).First();
            return randomSide;
        }
    }
}
