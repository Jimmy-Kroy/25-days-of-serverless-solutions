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
            _dreidelSides[0] = new DreidelSide('נ', "Nun", "Do nothing.", 
                "https://github.com/Jimmy-Kroy/25-days-of-serverless-solutions/blob/master/Day-01/Img/dreidel-NUN-side.jpg");
            _dreidelSides[1] = new DreidelSide('ג', "Gimmel", "Take everything in the pot.", 
                "https://github.com/Jimmy-Kroy/25-days-of-serverless-solutions/blob/master/Day-01/Img/dreidel-GIMMEL-side.jpg");
            _dreidelSides[2] = new DreidelSide('ה', "Hay", "Take half of the pot.", 
                "https://github.com/Jimmy-Kroy/25-days-of-serverless-solutions/blob/master/Day-01/Img/dreidel-HEY-side.jpg");
            _dreidelSides[3] = new DreidelSide('ש', "Shin", "Add to the pot.", 
                "https://github.com/Jimmy-Kroy/25-days-of-serverless-solutions/blob/master/Day-01/Img/dreidel-SHIN-side.jpg");
        }

        public DreidelSide Spin()
        {
            DreidelSide randomSide = _dreidelSides[Random.Shared.Next(0, _dreidelSides.Count())];
            return randomSide;
        }
    }
}
