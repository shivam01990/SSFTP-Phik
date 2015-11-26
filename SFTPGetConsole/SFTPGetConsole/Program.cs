﻿using System;
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
                    List<vw_importDownloadParameters> ImportFilePaths = DBProvider.Instance.DownloadParameters();
                    Helper.AddtoLogFile("All File Paths are download.");
                    foreach (vw_importDownloadParameters item in ImportFilePaths)
                    {
                        Helper.AddtoLogFile("Looping for " + item.gImportID);
                        Helper.AddtoLogFile("File Path " + item.filePath);
                        //RemoteDirectoryInfo directory = session.ListDirectory(ConfigurationManager.AppSettings["RemotePath"].ToString());
                        RemoteDirectoryInfo directory = session.ListDirectory(item.filePath);
                        foreach (RemoteFileInfo fileInfo in directory.Files)
                        {
                            Console.WriteLine("{0} with size {1}, permissions {2} and last modification at {3}",
                                fileInfo.Name, fileInfo.Length, fileInfo.FilePermissions, fileInfo.LastWriteTime);

                            string fileName = fileInfo.Name;
                            //string remotePath = ConfigurationManager.AppSettings["RemotePath"].ToString() + fileName;
                            string remotePath = item.filePath + fileName;
                            // string localPath = "d:\\backup\\" + fileName;
                            string localPath = ConfigurationManager.AppSettings["LocalPath"].ToString() + fileName;


                            // Manual "remote to local" synchronization.

                            // You can achieve the same using:
                            // session.SynchronizeDirectories(
                            //     SynchronizationMode.Local, localPath, remotePath, false, false, SynchronizationCriteria.Time, 
                            //     new TransferOptions { FileMask = fileName }).Check();
                            if (session.FileExists(remotePath))
                            {
                                bool download;
                                if (!File.Exists(localPath))
                                {
                                    Console.WriteLine("File {0} exists, local backup {1} does not", remotePath, localPath);
                                    Helper.AddtoLogFile(string.Format("File {0} exists, local backup {1} does not", remotePath, localPath));
                                    download = true;
                                }
                                else
                                {
                                    DateTime remoteWriteTime = session.GetFileInfo(remotePath).LastWriteTime;
                                    DateTime localWriteTime = File.GetLastWriteTime(localPath);

                                    if (remoteWriteTime > localWriteTime)
                                    {
                                        Console.WriteLine(
                                            "File {0} as well as local backup {1} exist, " +
                                            "but remote file is newer ({2}) than local backup ({3})",
                                            remotePath, localPath, remoteWriteTime, localWriteTime);
                                        download = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine(
                                            "File {0} as well as local backup {1} exist, " +
                                            "but remote file is not newer ({2}) than local backup ({3})",
                                            remotePath, localPath, remoteWriteTime, localWriteTime);
                                        download = false;
                                    }
                                }

                                if (download)
                                {
                                    // Download the file and throw on any error
                                    session.GetFiles(remotePath, localPath).Check();

                                    Console.WriteLine("Download to backup done.");
                                    Helper.AddtoLogFile("Download to backup done.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("File {0} does not exist yet", remotePath);
                            }
                        }
                    }
                }
                Helper.AddtoLogFile("-------Program End----------");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Helper.AddtoLogFile("-------Program End----------");
                return 1;
            }




        }
    }
}
