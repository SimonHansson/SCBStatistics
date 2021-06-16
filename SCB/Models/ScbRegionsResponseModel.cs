using System.Collections.Generic;

namespace SCB.Models
{
    /// <summary>
    /// Response model for Scb request
    /// </summary>
    public class ScbRegionsResponseModel
    {
        public string title { get; set; }
        public List<Variable> variables { get; set; }
    }
}
