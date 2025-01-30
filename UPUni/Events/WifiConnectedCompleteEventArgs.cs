using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPUni.Events
{
    /// <summary>
    /// Event arguments wifi connected complete <see cref="WifiManager.ManagerWifi"/>
    /// </summary>
    public class WifiConnectedCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Is connected
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Create new arguments wifi connected complete
        /// </summary>
        /// <param name="isConnected">Is connected</param>
        public WifiConnectedCompleteEventArgs(bool isConnected)
        {
            this.IsConnected = isConnected;
        }
    }
}
