using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace QuipuGmbHTest.MessageBox
{
    public class WPFMessageBoxService : IMessageBoxService
    {
        public void ShowMessage(string text, string caption)
        {
            System.Windows.Forms.MessageBox.Show(text, caption);
        }
    }
}
