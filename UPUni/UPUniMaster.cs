using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UPUni
{
    /// <summary>
    /// Class for shared functions
    /// </summary>
    public class UPUniMaster
    {
        /// <summary>
        /// Get assembly version.
        /// </summary>
        /// <returns>Version assembly <see cref="Version"/></returns>
        public static Version GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Version;
        }
    }
}
