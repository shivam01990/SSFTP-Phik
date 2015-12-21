using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace SFTPGetConsole
{
    public class Shipment
    {
        public static int RunShipment()
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

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);
                    Helper.AddtoLogFile("-------Session instance successfully Created.----------");
                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;
                    transferOptions.PreserveTimestamp = false;
                    string ToUpload = ConfigurationManager.AppSettings["RemoteShipmentPath"].ToString();
                    List<SFTPmap> ImportFilePaths = DBProvider.Instance.DownloadParameters();
                    Helper.AddtoLogFile("All File Paths are download.");
                    foreach (SFTPmap item in ImportFilePaths)
                    {
                        if (!string.IsNullOrEmpty(item.FolderNameShip))
                        {
                            try
                            {
                                TransferOperationResult transferResult;
                                if (!item.FolderNameShip.EndsWith("\\"))
                                {
                                    item.FolderNameShip += "\\";
                                }

                                if (!ToUpload.EndsWith("/"))
                                {
                                    item.FolderNameShip += "/";
                                }

                                transferResult = session.PutFiles(item.FolderNameShip + "*", ToUpload+item.FolderNameAlias, false, transferOptions);
                                // Throw on any error
                                transferResult.Check();

                                // Print results
                                foreach (TransferEventArgs transfer in transferResult.Transfers)
                                {
                                    Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                                    Helper.AddtoLogFile("Upload of " + transfer.FileName + " succeeded");

                                }
                            }
                            catch (Exception ex) { Helper.AddtoLogFile("Error:" + ex.ToString()); }
                        }
                    }
                }
                Helper.AddtoLogFile("-------Program End----------");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Helper.AddtoLogFile("Error:" + e.ToString());
                Helper.AddtoLogFile("-------Program End----------");
                return 1;
            }

        }

    }
}
