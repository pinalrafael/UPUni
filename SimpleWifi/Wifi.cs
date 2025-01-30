using WifiConnection.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using WifiConnection.Win32.Interop;

using NotifCodeACM = WifiConnection.Win32.Interop.WlanNotificationCodeAcm;
using NotifCodeMSM = WifiConnection.Win32.Interop.WlanNotificationCodeMsm;

namespace WifiConnection
{
    /// <summary>
    /// Class to connection an wifi interfaces.
    /// </summary>
    public class Wifi
	{
        private WlanClient _client { get; set; }
        private WifiStatus _connectionStatus { get; set; }
        private bool _isConnectionStatusSet = false;
		/// <summary>
		/// Return if Wifi is not avaliable
		/// </summary>
        public bool NoWifiAvailable = false;
        
		/// <summary>
        /// Event for changed connection state
        /// </summary>
        public event EventHandler<WifiStatusEventArgs> ConnectionStatusChanged;

		/// <summary>
		/// Start new connection object
		/// </summary>
		public Wifi()
		{
			_client = new WlanClient();
            NoWifiAvailable = _client.NoWifiAvailable;
            if (_client.NoWifiAvailable)
                return;
			
			foreach (var inte in _client.Interfaces)
				inte.WlanNotification += inte_WlanNotification;
		}

        /// <summary>
        /// Get list of access points
        /// </summary>
        /// <returns>List of <see cref="AccessPoint"/> availables</returns>
        public List<AccessPoint> GetAccessPoints()
		{
            List<AccessPoint> accessPoints = new List<AccessPoint>();
            if (_client.NoWifiAvailable)
                return accessPoints;
			
			foreach (WlanInterface wlanIface in _client.Interfaces)
			{
				WlanAvailableNetwork[] rawNetworks = wlanIface.GetAvailableNetworkList(0);
				List<WlanAvailableNetwork> networks = new List<WlanAvailableNetwork>();

				// Remove network entries without profile name if one exist with a profile name.
				foreach (WlanAvailableNetwork network in rawNetworks)
				{
					bool hasProfileName						= !string.IsNullOrEmpty(network.profileName);
					bool anotherInstanceWithProfileExists	= rawNetworks.Where(n => n.Equals(network) && !string.IsNullOrEmpty(n.profileName)).Any();

					if (!anotherInstanceWithProfileExists || hasProfileName)
						networks.Add(network);
				}

				foreach (WlanAvailableNetwork network in networks)
				{
					accessPoints.Add(new AccessPoint(wlanIface, network));
				}
			}

			return accessPoints;
		}

		/// <summary>
		/// Disconnect all wifi interfaces
		/// </summary>
		public void Disconnect()
        {
            if (_client.NoWifiAvailable)
                return;

			foreach (WlanInterface wlanIface in _client.Interfaces)
			{
				wlanIface.Disconnect();
			}		
		}

        /// <summary>
        /// Get connection status <see cref="WifiStatus"/>
        /// </summary>
        public WifiStatus ConnectionStatus
		{
			get
			{
				if (!_isConnectionStatusSet)
					ConnectionStatus = GetForcedConnectionStatus();

				return _connectionStatus;
			}
			private set
			{
				_isConnectionStatusSet = true;
				_connectionStatus = value;
			}
		}

		private void inte_WlanNotification(WlanNotificationData notifyData)
		{
			if (notifyData.notificationSource == WlanNotificationSource.ACM && (NotifCodeACM)notifyData.NotificationCode == NotifCodeACM.Disconnected)
				OnConnectionStatusChanged(WifiStatus.Disconnected);
			else if (notifyData.notificationSource == WlanNotificationSource.MSM && (NotifCodeMSM)notifyData.NotificationCode == NotifCodeMSM.Connected)
				OnConnectionStatusChanged(WifiStatus.Connected);
		}

		private void OnConnectionStatusChanged(WifiStatus newStatus)
		{
			ConnectionStatus = newStatus;

			if (ConnectionStatusChanged != null)
				ConnectionStatusChanged(this, new WifiStatusEventArgs(newStatus));
		}

		// I don't like this method, it's slow, ugly and should be refactored ASAP.
        private WifiStatus GetForcedConnectionStatus()
        {
            if (NoWifiAvailable)
                return WifiStatus.Disconnected;

			bool connected = false;

			foreach (var i in _client.Interfaces)
			{
				try
				{
					var a = i.CurrentConnection; // Current connection throws an exception if disconnected.
					connected = true;
				}
				catch {	}
			}

			if (connected)
				return WifiStatus.Connected;
			else
				return WifiStatus.Disconnected;
		}		
	}

	/// <summary>
	/// Event from status changed
	/// </summary>
	public class WifiStatusEventArgs : EventArgs
	{
        /// <summary>
        /// Get new state <see cref="WifiStatus"/>
        /// </summary>
        public WifiStatus NewStatus { get; private set; }

		internal WifiStatusEventArgs(WifiStatus status) : base()
		{
			this.NewStatus = status;
		}

	}

	/// <summary>
	/// States connections
	/// </summary>
	public enum WifiStatus
	{
		/// <summary>
		/// Wifi is disconnected
		/// </summary>
		Disconnected,
		/// <summary>
		/// Wifi is connected
		/// </summary>
		Connected
	}
}
