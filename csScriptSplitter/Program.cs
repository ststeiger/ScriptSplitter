
using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace csScriptSplitter
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

SELECT '''bla GO ' FROM T4
GO


SELECT * FROM T3
GO
");
            foreach (string str in scs)
            {
                Console.WriteLine(str);
            }


            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.DataSource = "localhost";
            csb.InitialCatalog="pdns";
            csb.IntegratedSecurity=true;



            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(csb.ConnectionString);

            System.Data.SqlClient.SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = @"
INSERT INTO Domains
(
	 name
	,master
	,last_check
	,type
	,notified_serial
	,account
)
OUTPUT Inserted.id
VALUES
(
	 CAST(newid() as varchar(36)) -- name varchar(255),>
	,'master' -- master varchar(128),>
	,'' -- last_check int,>
	,'type' -- type varchar(6),>
	,'123' -- notified_serial int,>
	,'account' -- account varchar(40),>
)
;

";

            if (con.State != System.Data.ConnectionState.Open)
                con.Open();

            object bla = cmd.ExecuteScalar();
            Console.WriteLine(bla);

            if(con.State != System.Data.ConnectionState.Closed)
                con.Close();



            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(" --- Press any key to continue --- ");
            Console.ReadKey();
        }


    }


}
