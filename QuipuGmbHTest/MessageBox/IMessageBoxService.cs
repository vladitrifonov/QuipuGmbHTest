using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuipuGmbHTest.MessageBox
{
    public interface IMessageBoxService
    {
        void ShowMessage(string text, string caption);
    }
}
