using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCB.Models
{
    public class ElectionTurnoutModel
    {
        public string Identifier { get; set; }
        public string RegionName { get; set; }
        public string Year { get; set; }
        public string Turnout { get; set; }
    }
}
