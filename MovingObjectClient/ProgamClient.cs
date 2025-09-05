using System;
using System.Windows.Forms;

namespace MovingObjectClient
{
    static class ProgramClient
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormClient());
        }
    }
}
