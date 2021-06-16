using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCB.Models
{
    public class ScbElectionTurnoutStatisticRequestModel
    {
        public List<Query> Query { get; set; }
        public Response Response { get; set; }
    }

    public class Query
    {
        public string Code { get; set; }
        public Selection Selection { get; set; }
    }

    public class Selection
    {
        public string Filter { get; set; }
        public List<string> Values { get; set; }
    }

    public class Response
    {
        public string Format { get; set; }
    }
   
}
