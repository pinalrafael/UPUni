﻿using WifiConnection.Win32;
using WifiConnection.Win32.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WifiConnection
{
	/// <summary>
	/// Class from auth wifi
	/// </summary>
	public class AuthRequest
	{
		/// <summary>
		/// Return true if password is required
		/// </summary>
        public bool IsPasswordRequired { get { return _isPasswordRequired; } }
        /// <summary>
        /// Return true if user name is required
        /// </summary>
        public bool IsUsernameRequired { get { return _isUsernameRequired; } }
        /// <summary>
        /// Return true if domain is supported
        /// </summary>
        public bool IsDomainSupported { get { return _isDomainSupported; } }

		/// <summary>
		/// Get and set password
		/// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Get and set user name
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        /// <summary>
        /// Get and set domain
        /// </summary>
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        /// <summary>
        /// Return true if password is valid
        /// </summary>
        public bool IsPasswordValid
        {
            get
            {
				#warning Robin: Not sure that Enterprise networks have the same requirements on the password complexity as standard ones.
                return PasswordHelper.IsValid(_password, _network.dot11DefaultCipherAlgorithm);
            }
        }

        private bool _isPasswordRequired, _isUsernameRequired, _isDomainSupported, _isEAPStore;
		private string _password, _username, _domain;
		private WlanAvailableNetwork _network { get; set; }
		private WlanInterface _interface { get; set; }

        /// <summary>
        /// Start mew auth request
        /// </summary>
        /// <param name="ap">Access point to request <see cref="AccessPoint"/></param>
        public AuthRequest(AccessPoint ap)
		{	
			_network	= ap.Network;
			_interface	= ap.Interface;

			_isPasswordRequired = 
				_network.securityEnabled &&
				_network.dot11DefaultCipherAlgorithm != Dot11CipherAlgorithm.None;

			_isEAPStore =
				_network.dot11DefaultAuthAlgorithm == Dot11AuthAlgorithm.RSNA ||
				_network.dot11DefaultAuthAlgorithm == Dot11AuthAlgorithm.WPA;

			_isUsernameRequired = _isEAPStore;
			_isDomainSupported	= _isEAPStore;
		}
		
		private bool SaveToEAP() 
		{
			if (!_isEAPStore || !IsPasswordValid)
				return false;

			string userXML = EapUserFactory.Generate(_network.dot11DefaultCipherAlgorithm, _username, _password, _domain);
			_interface.SetEAP(_network.profileName, userXML);

			return true;		
		}

		internal bool Process()
		{
			if (!IsPasswordValid)
				return false;
			
			string profileXML = ProfileFactory.Generate(_network, _password);
			_interface.SetProfile(WlanProfileFlags.AllUser, profileXML, true);

			if (_isEAPStore && !SaveToEAP())
				return false;			
			
			return true;
		}
	}

	/// <summary>
	/// Class helper password
	/// </summary>
	public static class PasswordHelper
	{
        /// <summary>
        /// Checks if a password is valid for a cipher type.
        /// </summary>
        /// <param name="password">String password</param>
        /// <param name="cipherAlgorithm">Algorithm password <see cref="Dot11CipherAlgorithm"/></param>
        /// <returns></returns>
        public static bool IsValid(string password, Dot11CipherAlgorithm cipherAlgorithm)
		{
			switch (cipherAlgorithm)
			{
				case Dot11CipherAlgorithm.None:
					return true;
				case Dot11CipherAlgorithm.WEP: // WEP key is 10, 26 or 40 hex digits long.
					if (string.IsNullOrEmpty(password))
						return false;

					int len = password.Length;

					bool correctLength = len == 10 || len == 26 || len == 40;
					bool onlyHex = new Regex("^[0-9A-F]+$").IsMatch(password);

					return correctLength && onlyHex;
				case Dot11CipherAlgorithm.CCMP: // WPA2-PSK 8 to 63 ASCII characters					
				case Dot11CipherAlgorithm.TKIP: // WPA-PSK 8 to 63 ASCII characters
					if (string.IsNullOrEmpty(password))
						return false;

					return 8 <= password.Length && password.Length <= 63;
				default:
					return true;
			}
		}
	}
}
