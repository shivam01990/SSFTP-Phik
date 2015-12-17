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
            try
            {
                Helper.AddtoLogFile("-------Program Starts Running----------");
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
                    List<SFTPmap> ImportFilePaths = DBProvider.Instance.DownloadParameters();
                    Helper.AddtoLogFile("All File Paths are download.");
                    foreach (SFTPmap item in ImportFilePaths)
                    {
                        if (!string.IsNullOrEmpty(item.FolderNameAlias))
                       {
                            try
                            {
                                Helper.AddtoLogFile("Looping for " + item.FolderNameAlias);
                                Helper.AddtoLogFile("File Path " + item.FilePath);

                                string LocalPath = item.FilePath;
                                string RemotePath = ConfigurationManager.AppSettings["RemotePath"].ToString() + item.FolderNameAlias;

                                RemoteDirectoryInfo directory = session.ListDirectory(RemotePath);
                                LocalPath = LocalPath + item.FolderName + "\\";

                                foreach (RemoteFileInfo fileInfo in directory.Files)
                                {
                                    if (!Directory.Exists(LocalPath))
                                    {
                                        Directory.CreateDirectory(LocalPath);
                                    }

                                    if (!File.Exists(LocalPath + fileInfo.Name))
                                    {
                                        try
                                        {
                                            if (fileInfo.Name != "..")
                                            {
                                                session.GetFiles(RemotePath + "/" + fileInfo.Name, LocalPath + fileInfo.Name).Check();
                                                Console.WriteLine("File Tranfer successful from " + RemotePath + "/" + fileInfo.Name + " to " + LocalPath + fileInfo.Name);
                                                Helper.AddtoLogFile("File Tranfer successful from " + RemotePath + "//" + fileInfo.Name + " to " + LocalPath + fileInfo.Name);
                                            }
                                        }
                                        catch (Exception Ex)
                                        {
                                            Console.WriteLine("Error Occurse:" + Ex.ToString());
                                            Helper.AddtoLogFile("Error Occurse:" + Ex.ToString());
                                        }
                                    }
                                }
                                // Upload files -  

                                //TransferOptions transferOptions = new TransferOptions();
                                //transferOptions.TransferMode = TransferMode.Binary;

                                //TransferOperationResult transferResult;
                                //transferResult = session.GetFiles(RemotePath + "*", LocalPath, false, transferOptions);

                                //// Throw on any error  
                                //transferResult.Check();

                                //// Print results
                                //foreach (TransferEventArgs transfer in transferResult.Transfers)
                                //{
                                //    Console.WriteLine("Download of {0} succeeded", transfer.FileName);
                                //    Helper.AddtoLogFile("Download of " + transfer.FileName + " succeeded");
                                //}

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
