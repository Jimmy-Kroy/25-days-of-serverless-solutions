using SpinDreidelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinDreidelApp.Services
{
    internal class DreidelService : IDreidelService
    {
        private readonly Dreidel _dreidel;

        public DreidelService()
        {
            _dreidel = new Dreidel();
        }

        public DreidelSide Spin()
        {
            DreidelSide randomSide = _dreidel.Spin();
            return randomSide;
        }
    }
}
