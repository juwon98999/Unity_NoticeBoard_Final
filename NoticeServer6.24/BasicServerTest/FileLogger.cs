using System;
using System.IO;
using System.Text;
using System.Text.Json;


namespace FinalTestSample
{
    public class FileLogger
    {
        public string _fileName { get; set; }

        public FileLogger(string fileName)
        {
            //_fileName에 filename + 날짜.log 로 로그파일명 저장
            //예제 > fileName이 Log일때 Log220616.log

            StreamWriter Log_Write;
            DateTime Today_Date = DateTime.Today;
            string day = Today_Date.ToString("yyMMdd");
            string Log_path = @$"Log{day}.txt";
            Log_Write = File.CreateText(Log_path);


        }

        public void Write(string logType, string logMessage)
        {
            //로그는 [날짜 시간] <logType> logMessage로 작성되도록 아래 log라는 string에 저장
            //예시 > [2022-06-16 12:00:00] <Error> 로그인 에러

            StreamWriter Log_Write;
            DateTime Today_Date = DateTime.Today;
            string day = Today_Date.ToString("yyMMdd");
            string input_time = Today_Date.ToString("yyyy-MM-dd HH:mm:ss");
            string Log_path = @$"Log{day}.log";
            if (File.Exists(Log_path))
            {
                string input_Log = $"[{input_time}] <{logType}> {logMessage}";
                Log_Write = File.AppendText(Log_path);
                Log_Write.WriteLine(input_Log);
                Log_Write.Close();
            }

        }

    }
}
