/*===================================================================
//
// Copyright 2005-2006, Renu Electronics Pvt. Ltd., Pune, India.
// All Rights Reserved.
//
// The copyright above and this notice must be preserved in all
// copies of this source code.  The copyright above does not
// evidence any actual or intended publication of this source
// code.
//
// This is unpublished proprietary trade secret source code of
// Renu Electronics Pvt. Ltd.  This source code may not be copied,
// disclosed, distributed, demonstrated or licensed except as
// expressly authorized by Renu Electronics Pvt. Ltd.
//
// This source code in its entirity is developed by Renu Electronics 
// Pvt. Ltd
//
// File Name	Device.cs
// Author		Kapil Vyas
//=====================================================================
*/
using System;

namespace ClassList
{
	/// <summary>
	/// This is an abstract base class for communication classes, Serial and
    /// Ethernet. It declares the protected members and abstract methods needed
    /// for the communication.
	/// </summary>

	public abstract class Device
    {
        #region Constants
        protected const int     UP = 1;
        protected const int     DOWN = 0;

        protected const int     SUCCESS = CommonConstants.SUCCESS;
        protected const int     FAILURE = CommonConstants.FAILURE;
        protected const int ONLYBOOTBLOCK = CommonConstants.ONLYBOOTBLOCK;        

        protected const int     OPEN_EXISTING = 3;
        protected const uint    GENERIC_READ = 0x80000000;
        protected const uint    GENERIC_WRITE = 0x40000000;
        #endregion


        #region Protected Members
        protected uint	        _deviceLength;
		protected string        _deviceFileName;
		protected byte	        _deviceFileID;
		protected bool	        _deviceIsConnected;
		protected uint	        _deviceTotalFrames;
		public short	        _deviceProdID;
		protected int	        _devicePortNo;
		protected string        _deviceLastError;
        protected byte[] _arrdwnlsetupframe;
        #region FP_Ethernet_Implementation-AMIT
        private static CommonConstants.EthernetSettings _deviceEthernetSettings;
        #endregion
        protected bool          _deviceFileInUse;//FlexiSoft_IEC_Mngr_1255_AD
        public string strbootblock_Base = string.Empty; //Download_NewHardwareChnages_Files Vijay
		#endregion


        #region Constructor and destructor
        public Device( )
		{

		}


		~Device()
		{

        }
        #endregion


        public short DeviceProdID
        {
            get
            {
                return _deviceProdID;
            }
            set
            {
                _deviceProdID = value;
            }
        }
        #region Download_NewHardwareChnages_Files Vijay
        public string BootBlockVersion
        {
            get
            {
                return strbootblock_Base;
            }
            set
            {
                strbootblock_Base = value;
            }
        }
        #endregion
        #region FP_Ethernet_Implementation-AMIT
        /// <summary>
        /// Ethernet settings
        /// </summary>
        public static CommonConstants.EthernetSettings EthernetSettings
        {
            get
            {
                return _deviceEthernetSettings;
            }
            set
            {
                _deviceEthernetSettings = value;
            }
        }
        #endregion

        #region Abstract methods
        public abstract int Connect();
		public abstract int SendFile( string pFileName, byte pFileID );
        public abstract int ReceiveFile(string pFileName, int pFileID);
		public abstract void Close();
        #endregion
   }
}
