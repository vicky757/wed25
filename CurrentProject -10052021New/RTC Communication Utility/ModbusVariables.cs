using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTC_Communication_Utility
{
    public class ModbusVariables : INotifyPropertyChanged
    {
        private string _pv;
        public string PV
        {
            get { return _pv; }
            set
            {
                _pv = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("PV"));
            }
        }

        private string _sv;
        public string SV
        {
            get { return _sv; }
            set
            {
                _sv = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("SV"));
            }
        }

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }
        #endregion
    }
}
