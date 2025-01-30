using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPUni.StringsCombinations
{
    /// <summary>
    /// Class defaults configurations
    /// </summary>
    public class Defaults
    {
        /// <summary>
        /// Type combination
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Name combination
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Minimum of characters
        /// </summary>
        public int Minimum { get; set; }
        /// <summary>
        /// Maximum of characters
        /// </summary>
        public int Maximum { get; set; }
        /// <summary>
        /// Characters Upper
        /// </summary>
        public bool Upper { get; set; }
        /// <summary>
        /// Characters Lower
        /// </summary>
        public bool Lower { get; set; }
        /// <summary>
        /// Characters Number
        /// </summary>
        public bool Number { get; set; }
        /// <summary>
        /// Characters Special
        /// </summary>
        public bool Special { get; set; }
        /// <summary>
        /// Characters Accents Lower
        /// </summary>
        public bool AccentsLower { get; set; }
        /// <summary>
        /// Characters Accents Upper
        /// </summary>
        public bool AccentsUpper { get; set; }
        /// <summary>
        /// Others characters
        /// </summary>
        public string Others { get; set; }
        /// <summary>
        /// Remove characters
        /// </summary>
        public string Remove { get; set; }
    }
}
