using WifiConnection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPUni.Events;

namespace UPUni.WifiManager
{
    /// <summary>
    /// Class to manage interfaces wifi
    /// </summary>
    public class ManagerWifi
    {
        private Wifi Wifi { get; set; }

        #region Delegates
        /// <summary>
        /// Event connection status changed delegate
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Argument of event <see cref="WifiStatusEventArgs"/></param>
        public delegate void ConnectionStatusChangedHandler(object sender, WifiStatusEventArgs e);
        /// <summary>
        /// Event connection completed delegate
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Argument of event <see cref="WifiConnectedCompleteEventArgs"/></param>
        public delegate void ConnectedCompleteHandler(object sender, WifiConnectedCompleteEventArgs e);
        #endregion

        #region Events
        /// <summary>
        /// Event connection status changed <see cref="ConnectionStatusChangedHandler"/>
        /// </summary>
        public event ConnectionStatusChangedHandler ConnectionStatusChanged;
        /// <summary>
        /// Event connection completed delegate <see cref="WifiConnectedCompleteEventArgs"/>
        /// </summary>
        public event ConnectedCompleteHandler ConnectedComplete;
        #endregion

        /// <summary>
        /// Create new wifi manager
        /// </summary>
        /// <exception cref="Exception">NO WIFI CARD WAS FOUND</exception>
        public ManagerWifi()
        {
            // Init wifi object and event handlers
            this.Wifi = new Wifi();
            this.Wifi.ConnectionStatusChanged += Wifi_ConnectionStatusChanged;

            if (this.Wifi.NoWifiAvailable)
            {
                throw new Exception("NO WIFI CARD WAS FOUND");
            }
        }

        /// <summary>
        /// Disconnect all wifi interfaces
        /// </summary>
        public void Disconnect()
        {
            this.Wifi.Disconnect();
        }

        /// <summary>
        /// Get status connection
        /// </summary>
        /// <returns>Connection status <see cref="WifiStatus"/></returns>
        public WifiStatus Status()
        {
            return this.Wifi.ConnectionStatus;
        }

        /// <summary>
        /// Get list access points.
        /// </summary>
        /// <returns>List of access points <see cref="AccessPoint"/></returns>
        public List<AccessPoint> ListWifi()
        {
            List<AccessPoint> ret = new List<AccessPoint>();
            IEnumerable<AccessPoint> accessPoints = this.Wifi.GetAccessPoints().OrderByDescending(ap => ap.SignalStrength);

            foreach (AccessPoint item in accessPoints)
            {
                ret.Add(item);
            }

            return ret;
        }

        /// <summary>
        /// Connect async wifi
        /// </summary>
        /// <param name="ssid">SSID of wifi</param>
        /// <param name="password">Password of wifi</param>
        /// <param name="domain">domain of wifi</param>
        /// <param name="overwriteProfile">Overwrite profile access point</param>
        public void ConnectInWifiAsync(string ssid, string password = "", string domain = "", bool overwriteProfile = true)
        {
            var accessPoints = ListWifi();

            int selectedIndex = this.GetIndexWifi(accessPoints, ssid);
            AccessPoint selectedAP = accessPoints.ToList()[selectedIndex];

            // Auth
            AuthRequest authRequest = new AuthRequest(selectedAP);
            bool overwrite = true;

            if (authRequest.IsPasswordRequired)
            {
                if (overwriteProfile)
                {
                    if (authRequest.IsUsernameRequired)
                    {
                        authRequest.Username = ssid;
                    }
                    
                    authRequest.Password = this.IsValidPassword(selectedAP, password);

                    if (authRequest.IsDomainSupported)
                    {
                        authRequest.Domain = domain;
                    }
                }
            }

            selectedAP.ConnectAsync(authRequest, overwrite, this.OnConnectedComplete);
        }

        /// <summary>
        /// Connect async wifi if overwrite profile true
        /// </summary>
        /// <param name="ssid">SSID of wifi</param>
        /// <param name="password">Password of wifi</param>
        /// <param name="domain">domain of wifi</param>
        public void ConnectInWifiAsync(string ssid, string password, string domain = "")
        {
            this.ConnectInWifiAsync(ssid, password, domain, true);
        }

        /// <summary>
        /// Connect async wifi if overwrite profile false
        /// </summary>
        /// <param name="ssid">SSID of wifi</param>
        /// <param name="domain">domain of wifi</param>
        public void ConnectInWifiAsync(string ssid, string domain = "")
        {
            this.ConnectInWifiAsync(ssid, string.Empty, domain, false);
        }

        /// <summary>
        /// Connect sync wifi
        /// </summary>
        /// <param name="ssid">SSID of wifi</param>
        /// <param name="password">Password of wifi</param>
        /// <param name="domain">domain of wifi</param>
        /// <param name="overwriteProfile">Overwrite profile access point</param>
        /// <returns>Is success or danger connection</returns>
        public bool ConnectInWifi(string ssid, string password = "", string domain = "", bool overwriteProfile = true)
        {
            var accessPoints = ListWifi();

            int selectedIndex = this.GetIndexWifi(accessPoints, ssid);
            AccessPoint selectedAP = accessPoints.ToList()[selectedIndex];

            // Auth
            AuthRequest authRequest = new AuthRequest(selectedAP);
            bool overwrite = true;

            if (authRequest.IsPasswordRequired)
            {
                if (overwriteProfile)
                {
                    if (authRequest.IsUsernameRequired)
                    {
                        authRequest.Username = ssid;
                    }

                    authRequest.Password = this.IsValidPassword(selectedAP, password);

                    if (authRequest.IsDomainSupported)
                    {
                        authRequest.Domain = domain;
                    }
                }
            }

            return selectedAP.Connect(authRequest, overwrite);
        }

        /// <summary>
        /// Connect sync wifi if overwrite profile true
        /// </summary>
        /// <param name="ssid">SSID of wifi</param>
        /// <param name="password">Password of wifi</param>
        /// <param name="domain">domain of wifi</param>
        public bool ConnectInWifi(string ssid, string password, string domain = "")
        {
            return this.ConnectInWifi(ssid, password, domain, true);
        }

        /// <summary>
        /// Connect sync wifi if overwrite profile false
        /// </summary>
        /// <param name="ssid">SSID of wifi</param>
        /// <param name="domain">domain of wifi</param>
        public bool ConnectInWifi(string ssid, string domain = "")
        {
            return this.ConnectInWifi(ssid, string.Empty, domain, false);
        }

        /// <summary>
        /// Get XML profile access point
        /// </summary>
        /// <param name="ssid">SSID access point</param>
        /// <returns>String XML profile</returns>
        public string ProfileXML(string ssid)
        {
            var accessPoints = ListWifi();

            int selectedIndex = this.GetIndexWifi(accessPoints, ssid);
            AccessPoint selectedAP = accessPoints.ToList()[selectedIndex];

            return selectedAP.GetProfileXML();
        }

        /// <summary>
        /// Delete profile access point
        /// </summary>
        /// <param name="ssid">SSID access point</param>
        public void DeleteProfile(string ssid)
        {
            var accessPoints = ListWifi();

            int selectedIndex = this.GetIndexWifi(accessPoints, ssid);
            AccessPoint selectedAP = accessPoints.ToList()[selectedIndex];

            selectedAP.DeleteProfile();
        }

        /// <summary>
        /// Show infos access point
        /// </summary>
        /// <param name="ssid">SSID access point</param>
        /// <returns>Object access point <see cref="AccessPoint"/></returns>
        public AccessPoint ShowInfoWifi(string ssid)
        {
            var accessPoints = ListWifi();

            int selectedIndex = this.GetIndexWifi(accessPoints, ssid);
            AccessPoint selectedAP = accessPoints.ToList()[selectedIndex];

            return selectedAP;
        }

        /// <summary>
        /// Get index to list access points
        /// </summary>
        /// <param name="accessPoints">List of access points <see cref="AccessPoint"/></param>
        /// <param name="ssid">SSID find in list</param>
        /// <returns></returns>
        public int GetIndexWifi(List<AccessPoint> accessPoints, string ssid)
        {
            int selectedIndex = -1;
            foreach (AccessPoint item in accessPoints)
            {
                selectedIndex++;

                if (item.Name.Equals(ssid))
                {
                    break;
                }
            }
            return selectedIndex;
        }

        private string IsValidPassword(AccessPoint selectedAP, string password)
        {
            bool validPassFormat = false;

            validPassFormat = selectedAP.IsValidPassword(password);

            if (!validPassFormat)
            {
                throw new Exception("Password is not valid for this network type.");
            }

            return password;
        }

        private void OnConnectedComplete(bool success)
        {
            this.OnConnectedComplete(this, new WifiConnectedCompleteEventArgs(success));
        }

        private void Wifi_ConnectionStatusChanged(object sender, WifiStatusEventArgs e)
        {
            this.OnConnectionStatusChanged(sender, e);
        }

        private void OnConnectionStatusChanged(object sender, WifiStatusEventArgs e)
        {
            ConnectionStatusChangedHandler handler = this.ConnectionStatusChanged;
            handler?.Invoke(this, e);
        }

        private void OnConnectedComplete(object sender, WifiConnectedCompleteEventArgs e)
        {
            ConnectedCompleteHandler handler = this.ConnectedComplete;
            handler?.Invoke(this, e);
        }
    }
}
