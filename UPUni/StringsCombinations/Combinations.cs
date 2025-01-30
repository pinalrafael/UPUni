using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UPUni.Events;

namespace UPUni.StringsCombinations
{
    /// <summary>
    /// Class of generate strings combinations
    /// </summary>
    public class Combinations
    {
        private char[] CharactersUpper = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private char[] CharactersLower = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private char[] CharactersNumber = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private char[] CharactersSpecial = new char[] { ' ', '´', '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '{', '[', '}', ']', '\\', '|', ':', ';', ',', '<', '.', '>', '?', '/', '"', '\'' };
        private char[] CharactersAccentsLower = new char[] { 'á', 'à', 'ã', 'â', 'é', 'è', 'ê', 'í', 'ì', 'î', 'ó', 'ò', 'õ', 'ô', 'ú', 'ù', 'û' };
        private char[] CharactersAccentsUpper = new char[] { 'Á', 'À', 'Ã', 'Â', 'É', 'È', 'Ê', 'Í', 'Ì', 'Î', 'Ó', 'Ò', 'Õ', 'Ô', 'Ú', 'Ù', 'Û' };

        private int MaxCombinations { get; set; }
        private bool IsCancel { get; set; }
        private Defaults Defaults { get; set; }
        private List<char> Chars { get; set; }
        /// <summary>
        /// Get combinations count
        /// </summary>
        public BigInteger CombinationCount { get; private set; }

        #region Delegates
        /// <summary>
        /// Event combinations generated delegate
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Argument combinations generated <see cref="GenerateCombinationsEventArgs"/></param>
        public delegate void GenerateCombinationsEventHandler(object sender, GenerateCombinationsEventArgs e);
        #endregion

        #region Events
        /// <summary>
        /// Event combinations generated <see cref="GenerateCombinationsEventHandler"/>
        /// </summary>
        public event GenerateCombinationsEventHandler GenerateCombinationsEvent;
        #endregion

        /// <summary>
        /// Create new combinations
        /// </summary>
        /// <param name="defaults">Confis to generate strings <see cref="Defaults"/></param>
        /// <param name="maxCombinations">Limite combinations</param>
        public Combinations(Defaults defaults, int maxCombinations = 0)
        {
            this.CombinationCount = 0;
            this.Defaults = defaults;
            this.MaxCombinations = maxCombinations;

            this.Chars = new List<char>();
            if (this.Defaults.Upper)
            {
                this.Chars.AddRange(this.CharactersUpper);
            }
            if (this.Defaults.Lower)
            {
                this.Chars.AddRange(this.CharactersLower);
            }
            if (this.Defaults.Number)
            {
                this.Chars.AddRange(this.CharactersNumber);
            }
            if (this.Defaults.Special)
            {
                this.Chars.AddRange(this.CharactersSpecial);
            }
            if (this.Defaults.AccentsLower)
            {
                this.Chars.AddRange(this.CharactersAccentsLower);
            }
            if (this.Defaults.AccentsUpper)
            {
                this.Chars.AddRange(this.CharactersAccentsUpper);
            }
            if (this.Defaults.Others != null && this.Defaults.Others.Length > 0)
            {
                foreach (var item in this.Defaults.Others)
                {
                    if (!this.FindChar(item))
                    {
                        this.Chars.Add(item);
                    }
                }
            }

            if (this.Defaults.Remove != null && this.Defaults.Remove.Length > 0)
            {
                foreach (var item in this.Defaults.Remove)
                {
                    if (this.FindChar(item))
                    {
                        this.Chars.Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// Generate combinations
        /// </summary>
        public void GenerateCombinations()
        {
            this.GenerateCombinations(this.Chars.ToArray(), this.Defaults.Minimum, this.Defaults.Maximum);
        }

        /// <summary>
        /// Calculate total of combinations
        /// </summary>
        /// <returns>Total of combinations <see cref="BigInteger"/></returns>
        public BigInteger CalculateTotalCombinations()
        {
            int N = this.Chars.ToArray().Length;
            BigInteger totalCombinations = 0;

            for (int length = this.Defaults.Minimum; length <= this.Defaults.Maximum; length++)
            {
                totalCombinations += BigInteger.Pow(N, length);
            }

            return totalCombinations;
        }

        /// <summary>
        /// Calculate total of combinations bytes
        /// </summary>
        /// <returns>Total of combinations bytes <see cref="BigInteger"/></returns>
        public BigInteger CalculateTotalCombinationsBytes()
        {
            int N = this.Chars.ToArray().Length;
            BigInteger totalBytes = 0;

            for (int length = this.Defaults.Minimum; length <= this.Defaults.Maximum; length++)
            {
                BigInteger totalCombinations = BigInteger.Pow(N, length);

                BigInteger bytesPerCombination = length * 2;

                totalBytes += totalCombinations * bytesPerCombination;
            }

            return totalBytes;
        }

        /// <summary>
        /// Convert total bytes to size
        /// </summary>
        /// <param name="bytes">Total bytes <see cref="BigInteger"/></param>
        /// <returns>File size</returns>
        public string ConvertBytesToReadableSize(BigInteger bytes)
        {
            if (bytes < 1024)
                return $"{bytes} bytes";
            else if (bytes < 1024 * 1024)
                return $"{bytes / new BigInteger(1024.0):F2} KB";
            else if (bytes < 1024L * 1024L * 1024L)
                return $"{bytes / new BigInteger(1024.0 * 1024):F2} MB";
            else if (bytes < 1024L * 1024L * 1024L * 1024L)
                return $"{bytes / new BigInteger(1024.0 * 1024 * 1024):F2} GB";
            else if (bytes < 1024L * 1024L * 1024L * 1024L * 1024L)
                return $"{bytes / new BigInteger(1024.0 * 1024 * 1024 * 1024):F2} TB";
            else
                return $"{bytes / new BigInteger(1024.0 * 1024 * 1024 * 1024 * 1024):F2} PB";
        }

        private void GenerateCombinations(char[] characters, int minLength, int maxLength)
        {
            for (int length = minLength; length <= maxLength; length++)
            {
                this.GenerateCombinationsRecursive(characters, length, 0, new char[length]);

                if (this.IsCancel)
                {
                    return;
                }

                if (this.MaxCombinations > 0)
                {
                    if (this.CombinationCount >= this.MaxCombinations)
                        break;
                }
            }
        }

        private void GenerateCombinationsRecursive(char[] characters, int length, int index, char[] currentCombination)
        {
            if (index == length)
            {
                this.CombinationCount++;

                GenerateCombinationsEventArgs generateCombinationsEventArgs = new GenerateCombinationsEventArgs(this.CombinationCount, currentCombination);
                this.OnGenerateCombinationsEvent(generateCombinationsEventArgs);
                this.IsCancel = generateCombinationsEventArgs.Cancel;

                if (this.IsCancel)
                {
                    return;
                }

                if (this.MaxCombinations > 0)
                {
                    if (this.CombinationCount >= this.MaxCombinations)
                        return;
                }

                return;
            }

            for (int i = 0; i < characters.Length; i++)
            {
                currentCombination[index] = characters[i];
                this.GenerateCombinationsRecursive(characters, length, index + 1, currentCombination);

                if (this.MaxCombinations > 0)
                {
                    if (this.CombinationCount >= this.MaxCombinations)
                        return;
                }
            }
        }

        private bool FindChar(char chr)
        {
            bool ret = false;
            foreach (var item in this.Chars)
            {
                if (item == chr)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        #region Events Functions
        /// <summary>
        /// Event create combinatios called
        /// </summary>
        /// <param name="e">Argument generate combinatios <see cref="GenerateCombinationsEventArgs"/></param>
        protected virtual void OnGenerateCombinationsEvent(GenerateCombinationsEventArgs e)
        {
            GenerateCombinationsEventHandler handler = this.GenerateCombinationsEvent;
            handler?.Invoke(this, e);
        }
        #endregion
    }
}
