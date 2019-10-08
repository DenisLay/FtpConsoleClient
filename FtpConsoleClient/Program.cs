using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FtpConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter ip:");
            string ip = Console.ReadLine();
            Console.WriteLine("Enter username:");
            string user = Console.ReadLine();
            Console.WriteLine("Enter password:");
            string password = Console.ReadLine();

            FtpClient ftp = new FtpClient(ip, user, password);
            RequestInfo connectInfo = ftp.TestConection();
            if (connectInfo.ErrorCode != 0)
            {
                if (connectInfo.ErrorCode == 7)
                {
                    Console.WriteLine("Username or password is uncorrectly.");
                }
                else
                    Console.WriteLine(connectInfo.Message);

                Console.ReadKey();
                Environment.Exit(1);
            }
            
            if (!Directory.Exists("Downloads"))
                Directory.CreateDirectory("Downloads");
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Downloads"));
            Console.WriteLine();
            Console.WriteLine("Directory for downloads: " + Directory.GetCurrentDirectory());

            string com;
            int i;
            List<string> dirList = new List<string>();
            
            while (true)
            {
                dirList = ftp.ListDirectory().FileInfo;
                i = 1;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(ftp.Dir);
                Console.WriteLine();

                if (dirList.Count == 0)
                {
                    Console.WriteLine("\nFolder is empty.\n");
                }
                else
                    foreach (string dir in dirList)
                    {
                        Console.WriteLine(" "+ i + ") " + dir);
                        i++;
                    }

                Console.WriteLine("\nEnter command:");
                com = Console.ReadLine();

                string [] split = com.Split(' ');
                

                switch (split[0])
                {
                    case "cd":
                        RequestInfo chdir = ftp.ChangeDir(split[1]);
                        if (chdir.ErrorCode != 0)
                        {
                            Console.WriteLine("Error: " + chdir.Message);
                            Console.WriteLine("ErrorCode: " + chdir.ErrorCode);
                        }
                        break;

                    case "dl":
                        RequestInfo dl = ftp.DownloadFile(split[1]);
                        if (dl.ErrorCode != 0)
                        {
                            Console.WriteLine("Error: " + dl.Message);
                            Console.WriteLine("ErrorCode: " + dl.ErrorCode);
                        }

                        StreamWriter sw = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), split[1]), false, System.Text.Encoding.Default);
                        foreach (string line in dl.FileInfo)
                            sw.WriteLine(line);
                        sw.Close();

                        Console.WriteLine("Dowload " + split[1] + " is successful.");
                        break;

                    case "ul":
                        RequestInfo ul = ftp.UploadFile(split[1]);
                        if (ul.ErrorCode != 0)
                        {
                            Console.WriteLine("Error: " + ul.Message);
                            Console.WriteLine("ErrorCode: " + ul.ErrorCode);
                        }

                        Console.WriteLine("Uploading is done.");
                        break;

                    case "dc":
                        Environment.Exit(0);
                        break;
                }

            }
        }
    }
}
