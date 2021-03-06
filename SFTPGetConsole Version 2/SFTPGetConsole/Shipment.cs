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
                                    ToUpload += "/";
                                }

                                string FileToUpload = ToUpload + item.FolderNameAlias;

                                if (!FileToUpload.EndsWith("/"))
                                {
                                    FileToUpload += "/";
                                }
                                RemoteDirectoryInfo directory = null;
                                try
                                {
                                    directory = session.ListDirectory(FileToUpload);
                                }
                                catch { }
                                List<string> filePaths = Directory.GetFiles(item.FolderNameShip).ToList();

                                foreach (string file in filePaths)
                                {
                                    //if (directory.Files.Where(f => f.Name == Path.GetFileName(file)).Count()>0)
                                    //{
                                    try
                                    {
                                        transferResult = session.PutFiles(file, FileToUpload, true, transferOptions);
                                        // Throw on any error
                                        transferResult.Check();

                                        // Print results
                                        foreach (TransferEventArgs transfer in transferResult.Transfers)
                                        {
                                            Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                                            Helper.AddtoLogFile("Upload of " + transfer.FileName + " succeeded");
                                            Helper.AddtoLogFile("Seesion Log Path :" + session.SessionLogPath);

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Helper.AddtoLogFile("Error:" + ex.ToString());
                                        Helper.AddtoLogFile("Seesion Log Path :" + session.SessionLogPath);
                                    }

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(item.FolderPathArchive))
                                        {
                                            
                                            //Cut File From Source To destination
                                            string ArchiveFile = item.FolderPathArchive;

                                            if (!ArchiveFile.EndsWith("\\"))
                                            {
                                                ArchiveFile += "\\";
                                            }
                                            string filename = Path.GetFileName(file);
                                            ArchiveFile += filename;
                                            System.IO.File.Move(file, ArchiveFile);
                                            Console.WriteLine("File Moves from " + file + " to " + ArchiveFile);
                                            Helper.AddtoLogFile("File Moves from " + file + " to " + ArchiveFile);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Helper.AddtoLogFile("Error:" + ex.ToString());
                                    }

                                    //}
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
