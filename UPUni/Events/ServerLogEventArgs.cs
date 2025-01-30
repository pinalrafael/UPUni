using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPUni.Events
{
    /// <summary>
    /// Class server log event arguments <see cref="FTPServerCommunication.FTPManager"/>
    /// </summary>
    public class ServerLogEventArgs : EventArgs
    {
        /// <summary>
        /// Type log <see cref="TypeLog"/>
        /// </summary>
        public UPUni.Enums.TypeLog TypeLog { get; private set; }
        /// <summary>
        /// Date time log <see cref="DateTime"/>
        /// </summary>
        public DateTime DateTimeLog { get; set; }
        /// <summary>
        /// Message log
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Esception log <see cref="Exception"/>
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Create new server event arguments log
        /// </summary>
        /// <param name="typeLog">Type log <see cref="TypeLog"/></param>
        /// <param name="dateTimeLog"> Date time log <see cref="DateTime"/></param>
        /// <param name="message">Message log</param>
        public ServerLogEventArgs(UPUni.Enums.TypeLog typeLog, DateTime dateTimeLog, string message)
        {
            this.TypeLog = typeLog;
            this.DateTimeLog = dateTimeLog;
            this.Message = message;
            this.Exception = null;
        }

        /// <summary>
        /// Create new server event arguments log with exception
        /// </summary>
        /// <param name="typeLog">Type log <see cref="TypeLog"/></param>
        /// <param name="dateTimeLog"> Date time log <see cref="DateTime"/></param>
        /// <param name="exception">Esception log <see cref="Exception"/></param>
        /// <param name="message">Message log</param>
        public ServerLogEventArgs(UPUni.Enums.TypeLog typeLog, DateTime dateTimeLog, Exception exception, string message)
        {
            this.TypeLog = typeLog;
            this.DateTimeLog = dateTimeLog;
            this.Exception = exception;
            this.Message = message;
        }
    }
}
