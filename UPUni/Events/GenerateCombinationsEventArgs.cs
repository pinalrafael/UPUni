using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UPUni.Events
{
    /// <summary>
    /// Class generate combinations event arguments <see cref="PassCombinations.Combinations"/>
    /// </summary>
    public class GenerateCombinationsEventArgs : EventArgs
    {
        /// <summary>
        /// Combination current count <see cref="BigInteger"/>
        /// </summary>
        public BigInteger CombinationCount { get; set; }
        /// <summary>
        /// Array char combination current
        /// </summary>
        public char[] CurrentCombination { get; set; }
        /// <summary>
        /// String combination current
        /// </summary>
        public string CurrentCombinationStr { get; set; }
        /// <summary>
        /// Get or set if cancel generate new combinations
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Create new object generate combinations event arguments
        /// </summary>
        /// <param name="combinationCount">Combination current count <see cref="BigInteger"/></param>
        /// <param name="currentCombination">String combination current</param>
        public GenerateCombinationsEventArgs(BigInteger combinationCount, char[] currentCombination)
        {
            this.CombinationCount = combinationCount;
            this.CurrentCombination = currentCombination;
            this.CurrentCombinationStr = new string(currentCombination);
            this.Cancel = false;
        }
    }
}
