﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace SFTPGetConsole
{
    public class Order
    {
        public static int RunOrder()
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

                                // during testing we put the orders in a subfolder + item.FolderName. for live we use the local path only. 
                                //LocalPath = LocalPath + item.FolderName + "\\";
                                //LocalPath = LocalPath;

                                foreach (RemoteFileInfo fileInfo in directory.Files)
                                {
                                    if (!Directory.Exists(LocalPath))
                                    {
                                        if (!File.Exists(LocalPath + fileInfo.Name))
                                        {
                                            Directory.CreateDirectory(LocalPath);
                                        }
                                    
                                        try
                                        {
                                            if (fileInfo.Name != "..")
                                            {
                                                // to delete the files on the commercehub site, change the next from false to true.
                                                session.GetFiles(RemotePath + "/" + fileInfo.Name, LocalPath + fileInfo.Name, true).Check();
                                                Console.WriteLine("File Tranfer successful from " + RemotePath + "/" + fileInfo.Name + " to " + LocalPath + fileInfo.Name);
                                                Helper.AddtoLogFile("File Tranfer successful from " + RemotePath + "//" + fileInfo.Name + " to " + LocalPath + fileInfo.Name);
                                            }
                                        }
                                        catch (Exception Ex)
                                        {
                                            Console.WriteLine("Error Occurs:" + Ex.ToString());
                                            Helper.AddtoLogFile("Error Occurs:" + Ex.ToString());
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
