
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApplication1
{


    static class Program
    {


        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (false)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }

            Subtext.Scripting.ScriptSplitter scs = new Subtext.Scripting.ScriptSplitter(@"
SELECT * FROM T1
GO
SELECT * FROM T2
GO

GO

SELECT * FROM T3
GO
");
            foreach (string str in scs)
            {
                Console.WriteLine(str);
            }



            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(" --- Press any key to continue --- ");
            Console.ReadKey();
        }


    }


}
