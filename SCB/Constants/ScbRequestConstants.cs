using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCB.Constants
{
    /// <summary>
    /// Constats used in request to Scb api
    /// </summary>
    public static class ScbRequestConstants
    {
        /// <summary>
		/// Region filter
		/// </summary>
		public const string RegionFilter = "vs:RegionKommun07+BaraEjAggr";

        // <summary>
        /// Region code
        /// </summary>
        public const string RegionCode = "Region";

        // <summary>
        /// Content selection filter
        /// </summary>
        public const string ContentSelectionFilter = "item";

        // <summary>
        /// Content selection take all value 
        /// </summary>
        public const string ContentSelectionValue = "ME0104B8";

        // <summary>
        /// Content query code 
        /// </summary>
        public const string ContentQueryCode = "ContentsCode";

        // <summary>
        /// Response format 
        /// </summary>
        public const string ResponseFormatJson = "json";
    }
}
