using WifiConnection.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using WifiConnection.Win32.Interop;

namespace WifiConnection
{
	/// <summary>
	/// Class from access point
	/// </summary>
	public class AccessPoint
	{
		private WlanInterface _interface { get; set; }
		private WlanAvailableNetwork _network { get; set; }

		/// <summary>
		/// Get SSID to access point
		/// </summary>
        public string Name
        {
            get
            {
                return Encoding.ASCII.GetString(_network.dot11Ssid.SSID, 0, (int)_network.dot11Ssid.SSIDLength);
            }
        }

        /// <summary>
        /// Get signal strength to access point
        /// </summary>
        public uint SignalStrength
        {
            get
            {
                return _network.wlanSignalQuality;
            }
        }

        /// <summary>
        /// If the computer has a connection profile stored for this access point
        /// </summary>
        public bool HasProfile
        {
            get
            {
                try
                {
                    return _interface.GetProfiles().Where(p => p.profileName == Name).Any();
                }
                catch
                {
                    return false;
                }
            }
        }

		/// <summary>
		/// If access point secure
		/// </summary>
        public bool IsSecure
        {
            get
            {
                return _network.securityEnabled;
            }
        }

        /// <summary>
        /// If access point connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                try
                {
                    var a = _interface.CurrentConnection; // This prop throws exception if not connected, which forces me to this try catch. Refactor plix.
                    return a.profileName == _network.profileName;
                }
                catch
                {
                    return false;
                }
            }

        }

        /// <summary>
        /// Returns the underlying network object <see cref="WlanAvailableNetwork"/>
        /// </summary>
        internal WlanAvailableNetwork Network
        {
            get
            {
                return _network;
            }
        }


        /// <summary>
        /// Returns the underlying interface object <see cref="WlanInterface"/>
        /// </summary>
        internal WlanInterface Interface
        {
            get
            {
                return _interface;
            }
        }

        internal AccessPoint(WlanInterface interfac, WlanAvailableNetwork network)
		{
			_interface = interfac;
			_network = network;
		}

		/// <summary>
		/// Checks that the password format matches this access point's encryption method.
		/// </summary>
		/// <param name="password">Strind password</param>
		/// <returns>If password is valid</returns>
		public bool IsValidPassword(string password)
		{
			return PasswordHelper.IsValid(password, _network.dot11DefaultCipherAlgorithm);
		}

        /// <summary>
        /// Connect synchronous to the access point.
        /// </summary>
        /// <param name="request">Object auth request <see cref="AuthRequest"/></param>
        /// <param name="overwriteProfile">Check true for overwrite profile</param>
        /// <returns>Retrun if success or danger in connection</returns>
        public bool Connect(AuthRequest request, bool overwriteProfile = false)
		{
			// No point to continue with the connect if the password is not valid if overwrite is true or profile is missing.
			if (!request.IsPasswordValid && (!HasProfile || overwriteProfile))
				return false;

			// If we should create or overwrite the profile, do so.
			if (!HasProfile || overwriteProfile)
			{				
				if (HasProfile)
					_interface.DeleteProfile(Name);

				request.Process();				
			}


			// TODO: Auth algorithm: IEEE80211_Open + Cipher algorithm: None throws an error.
			// Probably due to connectionmode profile + no profile exist, cant figure out how to solve it though.
			return _interface.ConnectSynchronously(WlanConnectionMode.Profile, _network.dot11BssType, Name, 6000);			
		}

        /// <summary>
        /// Connect asynchronous to the access point.
        /// </summary>
        /// <param name="request">Object auth request <see cref="AuthRequest"/></param>
        /// <param name="overwriteProfile">Check true for overwrite profile</param>
        /// <param name="onConnectComplete">Action retrun connection <see cref="Action"/></param>
        public void ConnectAsync(AuthRequest request, bool overwriteProfile = false, Action<bool> onConnectComplete = null)
		{
			// TODO: Refactor -> Use async connect in wlaninterface.
			ThreadPool.QueueUserWorkItem(new WaitCallback((o) => {
				bool success = false;

				try
				{
					success = Connect(request, overwriteProfile);
				}
				catch (Win32Exception)
				{					
					success = false;
				}

				if (onConnectComplete != null)
					onConnectComplete(success);
			}));
		}

        /// <summary>
        /// Get XML of profile access point
        /// </summary>
        /// <returns>String XML</returns>
        public string GetProfileXML()
		{
			if (HasProfile)
				return _interface.GetProfileXml(Name);
			else
				return string.Empty;
		}

        /// <summary>
        /// Delete profile access point
        /// </summary>
		public void DeleteProfile()
		{
			try
			{
				if (HasProfile)
					_interface.DeleteProfile(Name);
			}
			catch { }
		}

        /// <summary>
        /// Convert to string access point information 
        /// </summary>
        /// <returns>Access point informations</returns>
		public override sealed string ToString()
		{
			StringBuilder info = new StringBuilder();
            info.AppendLine("Name: " + this.Name);
            info.AppendLine("RSSI: " + _interface.RSSI);
            info.AppendLine("Interface: " + _interface.InterfaceName);
			info.AppendLine("Auth algorithm: " + _network.dot11DefaultAuthAlgorithm);
			info.AppendLine("Cipher algorithm: " + _network.dot11DefaultCipherAlgorithm);
			info.AppendLine("BSS type: " + _network.dot11BssType);
			info.AppendLine("Connectable: " + _network.networkConnectable);
			
			if (!_network.networkConnectable)
				info.AppendLine("Reason to false: " + _network.wlanNotConnectableReason);

			return info.ToString();
		}
	}
}
