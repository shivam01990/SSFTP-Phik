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
                    List<vw_importSFTP> ImportFilePaths = DBProvider.Instance.DownloadParameters();
                    Helper.AddtoLogFile("All File Paths are download.");
                    foreach (vw_importSFTP item in ImportFilePaths)
                    {
                        if (!string.IsNullOrEmpty(item.FolderName))
                        {
                            try
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
                                    string FolderName = "";
                                    if (string.IsNullOrEmpty(item.FolderName))
                                    {
                                        FolderName = "Others";
                                    }
                                    if (Directory.Exists(ConfigurationManager.AppSettings["LocalPath"].ToString() + "//" + FolderName))
                                    {
                                        Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalPath"].ToString() + "//" + FolderName);
                                    }
                                    //Local Path
                                    string localPath = ConfigurationManager.AppSettings["LocalPath"].ToString() + "//" + FolderName + "//" + fileName;
                                    Helper.AddtoLogFile("Local Path " + localPath);
                                    Console.WriteLine("Local Path " + localPath);

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
