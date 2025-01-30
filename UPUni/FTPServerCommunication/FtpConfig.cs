using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPUni.FTPServerCommunication
{
    /// <summary>
    /// Class FTP configs
    /// </summary>
    public class FtpConfig
    {
        /// <summary>
        /// FTP host
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// FTP port
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// FTP username
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// FTP password
        /// </summary>
        public string Password { get; set; }
    }
}
