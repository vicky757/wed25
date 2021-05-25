using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ClassList
{
    public enum ModbusMode
    {
        ASCII,
        RTU,
    }

    public class ModbusClient
    {
        private string modbusStatus;
        public ModbusMode Mode { get; set; }
        public byte SlaveId { get; set; } //= 1;        
        public ushort StartingAddress { get; set; }
        public ushort NumOfRegisters { get; set; }


        public ModbusClient()
        {
            
        }

        #region CRC Computation
        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
        #endregion

        #region Build Message
        private void BuildMessage(byte address, byte type, ushort start, ushort registers, ref byte[] message)
        {
            //Array to receive CRC bytes:
            byte[] CRC = new byte[2];

            message[0] = address;
            message[1] = type;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)start;
            message[4] = (byte)(registers >> 8);
            message[5] = (byte)registers;

            GetCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
        }
        #endregion

        #region Check Response
        private bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check:
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }
        #endregion

        #region Get Response
        
        #endregion

        public void WriteMultipleRegisters(int startingAddress, int[] values)
        {
            
        }

        public int[] ReadHoldingRegisters()
        {
            int[] result = new int[NumOfRegisters];

            //Ensure port is open:
            if (SerialPortManager.Instance.IsConnected)
            {
                //Clear in/out buffers:
                SerialPortManager.Instance.DiscardOutBuffer();
                SerialPortManager.Instance.DiscardInBuffer();
         
                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];

                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * NumOfRegisters];

                //Build outgoing modbus message:
                BuildMessage(SlaveId, (byte)3, StartingAddress, NumOfRegisters, ref message);
                
                //Send modbus message to Serial Port:
                try
                {
                    SerialPortManager.Instance.SendReceiveByteArray(message, 0, message.Length);
                   SerialPortManager.Instance.GetResponse(ref response);
                }
                catch (Exception err)
                {
                    modbusStatus = "Error in read event: " + err.Message;
                    return result;
                }
                
                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        result[i] = response[2 * i + 3];
                        result[i] <<= 8;
                        result[i] += response[2 * i + 4];
                    }
                    modbusStatus = "Read successful";
                    return result;
                }
                else
                {
                    modbusStatus = "CRC error";
                    return result;
                }
            }
            else
            {
                modbusStatus = "Serial port not open";
                return result;
            }
        }


    }
}
