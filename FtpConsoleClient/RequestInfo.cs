using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace FtpConsoleClient
{
    class RequestInfo   //  класс который хранит информацию ответа сервера (код ошибки, сообщение, содержание запрошенного файла)
    {
        private string method,      //  метод запроса серверу
                       message;     //  сообщение ответа сервера
        private int errorCode = -1;      //  код ошибки сервера, если ошибки нет, значение равно -1
        private List<string> fileInfo;      //  строки полученного файла или список каталога

        public string Method
        {
            get
            {
                return method;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
        }

        public int ErrorCode
        {
            get
            {
                return errorCode;
            }
        }

        public List<string> FileInfo
        {
            get
            {
                return fileInfo;
            }
        }

        public RequestInfo(string method, string message, int errorCode, List<string>  fileInfo)
        {
            this.method = method;
            this.message = message;
            this.errorCode = errorCode;
            this.fileInfo = fileInfo;
        }
    }
}
