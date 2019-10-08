using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace FtpConsoleClient
{
    class FtpClient     //  класс для создания ftp запросов
    {
        private string user, 
                       password, 
                       ip;
        private List<string> currDir;
        private FtpWebRequest request = null;
        private FtpWebResponse response = null;

        public string Dir
        {
            get
            {
                string str = "ftp://" + ip;

                foreach (string dir in currDir)
                    str += "/" + dir;
                str += "/";
                return str;
            }
        }

        public FtpClient(string ip, string user, string password)
        {
            this.user = user;
            this.password = password;
            this.ip = ip;
            currDir = new List<string>();
        }

        public RequestInfo TestConection()
        {
            try
            {
                request = (FtpWebRequest)WebRequest.Create(Dir);
            }
            catch (UriFormatException ex)
            {
                return new RequestInfo(request.Method, ex.Message, -1, null);
            }

            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(user, password);

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                return new RequestInfo(request.Method, ex.Message, (int)ex.Status, null);
            }

            return new RequestInfo(request.Method, "Done", 0, null);
        }

        public RequestInfo ChangeDir(string newDir)
        {
            if (Path.GetExtension(newDir) == "")
            {
                foreach (string dir in ListDirectory().FileInfo)
                    if (newDir == dir)
                    {
                        currDir.Add(newDir);
                        return new RequestInfo("ChDir", "Done", 0, null);
                    }

                if (newDir == "..")
                {
                    if (currDir.Count > 0)
                    {
                        currDir.RemoveAt(currDir.Count - 1);
                        return new RequestInfo("ChDir", "Done", 0, null);
                    }
                    else
                        return new RequestInfo("ChDir", "\"" + this.Dir + "\"" + " IT'S START FOLDER.", 3, null);
                }

                return new RequestInfo("ChDir", "\"" + newDir + "\"" + " IS NOT EXISTS IN THIS FOLDER.", 4, null);
            }
            else           
                return new RequestInfo("ChDir", "\"" + newDir + "\" IS NOT DIRECTORY.", 5, null);
        }

        public RequestInfo ListDirectory()
        {
            List<string> ls = new List<string>();

            try
            {
                request = (FtpWebRequest)WebRequest.Create(Dir);
            }
            catch (UriFormatException ex)
            {
                return new RequestInfo(request.Method, ex.Message, -1, null);
            }

            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(user, password);

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                return new RequestInfo(request.Method, ex.Message, (int)ex.Status, null);
            }

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            while (!reader.EndOfStream)
                ls.Add(reader.ReadLine());

            return new RequestInfo(request.Method, "Done", 0, ls);
        }

        public RequestInfo DownloadFile(string filename)
        {
            if (Path.GetExtension(filename) != "")
            {
                List<string> contentContains = new List<string>();

                try
                {
                    request = (FtpWebRequest)WebRequest.Create(Dir);
                }
                catch (UriFormatException ex)
                {
                    return new RequestInfo(request.Method, ex.Message, -1, null);
                }

                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(user, password);

                try
                {
                    response = (FtpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    return new RequestInfo(request.Method, ex.Message, (int)ex.Status, null);
                }

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                while (!reader.EndOfStream)
                {
                    contentContains.Add(reader.ReadLine());
                }

                return new RequestInfo("DownloadFile", "Done", 0, contentContains);
            }
            else
            {
                return new RequestInfo("DownloadFile", "\"" + filename + "\" IS NOT FILE.", -1, null);
            }
        }

        public RequestInfo UploadFile(string filepath)
        {
            if (Path.GetExtension(filepath) != "")
            {
                try
                {
                    request = (FtpWebRequest)WebRequest.Create(Path.Combine(Dir, Path.GetFileName(filepath)));
                }
                catch (UriFormatException ex)
                {
                    return new RequestInfo(request.Method, ex.Message, -1, null);
                }

                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(user, password);

                byte[] fileContents;
                using (StreamReader sourceStream = new StreamReader(filepath))
                {
                    fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                }

                request.ContentLength = fileContents.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                try
                {
                    response = (FtpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    return new RequestInfo(request.Method, ex.Message, (int)ex.Status, null);
                }

                return new RequestInfo("UploadFile", "Done", 0, null);
            }
            else
            {
                return new RequestInfo("UploadFile", "\"" + filepath + "\" IS NOT FILE.", -1, null);
            }
        }

    }
}
