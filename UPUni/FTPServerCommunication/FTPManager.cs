using FluentFTP;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPUni.Events;
using UPUni.FTPServerCommunication.FTP;
using UPUni.FTPServerCommunication.SFTP;

namespace UPUni.FTPServerCommunication
{
    /// <summary>
    /// Class FTP manager
    /// </summary>
    public class FTPManager
    {
        private UPUni.Enums.FTPType type { get; set; }
        private SftpService sftpService { get; set; }
        private FtpService ftpService { get; set; }
        private SftpClient sftpClient { get; set; }
        private FtpClient ftpClient { get; set; }

        /// <summary>
        /// Event log server delegate
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Arguments server log <see cref="ServerLogEventArgs"/></param>
        public delegate void ServerLogEventHandler(object sender, ServerLogEventArgs e);

        /// <summary>
        /// Event log server <see cref="ServerLogEventHandler"/>
        /// </summary>
        public event ServerLogEventHandler ServerLog;

        /// <summary>
        /// Create new FTP manager
        /// </summary>
        /// <param name="ptype">Type FTP <see cref="UPUni.Enums.FTPType"/></param>
        /// <param name="config">Configs FTP <see cref="FtpConfig"/></param>
        public FTPManager(UPUni.Enums.FTPType ptype, FtpConfig config)
        {
            this.type = ptype;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    this.ftpService = new FtpService(config);
                    this.ftpService.ServerLog += FtpService_ServerLog;
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    this.sftpService = new SftpService(config);
                    this.sftpService.ServerLog += SftpService_ServerLog;
                    break;
            }
        }

        /// <summary>
        /// If is FTP connected
        /// </summary>
        /// <returns>Retrurn FTP connected</returns>
        public bool IsConnected()
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.IsConnected(this.ftpClient);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.IsConnected(this.sftpClient);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Connect FTP
        /// </summary>
        /// <returns>If FTP connected</returns>
        public bool Connect()
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    FtpClient _ftpClient = null;
                    ret = this.ftpService.Connect(out _ftpClient);
                    this.ftpClient = _ftpClient;
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    SftpClient _sftpClient = null;
                    ret = this.sftpService.Connect(out _sftpClient);
                    this.sftpClient = _sftpClient;
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="localFilePath">Local file to download</param>
        /// <param name="remoteFilePath">Remote file download</param>
        /// <returns>If file downloaded</returns>
        public bool DownloadFile(string localFilePath, string remoteFilePath)
        {
            bool ret = false;

            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.DownloadFile(this.ftpClient, localFilePath, remoteFilePath);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.DownloadFile(this.sftpClient, localFilePath, remoteFilePath);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="localFilePath">Local file to download</param>
        /// <param name="remoteFilePath">Remote file download</param>
        /// <returns>If file uploaded</returns>
        public bool UploadFile(string localFilePath, string remoteFilePath)
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.UploadFile(this.ftpClient, localFilePath, remoteFilePath);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.UploadFile(this.sftpClient, localFilePath, remoteFilePath);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// List all directorys remotes
        /// </summary>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>List all remote direcotrys <see cref="DirectoryInfo"/></returns>
        public List<DirectoryInfo> ListAllDirectorys(string remoteDirectory)
        {
            List<DirectoryInfo> ret = new List<DirectoryInfo>();

            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.ListAllDirectorys(this.ftpClient, remoteDirectory);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.ListAllDirectorys(this.sftpClient, remoteDirectory);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Verify exist directory
        /// </summary>
        /// <param name="remoteDirectory">Remote directory to verify</param>
        /// <returns>Return if directory exists</returns>
        public bool ExistsDirectory(string remoteDirectory)
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.ExistsDirectory(this.ftpClient, remoteDirectory);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.ExistsDirectory(this.sftpClient, remoteDirectory);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Create remote directory
        /// </summary>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>Return if directory is created</returns>
        public bool CreateDirectory(string remoteDirectory)
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.CreateDirectory(this.ftpClient, remoteDirectory);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.CreateDirectory(this.sftpClient, remoteDirectory);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Delete remote directory
        /// </summary>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>Return if directory is deleted</returns>
        public bool DeleteDirectory(string remoteDirectory)
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.DeleteDirectory(this.ftpClient, remoteDirectory);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.DeleteDirectory(this.sftpClient, remoteDirectory);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// List all files directory
        /// </summary>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>List files <see cref="FileInfo"/></returns>
        public List<FileInfo> ListAllFiles(string remoteDirectory)
        {
            List<FileInfo> ret = new List<FileInfo>();

            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.ListAllFiles(this.ftpClient, remoteDirectory);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.ListAllFiles(this.sftpClient, remoteDirectory);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Verify if file exists
        /// </summary>
        /// <param name="remoteFilePath">Remote file</param>
        /// <returns>Return if file exists</returns>
        public bool ExistsFile(string remoteFilePath)
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.ExistsFile(this.ftpClient, remoteFilePath);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.ExistsFile(this.sftpClient, remoteFilePath);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Delete remote file
        /// </summary>
        /// <param name="remoteFilePath">Remote file</param>
        /// <returns>Return if file is deleted</returns>
        public bool DeleteFile(string remoteFilePath)
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.DeleteFile(this.ftpClient, remoteFilePath);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.DeleteFile(this.sftpClient, remoteFilePath);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Rename remote file
        /// </summary>
        /// <param name="remoteFilePath">Remote file</param>
        /// <param name="remoteFilePathNewName">Remote file new name</param>
        /// <returns>Return if file is renamed</returns>
        public bool RenameFile(string remoteFilePath, string remoteFilePathNewName)
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.RenameFile(this.ftpClient, remoteFilePath, remoteFilePathNewName);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.RenameFile(this.sftpClient, remoteFilePath, remoteFilePathNewName);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Change permission remote file
        /// </summary>
        /// <param name="remoteFilePath">Remote file</param>
        /// <param name="newPermission">New permission</param>
        /// <returns>Return if file is renamed</returns>
        public bool ChangePermission(string remoteFilePath, string newPermission)
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.ChangePermission(this.ftpClient, remoteFilePath, newPermission);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.ChangePermission(this.sftpClient, remoteFilePath, newPermission);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Disconnect FTP
        /// </summary>
        /// <returns>Retrun if FTP id disconnected</returns>
        public bool Disconnect()
        {
            bool ret = false;
            switch (this.type)
            {
                case UPUni.Enums.FTPType.FTP:
                    ret = this.ftpService.Disconnect(this.ftpClient);
                    break;
                case UPUni.Enums.FTPType.SFTP:
                    ret = this.sftpService.Disconnect(this.sftpClient);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Event called server log
        /// </summary>
        /// <param name="e">Argument server log <see cref="ServerLogEventArgs"/></param>
        protected virtual void OnServerLog(ServerLogEventArgs e)
        {
            ServerLogEventHandler handler = this.ServerLog;
            handler?.Invoke(this, e);
        }

        private void SftpService_ServerLog(object sender, ServerLogEventArgs e)
        {
            this.OnServerLog(e);
        }

        private void FtpService_ServerLog(object sender, ServerLogEventArgs e)
        {
            this.OnServerLog(e);
        }
    }
}
