using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ClassList
{
    public class SerialPortManager : IDisposable
    {
        private static readonly Lazy<SerialPortManager> lazy =
            new Lazy<SerialPortManager>(() => new SerialPortManager());

        private SerialPort _port = null;
        private string modbusStatus;
        private volatile bool _keepReading;
        static object _lock = new object();

        //private static Logger _logger = LogManager.GetCurrentClassLogger();

        public string PortName { get; set; } //= "COM1";
        public int Baudrate { get; set; } //= 9600;
        public int ConnectionTimeout { get; set; } //= 1000;
        public int ReadTimeout { get; set; } //= 1000;
        public int WriteTimeout { get; set; } //= 1000;
        public Parity Parity { get; set; } //= Parity.None;
        public StopBits StopBits { get; set; } //= StopBits.One;
        public int Databits { get; set; } //= 8;
        public byte UnitIdentifier { get; set; } //= 1;
        public bool IsConnected
        {
            get
            {
                return _port != null && _port.IsOpen ? true : false;
            }
        }

        #region Constructor
        private SerialPortManager()
        {
            if (_port == null)
            {
                lock(_lock){
                    _port = new SerialPort();
                }
            }
            // _readThread = null;
            _keepReading = false;
        }
        #endregion

        #region Instance
        public static SerialPortManager Instance { get { return  lazy.Value;  } }
        #endregion

        public bool Connect()
        {
            //Ensure port isn't already opened:
            if (!_port.IsOpen)
            {
                //Assign desired settings to the serial port:
                _port.PortName = PortName;
                _port.BaudRate = Baudrate;
                _port.DataBits = Databits;
                _port.Parity = Parity;
                _port.StopBits = StopBits;
                //These timeouts are default and cannot be editted through the class at this point:
                _port.ReadTimeout = ReadTimeout;
                _port.WriteTimeout = WriteTimeout;

                try
                {
                    _port.Open();
                }
                catch (Exception err)
                {
                    modbusStatus = "Error opening " + PortName + ": " + err.Message;
                    return false;
                }
                modbusStatus = PortName + " opened successfully";
                return true;
            }
            else
            {
                modbusStatus = PortName + " already opened";
                return false;
            }
        }

        public bool Disconnect()
        {
            //Ensure port is opened before attempting to close:
            if (_port.IsOpen)
            {
                try
                {
                    _port.Close();
                }
                catch (Exception err)
                {
                    modbusStatus = "Error closing " + _port.PortName + ": " + err.Message;
                    return false;
                }
                modbusStatus = _port.PortName + " closed successfully";
                return true;
            }
            else
            {
                modbusStatus = _port.PortName + " is not open";
                return false;
            }
            return false;
        }

        public bool SendReceiveByteArray(byte[] message, int index, int length)
        {
            byte[] recBuff = null;
            try
            {
                _port.Write(message, index, length);

                // number of bytes on stream to read
                int size = _port.BytesToRead;

                //// number of bytes greater then 0
                //if (size > 0)
                //{
                //    // create receive buffer 
                //    recBuff = new byte[_port.BytesToRead];

                //    // read data from stream
                //    int byteRead = _port.Read(recBuff, 0, recBuff.Length);
                //}
                //return recBuff;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal void DiscardInBuffer()
        {
            _port.DiscardInBuffer();
        }

        internal void DiscardOutBuffer()
        {
            _port.DiscardOutBuffer();
        }

        internal void GetResponse(ref byte[] response)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.
            for (int i = 0; i < response.Length; i++)
            {
                response[i] = (byte)(_port.ReadByte());
            }
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(_port != null)
                {
                _port.Dispose();
                }
                _port = null;
            }
        }
        #endregion
    }
}
