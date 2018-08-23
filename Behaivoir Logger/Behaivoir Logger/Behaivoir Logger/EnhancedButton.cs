using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Behaivoir_Logger
{
    class EnhancedButton : Button
    {
        public string userName
        {
            get;
            set;
        }
        public EnhancedButton(string username)
        {
            userName = username;
        }
        public EnhancedButton()
        {
            
        }
    }
}
