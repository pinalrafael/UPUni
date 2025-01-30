using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UPUni.Events;

namespace UPUni.FTPServerCommunication.SFTP
{
    /// <summary>
    /// Class SFTP service
    /// </summary>
    public class SftpService
    {
        private readonly FtpConfig _config;
        private int _connectiontRetryAttempts = 10;

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
        /// Create new SFTP service
        /// </summary>
        /// <param name="sftpConfig">SFTP configs <see cref="FtpConfig"/></param>
        public SftpService(FtpConfig sftpConfig)
        {
            _config = sftpConfig;
        }

        /// <summary>
        /// If is SFTP connected
        /// </summary>
        /// <returns>Retrurn SFTP connected</returns>
        public bool IsConnected(SftpClient client)
        {
            return client.IsConnected;
        }

        /// <summary>
        /// Connect SFTP
        /// </summary>
        /// <param name="client">Return SFTP client <see cref="SftpClient"/></param>
        /// <returns>If SFTP connected</returns>
        public bool Connect(out SftpClient client)
        {
            bool resp = false;
            client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.UserName, _config.Password);
            try
            {
                int attempts = 0;
                do
                {
                    try
                    {
                        this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, $"Connecting [{attempts}]"));
                        client.Connect();
                    }
                    catch (System.Net.Sockets.SocketException e)
                    {
                        attempts++;
                        Thread.Sleep(1000);
                    }
                    catch (Renci.SshNet.Common.SshConnectionException e)
                    {
                        attempts++;
                        Thread.Sleep(1000);
                    }
                } while (attempts < _connectiontRetryAttempts && !client.IsConnected);
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="localFilePath">Local file to download</param>
        /// <param name="remoteFilePath">Remote file download</param>
        /// <returns>If file downloaded</returns>
        public bool DownloadFile(SftpClient client, string remoteFilePath, string localFilePath)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Downloading File"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                var s = File.Create(localFilePath);
                client.DownloadFile(remoteFilePath, s);
                s.Close();
                resp = true;
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="localFilePath">Local file to download</param>
        /// <param name="remoteFilePath">Remote file download</param>
        /// <returns>If file uploaded</returns>
        public bool UploadFile(SftpClient client, string localFilePath, string remoteFilePath)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Uploading File"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                var s = File.OpenRead(localFilePath);
                client.UploadFile(s, remoteFilePath);
                resp = true;
                s.Close();
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>List all remote direcotrys <see cref="DirectoryInfo"/></returns>
        public List<DirectoryInfo> ListAllDirectorys(SftpClient client, string remoteDirectory)
        {
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Listing Directorys"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                List<DirectoryInfo> directories = new List<DirectoryInfo>();
                IEnumerable<SftpFile> sftpdirectories = client.ListDirectory(remoteDirectory);
                foreach (var item in sftpdirectories)
                {
                    if (item.IsDirectory)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(item.FullName);
                        directories.Add(directoryInfo);
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="remoteDirectory">Remote directory to verify</param>
        /// <returns>Return if directory exists</returns>
        public bool ExistsDirectory(SftpClient client, string remoteDirectory)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Existing Directory"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                resp = client.Exists(remoteDirectory);
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>Return if directory is created</returns>
        public bool CreateDirectory(SftpClient client, string remoteDirectory)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Creating Directory"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                client.CreateDirectory(remoteDirectory);
                resp = true;
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>Return if directory is deleted</returns>
        public bool DeleteDirectory(SftpClient client, string remoteDirectory)
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="remoteDirectory">Remote directory</param>
        /// <returns>List files <see cref="FileInfo"/></returns>
        public List<FileInfo> ListAllFiles(SftpClient client, string remoteDirectory)
        {
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Listing Files"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                List<FileInfo> files = new List<FileInfo>();
                IEnumerable<SftpFile> sftpFiles = client.ListDirectory(remoteDirectory);
                foreach (var item in sftpFiles)
                {
                    if (!item.IsDirectory)
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="remoteFilePath">Remote file</param>
        /// <returns>Return if file exists</returns>
        public bool ExistsFile(SftpClient client, string remoteFilePath)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Existing File"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                resp = client.Exists(remoteFilePath);
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="remoteFilePath">Remote file</param>
        /// <returns>Return if file is deleted</returns>
        public bool DeleteFile(SftpClient client, string remoteFilePath)
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="remoteFilePath">Remote file</param>
        /// <param name="remoteFilePathNewName">Remote file new name</param>
        /// <returns>Return if file is renamed</returns>
        public bool RenameFile(SftpClient client, string remoteFilePath, string remoteFilePathNewName)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, "Renaming File"));
                if (!client.IsConnected)
                {
                    this.Connect(out client);
                }
                client.RenameFile(remoteFilePath, remoteFilePathNewName);
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <param name="remotePath">Remote file</param>
        /// <param name="newPermission">New permission</param>
        /// <returns>Return if file is renamed</returns>
        public bool ChangePermission(SftpClient client, string remotePath, string newPermission)
        {
            bool resp = false;
            try
            {
                this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.INFO, DateTime.Now, $"Changing Permission"));
                if (!client.IsConnected)
                {
                    client.Connect();
                }
                short per = 0;
                if (short.TryParse(newPermission, out per))
                {
                    client.ChangePermissions(remotePath, per);
                    resp = true;
                }
                else
                {
                    this.OnServerLog(new ServerLogEventArgs(Enums.TypeLog.ERROR, DateTime.Now, $"Failed short Parse [{newPermission}]"));
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
        /// <param name="client">SFTP client <see cref="SftpClient"/></param>
        /// <returns>Retrun if FTP id disconnected</returns>
        public bool Disconnect(SftpClient client)
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
