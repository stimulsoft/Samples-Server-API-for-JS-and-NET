using System;
using System.Windows.Forms;

namespace ShowAllWorkspaces
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ShowAllWorkspacesForm());
        }
    }
}
