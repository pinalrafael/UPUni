using Renci.SshNet.Sftp;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UPUni.Events;
using FluentFTP;

namespace UPUni.FTPServerCommunication.FTP
{
    /// <summary>
    /// Class FTP service
    /// </summary>
    public class FtpService
    {
        private readonly FtpConfig _config;

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
        /// Create new FTP service
        /// </summary>
        /// <param name="sftpConfig">FTP configs <see cref="FtpConfig"/></param>
        public FtpService(FtpConfig sftpConfig)
        {
            _config = sftpConfig;
        }

        /// <summary>
        /// If is FTP connected
        /// </summary>
        /// <returns>Retrurn FTP connected</returns>
        public bool IsConnected(FtpClient client)
        {
            return client.IsConnected;
        }

        /// <summary>
        /// Connect FTP
        /// </summary>
        /// <param name="client">Return FTP client <see cref="FtpClient"/></param>
        /// <returns>If FTP connected</returns>
        public bool Connect(out FtpClient client)
        {
            bool resp = false;
            client = new FtpClient(_config.Host, _config.UserName, _config.Password, _config.Port == 0 ? 21 : _config.Port);
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, $"Connecting"));
                client.Connect();
                resp = true;
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Connect"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Connect"));
            }
            return resp;
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="localFilePath">Local file to download</param>
        /// <param name="remoteFilePath">Remote file download</param>
        /// <returns>If file downloaded</returns>
        public bool DownloadFile(FtpClient client, string localFilePath, string remoteFilePath)
        {
            FtpStatus ftpStatus = FtpStatus.Success;

            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Downloading File"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                ftpStatus = client.DownloadFile(localFilePath, remoteFilePath, FluentFTP.FtpLocalExists.Overwrite);
                if (ftpStatus == FtpStatus.Success)
                {
                    resp = true;
                }

                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Download file [{localFilePath}] from [{remoteFilePath}]"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Download file [{localFilePath}] from [{remoteFilePath}]"));
            }

            return resp;
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="localFilePath">Local file to download</param>
        /// <param name="remoteFilePath">Remote file download</param>
        /// <returns>If file uploaded</returns>
        public bool UploadFile(FtpClient client, string localFilePath, string remoteFilePath)
        {
            FtpStatus ftpStatus = FtpStatus.Success;

            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Uploading File"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                ftpStatus = client.UploadFile(localFilePath, remoteFilePath);
                if (ftpStatus == FtpStatus.Success)
                {
                    resp = true;
                }

                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Upload file [{localFilePath}] to [{remoteFilePath}]"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Upload file [{localFilePath}] to [{remoteFilePath}]"));
            }

            return resp;
        }

        /// <summary>
        /// List all directorys remotes
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>List all remote direcotrys <see cref="DirectoryInfo"/></returns>
        public List<DirectoryInfo> ListAllDirectorys(FtpClient client, string remoteDirectory)
        {
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Listing Directorys"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                List<DirectoryInfo> directories = new List<DirectoryInfo>();
                FtpListItem[] ftpdirectories = client.GetListing(remoteDirectory);
                foreach (var item in ftpdirectories)
                {
                    if (item.Type == FtpObjectType.Directory)
                    {
                        DirectoryInfo fileInfo = new DirectoryInfo(item.FullName);
                        directories.Add(fileInfo);
                    }
                }
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished List Directory [{remoteDirectory}]"));
                return directories;
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in listing Directory under [{remoteDirectory}]"));
                return null;
            }
        }

        /// <summary>
        /// Verify exist directory
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="remoteDirectory">Remote directory to verify</param>
        /// <returns>Return if directory exists</returns>
        public bool ExistsDirectory(FtpClient client, string remoteDirectory)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Existing Directory"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                resp = client.DirectoryExists(remoteDirectory);
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Exists Directory [{remoteDirectory}]"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Exists Directory [{remoteDirectory}]"));
            }
            return resp;
        }

        /// <summary>
        /// Create remote directory
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>Return if directory is created</returns>
        public bool CreateDirectory(FtpClient client, string remoteDirectory)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Creating Directory"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                resp = client.CreateDirectory(remoteDirectory);
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Create Directory [{remoteDirectory}]"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Create Directory [{remoteDirectory}]"));
            }
            return resp;
        }

        /// <summary>
        /// Delete remote directory
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>Return if directory is deleted</returns>
        public bool DeleteDirectory(FtpClient client, string remoteDirectory)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Deleting Directory"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                client.DeleteDirectory(remoteDirectory);
                resp = true;
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Delete Directory [{remoteDirectory}]"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Delete Directory [{remoteDirectory}]"));
            }
            return resp;
        }

        /// <summary>
        /// List all files directory
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>List files <see cref="FileInfo"/></returns>
        public List<FileInfo> ListAllFiles(FtpClient client, string remoteDirectory)
        {
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Listing Files"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                List<FileInfo> files = new List<FileInfo>();
                FtpListItem[] ftpfiles = client.GetListing(remoteDirectory);
                foreach (var item in ftpfiles)
                {
                    if (item.Type == FtpObjectType.File)
                    {
                        FileInfo fileInfo = new FileInfo(item.FullName);
                        files.Add(fileInfo);
                    }
                }
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished List Files [{remoteDirectory}]"));
                return files;
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in listing files under [{remoteDirectory}]"));
                return null;
            }
        }

        /// <summary>
        /// Verify if file exists
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="remoteFilePath">Remote file</param>
        /// <returns>Return if file exists</returns>
        public bool ExistsFile(FtpClient client, string remoteFilePath)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Existing File"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                resp = client.FileExists(remoteFilePath);
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Exists File [{remoteFilePath}]"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Exists File [{remoteFilePath}]"));
            }
            return resp;
        }

        /// <summary>
        /// Delete remote file
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="remoteFilePath">Remote file</param>
        /// <returns>Return if file is deleted</returns>
        public bool DeleteFile(FtpClient client, string remoteFilePath)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Deleting File"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                client.DeleteFile(remoteFilePath);
                resp = true;
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Delete File [{remoteFilePath}]"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Delete File [{remoteFilePath}]"));
            }
            return resp;
        }

        /// <summary>
        /// Rename remote file
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="remoteFilePath">Remote file</param>
        /// <param name="remoteFilePathNewName">Remote file new name</param>
        /// <returns>Return if file is renamed</returns>
        public bool RenameFile(FtpClient client, string remoteFilePath, string remoteFilePathNewName)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Renaming File"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                client.Rename(remoteFilePath, remoteFilePathNewName);
                resp = true;
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Rename File [{remoteFilePath}] to [{remoteFilePathNewName}]"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Rename File [{remoteFilePath}] to [{remoteFilePathNewName}]"));
            }
            return resp;
        }

        /// <summary>
        /// Change permission remote file
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <param name="remotePath">Remote file</param>
        /// <param name="newPermission">New permission</param>
        /// <returns>Return if file is renamed</returns>
        public bool ChangePermission(FtpClient client, string remotePath, string newPermission)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, $"Changing Permission"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                int per = 0;
                if(int.TryParse(newPermission, out per))
                {
                    client.SetFilePermissions(remotePath, per);
                    resp = true;
                }
                else
                {
                    this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.ERROR, DateTime.Now, $"Failed int Parse [{newPermission}]"));
                    resp = false;
                }

                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Permissions [{remotePath}]"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Permissions [{remotePath}]"));
            }

            return resp;
        }

        /// <summary>
        /// Disconnect FTP
        /// </summary>
        /// <param name="client">FTP client <see cref="FtpClient"/></param>
        /// <returns>Retrun if FTP id disconnected</returns>
        public bool Disconnect(FtpClient client)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, $"Disconnecting"));
                client.Disconnect();
                resp = true;
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.SUCCESS, DateTime.Now, $"Finished Disconnect"));
            }
            catch (Exception exception)
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.EXCEPTION, DateTime.Now, exception, $"Failed in Disconnect"));
            }
            return resp;
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
    }
}
