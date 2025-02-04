using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AMSLabel
{
    public class FTPClient
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Dir, Permission, Filecode, Owner, Group, Size, Filename, Datetime;
        FtpWebRequest ftpRequest = null;
        FtpWebResponse ftpResponse = null;
        Stream ftpStream = null;
        public FTPClient(string _userName, string _password, string _address, int _port)
        {
            UserName = _userName;
            Password = _password;
            IpAddress = _address;
            Port = _port;
        }
        public DateTime getdate(string filePath)
        {
            Uri serverUri = new Uri(filePath);
            ftpRequest = (FtpWebRequest)WebRequest.Create(serverUri) as FtpWebRequest;
            ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            ftpRequest.Credentials = new NetworkCredential(UserName, Password);
            ftpRequest.UsePassive = false;
            ftpRequest.UseBinary = true;
            ftpRequest.KeepAlive = false;
            ftpRequest.Proxy = null;
            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            DateTime lastModifiedDate = ftpResponse.LastModified;
            return lastModifiedDate;
        }
        public void GetDirectoriesList(string _path)
        {
            ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" +
            IpAddress + _path));
            ftpRequest.UseBinary = true;
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            ftpRequest.Credentials = new NetworkCredential(UserName, Password);

            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            Stream responseStream = ftpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            string line = reader.ReadLine();
            while (line != null)
            {
                //to do something
                line = reader.ReadLine();
            }

            reader.Close();
            ftpResponse.Close();
        }
        //public bool DownloadFileFTP(string ftpfilepath, string outputfilepath)
        //{
            
        //    bool IsOk = true;
        //    string fullOutputfilepath = outputfilepath;
        //    string ftpfullpath = ftpfilepath.Replace("\\", "//");
        //   // Stream stream;
        //   // StreamReader reader;
        //   // String response = null;
        //    ftpfullpath = "ftp://" + IpAddress + ftpfullpath;
        //    try
        //    {
        //        using (WebClient request = new WebClient())
        //        {
        //            request.Proxy = null;
        //            request.Credentials = new NetworkCredential(UserName, Password);
        //            //stream = request.OpenRead(ftpfullpath);
        //            //reader = new StreamReader(stream);
        //            //response = reader.ReadToEnd();
        //            byte[] fileData = request.DownloadData(ftpfullpath);

        //            using (FileStream file = File.Create(fullOutputfilepath))
        //            {
        //                file.Write(fileData, 0, fileData.Length);
        //                file.Close();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        IsOk = false;

        //    }
        //    return IsOk;
        //}

        public bool DownloadFileFTP(string ftpfilepath, string outputfilepath)
        {
            bool IsOk = true;
            int bufferSize = 2048;
           try
            {
                Uri serverUri = new Uri(ftpfilepath);
                ftpRequest = (FtpWebRequest)WebRequest.Create(serverUri) as FtpWebRequest;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpRequest.Credentials = new NetworkCredential(UserName, Password);
                ftpRequest.UsePassive = false;
                ftpRequest.UseBinary = true;
                ftpRequest.KeepAlive = false;
                ftpRequest.Proxy = null;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Get the FTP Server's Response Stream */
                ftpStream = ftpResponse.GetResponseStream();
                /* Open a File Stream to Write the Downloaded File */
                FileStream localFileStream = new FileStream(outputfilepath, FileMode.Create);
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesRead > 0)
                    {
                        localFileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex)
                {
                    IsOk = false;
                }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { IsOk = false; }
            return IsOk;
        }
        public bool UploadOneFileToFTP(string sourcePath, string ftpPath, string ftpDir)
        {
            try
            {
                string filename = Path.GetFileName(sourcePath);
                string ftpfullpath = "ftp://" + ftpPath + ftpDir.Replace("\\", "//") + "//" + filename;
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(ftpfullpath);
                ftpRequest.Credentials = new NetworkCredential(UserName, Password);
                ftpRequest.UsePassive = false;
                ftpRequest.KeepAlive = true;
                ftpRequest.UseBinary = true;
                ftpRequest.Proxy = null;
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                FileStream fs = File.OpenRead(sourcePath);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                Stream ftpstream = ftpRequest.GetRequestStream();
                ftpstream.Write(buffer, 0, buffer.Length);
                ftpstream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public bool UploadAllFileToFtp(string sourcePath, string ftpFileDir, string ftpDir)
        {
            MakeFTPDir( ftpDir.Replace("\\", "//") + "//" + ftpFileDir);
            string[] fileList;
            try
            {
                fileList = Directory.GetFiles(Path.GetDirectoryName(sourcePath));
                foreach (string FilePath in fileList)
                {
                    string filename = Path.GetFileName(FilePath);
                    string ftpfullpath = "ftp://" + IpAddress + ftpDir.Replace("\\", "//") + "//" + ftpFileDir + "//" + filename;
                    ftpRequest = (FtpWebRequest)FtpWebRequest.Create(ftpfullpath);
                    ftpRequest.Credentials = new NetworkCredential(UserName, Password);
                    ftpRequest.UseBinary = true;
                    ftpRequest.UsePassive = false;
                    ftpRequest.KeepAlive = true;
                    ftpRequest.Proxy = null;
                    ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                    FileStream fs = File.OpenRead(sourcePath);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    Stream ftpstream = ftpRequest.GetRequestStream();
                    ftpstream.Write(buffer, 0, buffer.Length);
                    ftpstream.Close();
                    ftpRequest = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        public bool DownloadAllFileFTP(string DirPath, string outputfilepath)
        {
            bool IsOk = true;
            string Dirpath = DirPath.Replace("\\", "/");
            string ftpfullpath = "ftp://" + IpAddress + Dirpath;
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpfullpath);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = false;
                ftpRequest.KeepAlive = true;
                ftpRequest.Proxy = null;
                ftpRequest.Credentials = new NetworkCredential(UserName, Password);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                List<string> directories = new List<string>();

                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    directories.Add(line);
                    line = streamReader.ReadLine();
                }
                streamReader.Close();
                ftpRequest = null;
                for (int i = 0; i <= directories.Count - 1; i++)
                {
                    if (directories[i].Contains("."))
                    {
                        DownloadFileFTP(Dirpath + directories[i].ToString(), outputfilepath + directories[i].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                IsOk = false;
            }
            return IsOk;
        }
        public bool CheckFileExistOrNot(string ftpURL)
        {
            FtpWebRequest ftpRequest = null;
            FtpWebResponse ftpResponse = null;
            bool IsExists = true;
            try
            {
                string ftpfullpath = ftpURL.Replace("\\", "//");
                ftpfullpath = "ftp://" + IpAddress + ftpfullpath;
                ftpRequest = (FtpWebRequest)WebRequest.Create(ftpfullpath);
                ftpRequest.Credentials = new NetworkCredential(UserName, Password);
                ftpRequest.UsePassive = false;
                ftpRequest.UseBinary = true;
                ftpRequest.KeepAlive = false;
                ftpRequest.Proxy = null;
                ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                IsExists = false;
            }
            return IsExists;
        }
        public bool FtpDirectoryExists(string directoryPath)
        {
            bool IsExists = true;
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryPath);
                request.Credentials = new NetworkCredential(UserName, Password);
                request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
                
            catch (WebException ex)
            {
                IsExists = false;
            }
            return IsExists;
        }
        public bool MakeFTPDir(string pathToCreate)
        {
            bool IsMakeOk = true;
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;


            string currentDir = string.Format("ftp://{0}", IpAddress);

            try
            {
                currentDir = currentDir + pathToCreate;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(currentDir);
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.Proxy = null;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(UserName, Password);
                ftpResponse = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                ftpStream.Close();
                ftpResponse.Close();
            }
            catch (Exception ex)
            {
                IsMakeOk = false;
            }

            return IsMakeOk;
        }
        public bool RemoveDir(string path)
        {
            try
            {
                FtpWebRequest reqFTP = null;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(path);
                /* Log in to the FTP Server with the User Name and Password Provided */
                reqFTP.Credentials = new NetworkCredential(UserName, Password);
                /* When in doubt, use these options */
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.KeepAlive = true;
                reqFTP.Proxy=null;
                /* Specify the Type of FTP Request */
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                /* Establish Return Communication with the FTP Server */
                 ftpResponse = (FtpWebResponse)reqFTP.GetResponse();
                /* Get the FTP Server's Response Stream */
                Stream ftpStream = ftpResponse.GetResponseStream();
                /* Open a File Stream to Write the Downloaded File */
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteFTPDirectory(string ftpDir, string Path)
        {
            bool IsOk = true;
            string FullPath= ftpDir +"//"+ Path;
            string ftpfullpath =FullPath.Replace("\\", "//");
            ftpfullpath = "ftp://" + IpAddress + ftpfullpath;
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpfullpath);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = false;
                ftpRequest.KeepAlive = true;
                ftpRequest.Proxy = null;
                ftpRequest.Credentials = new NetworkCredential(UserName, Password);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(ftpResponse.GetResponseStream());
                List<string> directories = new List<string>();

                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    directories.Add(line);
                    line = streamReader.ReadLine();
                }
                streamReader.Close();
                ftpResponse.Close();

                   for (int i = 0; i <= directories.Count - 1; i++)
                   {
                       if (directories[i].Contains("."))
                       {
                           DeleteFTPFile(ftpfullpath +"//"+ directories[i].ToString());
                       }
                    }
            }
            catch (Exception ex)
            {
                IsOk = false;
            }
            //if (RemoveDir(ftpfullpath))
            //    IsOk = true;
            //else
            //    IsOk = false;
            return IsOk;
        }

        public bool DeleteFTPFile(string Path)
        {
            bool isOk = true;
            try
            {
                FtpWebRequest ftpRequest = (System.Net.FtpWebRequest)WebRequest.Create(Path);
                ftpRequest.Credentials = new System.Net.NetworkCredential(UserName, Password);

                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = false;
                ftpRequest.KeepAlive = true;
                ftpRequest.Proxy = null;
                string result = string.Empty;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                long size = ftpResponse.ContentLength;
                Stream datastream = ftpResponse.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                ftpResponse.Close();
                isOk = true;
            }
            catch
            {
                isOk = false;
            }
            return isOk;

        }

    }
}
