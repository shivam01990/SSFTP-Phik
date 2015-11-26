using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace SSFTPConsole
{
    class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                // Setup session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = ConfigurationManager.AppSettings["HostName"].ToString(),
                    UserName = ConfigurationManager.AppSettings["UserName"].ToString(),
                    Password = ConfigurationManager.AppSettings["Password"].ToString(),
                    SshHostKeyFingerprint = ConfigurationManager.AppSettings["SshHostKeyFingerprint"].ToString()
                };
                string LocalPath = ConfigurationManager.AppSettings["LocalPath"].ToString();
                string RemotePath = ConfigurationManager.AppSettings["RemotePath"].ToString();
                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    transferResult = session.PutFiles(LocalPath + "*", RemotePath, false, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                    }
                }
               
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Console.ReadKey();
                return 1;
            }
           
        }
    }
}
