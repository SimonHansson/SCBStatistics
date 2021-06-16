using System.Collections.Generic;

namespace SCB.Models
{
    /// <summary>
    /// Varuable model 
    /// </summary>
    public class Variable
    {
        public string code { get; set; }
        public string text { get; set; }
        public List<string> values { get; set; }
        public List<string> valueTexts { get; set; }
        public bool elimination { get; set; }
        public bool? time { get; set; }
    }
}
