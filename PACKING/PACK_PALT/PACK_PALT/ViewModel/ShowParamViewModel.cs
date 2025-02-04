using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACK_PALT.ViewModel
{
     public class ShowParamViewModel:BaseViewModel
    {
        private string _URLLabel = "";
        public string URLLabel { get { return _URLLabel; } set { _URLLabel = value; OnPropertyChanged("URLLabel"); } }

        public ShowParamViewModel()
        {

        }
    }
}
