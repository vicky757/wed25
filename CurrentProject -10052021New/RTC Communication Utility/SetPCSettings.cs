using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO.Ports;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassList;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;

namespace RTC_Communication_Utility
{
    public partial class SetPCSettings : Form
    {

        private string [] Usblist;
        private bool validConfig = false;
        DataSet dsFile = new DataSet();
        SerialPort _port = null;
        string m_exePath1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
               "\\DefaultSettings.xml";

        string rampSoak = "";

        clsModbus objmob = new clsModbus();
        public SetPCSettings()
        {
            _port = new SerialPort();
            InitializeComponent();
        }

        public string[] RemoveVirtualPorts(string[] portList)
        {
            string Vport1 = "";
            string Vport2 = "";
            List<string> list = new List<string>(portList);
            string[] NewPortList = new string[0];

            ClassList.CommonConstants.checkVPSerialPort(ref Vport1, ref Vport2);
            for (int i = 0; i < list.Count; )
            {
                if (list[i] == Vport1.ToUpper() || list[i] == Vport2.ToUpper())
                {
                    list.RemoveAt(i);
                    i = 0;
                }
                else
                    i++;
            }

            NewPortList = list.ToArray();

            return NewPortList;
        }

        private void SetPCSettings_Load(object sender, EventArgs e)
        {

            lblErrorMsg.Visible = false;
            try
            {
                ExceptionHelper.LogFile("Form loaded (S)", "SetPCSettings_Load", "(F)", 0, this.FindForm().Name, "SetPCSettings.txt");

                string[] ports = SerialPort.GetPortNames();

                cmbBxPorts.Items.Add("--Select--");

                foreach (string prt in ports)
                {
                    cmbBxPorts.Items.Add(prt);
                }

                cmbBxBaudrate.DataSource = BasicProperties.baudRates;
                cmbBxDataLength.DataSource = BasicProperties.dataLengths;
                cmbBxParityBit.DataSource = BasicProperties.parity;
                cmbBxStopBits.DataSource = BasicProperties.stopBits;
                cmbBxMode.DataSource = BasicProperties.protocol;

                cmbBxMode.SelectedIndex = 0;
                cmbBxPorts.SelectedIndex = 0;

                //check if file exists or not
                if (CheckIfFileExists1())  //  if (CheckIfFileExists())
                {
                    try
                    {

                        BindDefaultSettings();

                        ExceptionHelper.LogFile("Default settings bind (S)", "SetPCSettings_Load", "(F)", 0, this.FindForm().Name, "SetPCSettings.txt");

                        #region oldData
                        //XElement xelement = XElement.Load(m_exePath1);
                        //IEnumerable<XElement> settings = xelement.Descendants("settings");
                        //if (settings.Count() > 0)
                        //{
                        //    foreach (XElement item in settings)
                        //    {
                        //        switch (item.Name.ToString().ToLower())
                        //        {
                        //            case "port":
                        //                cmbBxPorts.SelectedIndex = string.IsNullOrEmpty(item.Value) ?
                        //        0 : Convert.ToInt32(item.Value);
                        //                break;

                        //            case "baudrate":
                        //                cmbBxBaudrate.SelectedIndex = string.IsNullOrEmpty(item.Value) ?
                        //       0 : Convert.ToInt32(item.Value);

                        //                break;
                        //            case "parity":
                        //                cmbBxParityBit.SelectedIndex = string.IsNullOrEmpty(item.Value) ?
                        //         0 : Convert.ToInt32(item.Value);
                        //                break;

                        //            case "datalength":
                        //                cmbBxDataLength.SelectedIndex = string.IsNullOrEmpty(item.Value) ?
                        //        0 : Convert.ToInt32(item.Value);
                        //                break;

                        //            case "stopbits":
                        //                cmbBxStopBits.SelectedIndex = string.IsNullOrEmpty(item.Value) ?
                        //        0 : Convert.ToInt32(item.Value);
                        //                break;
                        //            case "mode":
                        //                cmbBxMode.SelectedIndex = string.IsNullOrEmpty(item.Value) ?
                        //        0 : Convert.ToInt32(item.Value);
                        //                break;
                        //        }

                        //    }


                        //}
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper.LogFile(ex.Message, e.ToString(), ((Control)sender).Name, ex.LineNumber(), this.FindForm().Name, null);
                    }
                }
                else
                {
                    lblErrorMsg.Visible = true;
                    lblErrorMsg.Text = "File not found.";
                    ExceptionHelper.LogFile("Settings file not found", "SetPCSettings_Load", "(F)", 0, this.FindForm().Name, "SetPCSettings.txt");
                }
            }
            catch (Exception ex)
            {
                //call LogFile method and pass argument as Exception message, event name, controlname, error line number, current form name
                ExceptionHelper.LogFile(ex.Message, e.ToString(), ((Control)sender).Name, ex.LineNumber(), this.FindForm().Name, null);
            }
        }

        private void BindDefaultSettings()
        {
            try
            {
                dsFile.ReadXml(m_exePath1);

                if (dsFile.Tables.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt = dsFile.Tables["settings"];

                    if (dt.Rows.Count > 0)
                    {
                        cmbBxBaudrate.SelectedIndex = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["baudrate"])) ? -1 : Convert.ToInt32(Convert.ToString(dt.Rows[0]["baudrate"]));
                        cmbBxDataLength.SelectedIndex = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["datalength"])) ? -1 : Convert.ToInt32(Convert.ToString(dt.Rows[0]["datalength"]));
                        cmbBxMode.SelectedIndex = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["mode"])) ? -1 : Convert.ToInt32(Convert.ToString(dt.Rows[0]["mode"]));
                        cmbBxParityBit.SelectedIndex = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["parity"])) ? -1 : Convert.ToInt32(Convert.ToString(dt.Rows[0]["parity"]));
                        cmbBxPorts.SelectedIndex = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["port"])) ? -1 : (cmbBxPorts.Items.Count > Convert.ToInt32(Convert.ToString(dt.Rows[0]["port"])) ? Convert.ToInt32(Convert.ToString(dt.Rows[0]["port"])) : 0);
                        cmbBxStopBits.SelectedIndex = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["stopbits"])) ? -1 : Convert.ToInt32(Convert.ToString(dt.Rows[0]["stopbits"]));
                        SetValues.Set_CommType = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["commType"])) ? 1 : Convert.ToInt32(dt.Rows[0]["commType"]);
                        SetValues.Set_Release = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["release"])) ? "0" : Convert.ToString(dt.Rows[0]["release"]);
                        btnAutoset.Enabled = SetValues.Set_CommType == 1 ? true : false;
                        ExceptionHelper.LogFile("file1 records bind (S)", "BindDefaultSettings", "(F)", 0, this.FindForm().Name, "SetPCSettings.txt");
                    }

                    if (SetValues.Set_CommType == 1)
                    {
                        rbdSerial.Checked = true;
                    }
                    else if (SetValues.Set_CommType == 2)
                    {
                        rbdUsb.Checked = true;
                    }

                    DataTable dt1 = new DataTable();
                    dt1 = dsFile.Tables["rampSoak"];

                    if (dt1.Rows.Count > 0)
                    {
                        rampSoak = string.IsNullOrEmpty(Convert.ToString(dt1.Rows[0]["start"])) ? "-1" : Convert.ToString(dt1.Rows[0]["start"]);
                    }
                    else
                    {
                        ExceptionHelper.LogFile("no rampsoak tables present", "BindDefaultSettings", "(F)", 0, this.FindForm().Name, "SetPCSettings.txt");
                    }
                }
                else
                {
                    ExceptionHelper.LogFile("no settings tables present", "BindDefaultSettings", "(F)", 0, this.FindForm().Name, "SetPCSettings.txt");
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogFile(ex.Message, "BindDefaultSettings", "(F)", ex.LineNumber(), this.FindForm().Name, null);
            }
        }

        private void btnAutoset_Click(object sender, EventArgs e)
        {
            try
            {
                string portName = cmbBxPorts.SelectedIndex == 0 ? null : cmbBxPorts.Text;

                if (string.IsNullOrEmpty(portName))
                {
                  //  lblErrorMsg.Text = "Please select COM port";
                    MessageBox.Show("Please select COM port");
                }
                else
                {
                    //SetValues.Set_PortName = cmbBxPorts.Text;
                    string comName = cmbBxPorts.SelectedItem.ToString() ;

                    this.Hide();

                    //using (AutoSetForm autoSetForm = new AutoSetForm())
                    using (Form11 autoSetForm = new Form11())
                    {
                        autoSetForm.comName = comName;

                        DialogResult dr = autoSetForm.ShowDialog(this);

                        if (dr == DialogResult.Cancel)
                        {
                            autoSetForm.Close();
                        }
                        else if (dr == DialogResult.OK)
                        {
                            string[] parameters = autoSetForm.getText();
                            if (parameters.Length > 0)
                            {
                                cmbBxBaudrate.SelectedIndex = Array.IndexOf(BasicProperties.baudRates, parameters[0]);
                                cmbBxParityBit.SelectedIndex = Array.IndexOf(BasicProperties.parity, parameters[1]);
                                cmbBxStopBits.SelectedIndex = Array.IndexOf(BasicProperties.stopBits, parameters[2]);
                                cmbBxDataLength.SelectedIndex = Array.IndexOf(BasicProperties.dataLengths, parameters[3]);
                                cmbBxMode.SelectedIndex = Array.IndexOf(BasicProperties.protocol, parameters[4]);
                            }
                            autoSetForm.Close();
                        }
                        this.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogFile(ex.Message, e.ToString(), ((Control)sender).Name, ex.LineNumber(), this.FindForm().Name, null);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbBxPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAllDropdownSelectedOrNot();

            if (cmbBxPorts.SelectedIndex > 0)
            {
                SetValues.Set_PortName = cmbBxPorts.SelectedItem.ToString();
                validConfig = true;
            }
        }

        private void cmbBxBaudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAllDropdownSelectedOrNot();

            if (cmbBxBaudrate.SelectedIndex > 0)
            {
                SetValues.Set_Baudrate = cmbBxBaudrate.SelectedItem.ToString();
                validConfig = true;
            }
        }

        private void cmbBxParityBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAllDropdownSelectedOrNot();

            if (cmbBxParityBit.SelectedIndex > 0)
            {
                SetValues.Set_parity = cmbBxParityBit.SelectedItem.ToString();
            }
        }

        private void cmbBxDataLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAllDropdownSelectedOrNot();

            if (cmbBxDataLength.SelectedIndex > 0)
            {
                SetValues.Set_BitsLength = Convert.ToInt32(cmbBxDataLength.SelectedItem.ToString());
            }
        }

        private void cmbBxStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAllDropdownSelectedOrNot();

            if (cmbBxStopBits.SelectedIndex > 0)
            {
                SetValues.Set_StopBits = cmbBxStopBits.SelectedItem.ToString();
            }
        }

        private bool CheckControls()
        {
            validConfig = false;
            if (rbdSerial.Checked)
            {
                validConfig = (cmbBxPorts.SelectedIndex == 0) ? false : true;
                validConfig = (cmbBxBaudrate.SelectedIndex == 0) ? false : true;
                validConfig = (cmbBxParityBit.SelectedIndex == 0) ? false : true;
                validConfig = (cmbBxDataLength.SelectedIndex == 0) ? false : true;
                validConfig = (cmbBxStopBits.SelectedIndex == 0) ? false : true;
                validConfig = (cmbBxMode.SelectedIndex == 0) ? false : true;
               
                return validConfig;
            }
            else if (rbdUsb.Checked)
            {
                validConfig = true;
            }

            return validConfig;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckControls())
                {
                    if (ReadWriteFile1())
                    {
                        Messages(3);
                        SetValues.Set_CommType = rbdSerial.Checked ? 1 : (rbdUsb.Checked ? 2 : 1);
                        
                    }
                    this.DialogResult = DialogResult.OK;
                    if(Usblist.Length > 1)
                    {
                       SetValues.Set_PortName = cmbBxPorts.SelectedItem.ToString();
                    }
                    this.Close();
                }
                else
                {
                    Messages(2);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogFile(ex.Message, e.ToString(), ((Control)sender).Name, ex.LineNumber(), this.FindForm().Name, null);
            }
        }

        private DataSet CreateDynamicDataSet(string port, string baud, string parity, string databits, string stopbits, string mode, string commType, string release, bool ramp)
        {
            try
            {
                DataSet ds = new DataSet("Default");

                DataTable stdTable = new DataTable("settings");
                DataColumn col1 = new DataColumn("port");
                DataColumn col2 = new DataColumn("baudrate");
                DataColumn col3 = new DataColumn("parity");
                DataColumn col4 = new DataColumn("datalength");
                DataColumn col5 = new DataColumn("stopbits");
                DataColumn col6 = new DataColumn("mode");
                DataColumn col7 = new DataColumn("commType");
                DataColumn col8 = new DataColumn("release");

                stdTable.Columns.Add(col1);
                stdTable.Columns.Add(col2);
                stdTable.Columns.Add(col3);
                stdTable.Columns.Add(col4);
                stdTable.Columns.Add(col5);
                stdTable.Columns.Add(col6);
                stdTable.Columns.Add(col7);
                stdTable.Columns.Add(col8);

                ds.Tables.Add(stdTable);

                //Add student Data to the table  
                DataRow newRow, newRow1; newRow = stdTable.NewRow();

                newRow["port"] = Convert.ToString(cmbBxPorts.SelectedIndex);
                newRow["baudrate"] = Convert.ToString(cmbBxBaudrate.SelectedIndex);
                newRow["parity"] = Convert.ToString(cmbBxParityBit.SelectedIndex);
                newRow["datalength"] = Convert.ToString(cmbBxDataLength.SelectedIndex);
                newRow["stopbits"] = Convert.ToString(cmbBxStopBits.SelectedIndex);
                newRow["mode"] = Convert.ToString(cmbBxMode.SelectedIndex);
                newRow["commType"] = commType;
                newRow["release"] = release;

                stdTable.Rows.Add(newRow);


                DataTable rTable = new DataTable("rampSoak");
                DataColumn rCol1 = new DataColumn("start");
                rTable.Columns.Add(rCol1);
                ds.Tables.Add(rTable);
                newRow1 = rTable.NewRow();
                newRow1["start"] = ramp ? rampSoak : "-1";
                rTable.Rows.Add(newRow1);

                ds.AcceptChanges();
                return ds;
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogFile(ex.Message, "CreateDynamicDataSet", "(F)", ex.LineNumber(), this.FindForm().Name, null);
                return null;
            }

        }

        private bool CheckIfFileExists1()
        {
            if (File.Exists(m_exePath1))
            {
                return true;
            }
            else
            {
                try
                {
                    //create file
                    File.Create(m_exePath1).Dispose();

                    LogWriter.WriteToFile("SetPCSettings.cs => SaveToFile() - Created", "Path: " +
                        m_exePath1, "RTC_Upgrade");

                    try
                    {

                        DataSet ds1 = CreateDynamicDataSet("0", "0", "0", "0", "0", "0", "1", "0", false);
                        ds1.WriteXml(m_exePath1);

                        LogWriter.WriteToFile("SetPCSettings.cs => ReadWriteFile()", "Write complete "
                            , "RTC_Upgrade");

                        return true;
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper.LogFile(ex.Message, "CheckIfFileExists1", "(F)", ex.LineNumber(), this.FindForm().Name, null);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHelper.LogFile(ex.Message, "CheckIfFileExists1", "(F)", ex.LineNumber(), this.FindForm().Name, null);
                    return false;
                }

            }
        }

        private bool CheckIfFileExists()
        {
            if (!File.Exists(m_exePath1))
            {
                try
                {
                    //create file
                    File.Create(m_exePath1).Dispose();

                    LogWriter.WriteToFile("SetPCSettings.cs => SaveToFile() - Created", "Path: " +
                        m_exePath1, "RTC_Upgrade");

                    try
                    {
                        XNamespace empNM = "urn:lst-settings:settings";

                        XDocument xDoc = new XDocument(
                                    new XDeclaration("1.0", "UTF-16", null),
                                     new XElement("Default",
                                        new XElement("settings",
                                            new XElement("port", "1"),
                                            new XElement("baudrate", "1"),
                                            new XElement("parity", "1"),
                                            new XElement("datalength", "1"),
                                            new XElement("stopbits", "1"),
                                            new XElement("mode", "1"),
                                            new XElement("commType", "1"),
                                            new XElement("release", "0")
                                            )));

                        xDoc.Save(m_exePath1);
                        LogWriter.WriteToFile("SetPCSettings.cs => ReadWriteFile()", "Write complete "
                            , "RTC_Upgrade");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper.LogFile(ex.Message, "CheckIfFileExists", "(F)", ex.LineNumber(), this.FindForm().Name, null);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHelper.LogFile(ex.Message, "CheckIfFileExists", "(F)", ex.LineNumber(), this.FindForm().Name, null);
                    return false;
                }

            }
            return false;
        }

        private bool ReadWriteFile1()
        {
            //check if file exists or not
            if (CheckIfFileExists1()) //  if (CheckIfFileExists())
            {
                //read settings from file
                LogWriter.WriteToFile("SetPCSettings.cs => SaveToFile() - Write", "Path: " + m_exePath1
                        , "RTC_Upgrade"); // m_exePath1

                //to check if file is empty or it contains data to modify
                try
                {
                    string commType = string.Empty;
                    string modeType = string.Empty;
                    if (rbdSerial.Checked)
                    {
                        commType = "1"; // serial
                        modeType = Convert.ToString(cmbBxMode.SelectedIndex);  // ASCII/ RTU
                        SetValues.Set_BoolUSBdata = false;
                    }
                    else if (rbdUsb.Checked)
                    {
                        commType = "2"; // usb
                        modeType = "2"; // RTU
                        SetValues.Set_BoolUSBdata = true;
                        SetValues.Set_CommunicationProtocol = "RTU";
                        SetValues.Set_CommunicationProtocolindex = 2;
                        string[] list = Serial.GetUsbSerDevices();
                        Usblist = list;
                        string portName = string.Empty;

                        if (list == null || list.Length == 0)
                        {
                            //Debug.Fail("No ports available");
                            MessageBox.Show("No USB devices found!!");
                        }
                        else
                        {
                            portName = Convert.ToString(list[0]);

                            if (string.IsNullOrEmpty(portName))
                            {
                                MessageBox.Show("Check connection");
                                return false;
                            }
                            else
                            {
                                SetValues.Set_PortName = portName;
                            }
                        }


                    }

                    string release = string.Empty;

                    try
                    {
                        release = string.IsNullOrEmpty(SetValues.Set_Release.ToString()) ? "0" : SetValues.Set_Release.ToString();
                    }
                    catch (Exception)
                    {
                        release = "0";
                    }

                    //modify
                    try
                    {
                        DataSet ds1 = CreateDynamicDataSet(Convert.ToString(cmbBxPorts.SelectedIndex),
                           Convert.ToString(cmbBxBaudrate.SelectedIndex),
                           Convert.ToString(cmbBxParityBit.SelectedIndex),
                            Convert.ToString(cmbBxDataLength.SelectedIndex),
                            Convert.ToString(cmbBxStopBits.SelectedIndex),
                            modeType, commType, release, true);

                        ds1.WriteXml(m_exePath1);

                        LogWriter.WriteToFile("SetPCSettings.cs => ReadWriteFile()", "Modify complete "
                            , "RTC_Upgrade");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper.LogFile(ex.Message, "ReadWriteFile1", "(F)", ex.LineNumber(), this.FindForm().Name, null);
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    ExceptionHelper.LogFile(ex.Message, "ReadWriteFile1", "(F)", ex.LineNumber(), this.FindForm().Name, null);
                    return false;
                }
            }
            return false;
        }

        private bool ReadWriteFile()
        {
            //check if file exists or not
            if (CheckIfFileExists())
            {
                //read settings from file
                LogWriter.WriteToFile("SetPCSettings.cs => SaveToFile() - Write", "Path: " + "E:\\Students11.xml"
                        , "RTC_Upgrade"); // m_exePath1

                //to check if file is empty or it contains data to modify
                try
                {
                    XElement xelement = XElement.Load("E:\\Students11.xml");
                    IEnumerable<XElement> settings = xelement.Elements("Default");

                    if (settings.Count() > 0)
                    {
                        //modify
                        try
                        {
                            XNamespace empNM = "urn:lst-settings:settings";

                            XDocument xDoc = new XDocument(
                                        new XDeclaration("1.0", "UTF-16", null),
                                         new XElement("Default",
                                            new XElement("settings",
                                                new XElement("port", cmbBxPorts.SelectedIndex.ToString()),
                                                new XElement("baudrate", cmbBxBaudrate.SelectedIndex.ToString()),
                                                new XElement("parity", cmbBxParityBit.SelectedIndex.ToString()),
                                                new XElement("datalength", cmbBxDataLength.SelectedIndex.ToString()),
                                                new XElement("stopbits", cmbBxStopBits.SelectedIndex.ToString()),
                                                new XElement("mode", cmbBxMode.SelectedIndex.ToString()),
                                                 new XElement("commType", cmbBxMode.SelectedIndex.ToString()),
                                                 new XElement("release", "1")
                                                ))
                                                );

                            xDoc.Save(m_exePath1);
                            LogWriter.WriteToFile("SetPCSettings.cs => ReadWriteFile()", "Modify complete "
                                , "RTC_Upgrade");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            ExceptionHelper.LogFile(ex.Message, "ReadWriteFile", "(F)", ex.LineNumber(), this.FindForm().Name, null);
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHelper.LogFile(ex.Message, "ReadWriteFile", "(F)", ex.LineNumber(), this.FindForm().Name, null);
                    return false;
                }
            }
            return false;
        }

        public void GetAutoDetectSettings()
        {

        }

        private void cmbBxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAllDropdownSelectedOrNot();

            if (cmbBxMode.SelectedIndex > 0)
            {
                SetValues.Set_CommunicationProtocol = cmbBxMode.SelectedItem.ToString();
                SetValues.Set_CommunicationProtocolindex = cmbBxMode.SelectedIndex;

                if (cmbBxMode.SelectedIndex == 2) // RTU
                {
                    //disable datalength  and set it default to 8 bits
                    cmbBxDataLength.SelectedIndex = 2; // 8 bits
                    cmbBxDataLength.Enabled = false;
                    SetValues.Set_BitsLength = Convert.ToInt32(cmbBxDataLength.SelectedItem.ToString());
                }
                else
                    cmbBxDataLength.Enabled = true;
            }
        }

        private void CheckAllDropdownSelectedOrNot()
        {
            if (rbdSerial.Checked)
            {

                if (cmbBxBaudrate.SelectedIndex > 0 && cmbBxParityBit.SelectedIndex > 0 &&
                    cmbBxDataLength.SelectedIndex > 0 && cmbBxStopBits.SelectedIndex > 0 &&
                    cmbBxMode.SelectedIndex > 0 && cmbBxPorts.SelectedIndex > 0
                    )
                {
                    btnOK.Enabled = true;
                    Messages(0);
                }
                else
                {
                    btnOK.Enabled = false;
                    Messages(1);
                }
            }
        }

        private void Messages(int id)
        {
            switch (id)
            {
                case 0:
                    lblErrorMsg.Text = "";
                    break;
                case 1:
                    lblErrorMsg.Text = "Please select default settings.";
                    break;
                case 2:
                    lblErrorMsg.Text = "Something went wrong!!";
                    break;
            }
        }

        private void SetPCSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_port.IsOpen)
                _port.Close();

            _port = null;
        }

        private void rbdSerial_CheckedChanged(object sender, EventArgs e)
        {
            if (rbdSerial.Checked)
            {
                label2.Enabled = true;
                label3.Enabled = true;
                label4.Enabled = true;
                label5.Enabled = true;
                label7.Enabled = true;
                cmbBxBaudrate.Enabled = true;
                cmbBxParityBit.Enabled = true;
                cmbBxDataLength.Enabled = true;
                cmbBxStopBits.Enabled = true;
                cmbBxMode.Enabled = true;
                btnAutoset.Enabled = false;

                rbdUsb.Checked = false;

                //set 
                if (CheckControls())
                {
                    btnOK.Enabled = btnAutoset.Enabled = groupBox1.Enabled = true;
                }
                else
                {
                    btnOK.Enabled = btnAutoset.Enabled = groupBox1.Enabled = true;
                }
            }
        }

        private void rbdUsb_CheckedChanged(object sender, EventArgs e)
        {
            if (rbdUsb.Checked)
            {
                rbdSerial.Checked = false;

        // set
                btnAutoset.Enabled = false;
                //groupBox1.Enabled = true;

                label2.Enabled = false;
                label3.Enabled = false;
                label4.Enabled = false;
                label5.Enabled = false;
                label7.Enabled = false;
                cmbBxBaudrate.Enabled = false;
                cmbBxParityBit.Enabled = false;
                cmbBxDataLength.Enabled = false;
                cmbBxStopBits.Enabled = false;
                cmbBxMode.Enabled = false;
        

        btnOK.Enabled = true;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("devmgmt.msc");
            }
            catch (Exception ex)
            {

            }
        }
    }
}
