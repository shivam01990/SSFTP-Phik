using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace SFTPGetConsole
{
    class Program
    {
        public static int Main()
        {
            Helper.AddtoLogFile("-------Program Starts Running----------");

            if (ConfigurationManager.AppSettings["ProgramType"].ToString() == "1")
            {
                Order.RunOrder();
            }
            if (ConfigurationManager.AppSettings["ProgramType"].ToString() == "2")
            {
                Shipment.RunShipment();
            }


            Helper.AddtoLogFile("-------Program Ends ----------");
            return 0;
        }
    }
}
