using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Severstal.ErrorHandler
{
    class Error
    {
        public Error(Exception ex)
        {
            ShowMessage(ex);
        }
        public void ShowMessage(Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
