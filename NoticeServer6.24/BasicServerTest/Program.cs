using System;
using System.Net;
using System.Threading;
using System.Text.Json;
using System.Text;


namespace FinalTestSample
{
    class data
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string token_Id { get; set; }
    }

    public class Global
    {
        public static Mutex mut = new Mutex();
        public static List<string> id = new List<string>();
        public static bool connect = false;

        public static void InitServer()
        {
            ServerLoop SL = new ServerLoop("http://127.0.0.1", "3000");
            SL.AddRestAPI("GodDamnChatService");
            SL.Run();
            //서버 초기화 로직을 여기에 담으세요
        }

        public static void ContextHandle(object httplistener)
        {
            string Request_Data = "";

            mut.WaitOne();
            HttpListener listener = (HttpListener)httplistener;
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            Stream body = request.InputStream;
            Encoding encoding = request.ContentEncoding;
            StreamReader reader = new StreamReader(body, encoding);
            string s = reader.ReadToEnd();
            mut.ReleaseMutex();

            //사용자의 입력을 받고 출력을 한다.
            Request_Data = s;
            Console.WriteLine(s);

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var test = JsonSerializer.Deserialize<data>(Request_Data, jsonOptions);

            if (test.Status == "Try_Login")
            {
                checkLogin(context, test.Id, test.Password);
            }
            else if(test.Status == "Upload_Message_Success")
            {
                GetNewMessage(context, test.Message);
            }else if(test.Status == "Check_Logout")
            {
                checkLogout(context, test.Id ,test.token_Id);
            }else if(test.Status == "Status_Check")
            {
                Status_Check(context, test.Id, test.token_Id);
            }


        }

        public static void Response(HttpListenerContext context, string Response_Data)
        {
            HttpListenerResponse response = context.Response;
            byte[] buffer = Encoding.UTF8.GetBytes(Response_Data);
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }


        static void GetNewMessage(HttpListenerContext context, string pName)
        {
            //사용자가 받지못한 최신 메시지를 출력하세요
            string json_path = @"Noticeboard.json";
            string[] json_Value = File.ReadAllLines(json_path);
            string Response_Message = "";

            for (int i = 0; i < json_Value.Length; i++)
            {
                Response_Message += json_Value[i];
            }
            Response(context, Response_Message);
        }

        static void checkLogin(HttpListenerContext context, string pID, string serverID)
        {

            //사용자ID가 서버에 저장되어있는지 확인하세요.
            //저장되어있다면 true 아니라면 false
            bool Login_Check = true;
            int Value_Count = 0;
            string Response_Data = "";

            while (Login_Check)
            {
                string json_path = @"Member_Info.json";
                string[] json_Value = File.ReadAllLines(json_path);

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var User_Data = JsonSerializer.Deserialize<data>(json_Value[Value_Count], jsonOptions);

                if (User_Data.Id == pID && User_Data.Password == serverID)
                {
                    string U_Name = User_Data.Id;
                    
                    Console.WriteLine("로그인 되었습니다.");
                    Value_Count = 0;
                    Login_Check = false;
                    connect = true;

                    Response_Data = GetServerID();
                    Console.WriteLine(Response_Data);
                    Response(context, Response_Data);
                }
                else
                {
                    Value_Count++;

                    if (Value_Count == json_Value.Length)
                    {
                        Console.WriteLine("계정이 맞지 않습니다.");
                        Value_Count = 0;
                        Login_Check = false;
                    }
                }

            }

        }

        public static string GetServerID()
        {
            //서버에서 고유 id를 부여해서 리턴하시오

            int[] ran_array = new int[4];
            for (int i = 0; i < ran_array.Length; i++)
            {
                var ran = new Random();
                int ran_num = ran.Next(1, 10);
                ran_array[i] = ran_num;
            }

            int Client_Num = Global.id.Count;
            string token_id = $"{Client_Num}{ran_array[0]}{ran_array[1]}{ran_array[2]}{ran_array[3]}";
            Global.id.Add(token_id);

            return token_id;

        }

        public static void checkLogout(HttpListenerContext context, string client_Id, string token_Id)
        {
            
            bool Logout_Check = true;
            int Value_Count = 0;
            string Response_Data = "";

            while (Logout_Check)
            {
                string json_path = @"Connect_Member.json";
                string[] json_Value = File.ReadAllLines(json_path);

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                var User_Data = JsonSerializer.Deserialize<data>(json_Value[Value_Count], jsonOptions);

                if (User_Data.Id == client_Id && User_Data.token_Id == token_Id)
                {
                    string U_Name = client_Id;

                    Console.WriteLine($"로그아웃 되었습니다.");
                    Value_Count = 0;
                    Logout_Check = false;

                    connect = false;
                    Response_Data = $"Connect_false";
                    Response(context, Response_Data);
                }
                else
                {
                    Value_Count++;

                    if (Value_Count == json_Value.Length)
                    {
                        Console.WriteLine("토큰 값이 맞지 않습니다.");
                        Value_Count = 0;
                        Logout_Check = false;
                    }
                }

            }
        }

        public static void Status_Check(HttpListenerContext context, string User_Id, string token_Id)
        {
            bool Check = true;
            int Value_Count = 0;
            string Response_Data = "";

            while (Check)
            {

                string json_path = @"Connect_Member.json";
                string[] json_Value = File.ReadAllLines(json_path);

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var User_Data = JsonSerializer.Deserialize<data>(json_Value[Value_Count], jsonOptions);

                if (connect)
                {
                    if (User_Data.token_Id == token_Id)
                    {

                        Console.WriteLine($"접속중입니다.");
                        Value_Count = 0;
                        Check = false;

                        Response_Data = $"Connect_true";
                        Response(context, Response_Data);
                    }
                    else
                    {
                        Value_Count++;

                        if (User_Data.token_Id != token_Id)
                        {
                            Console.WriteLine("미접속 상태입니다.");
                            Response_Data = $"Connect_false";
                            Response(context, Response_Data);
                            Value_Count = 0;
                            Check = false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("미접속 상태입니다.");
                    Response_Data = $"Connect_false";
                    Response(context, Response_Data);
                    Check = false;
                }
                

            }
        }

    }

    public class ServerLoop
    {
        HttpListener _listener;
        string _address;

        public ServerLoop(string address, string port)
        {
            _listener = new HttpListener();
            _address = "http://127.0.0.1" + ":" + port + "/";

            //Global.mut = new Mutex();
            //Global.mut.ReleaseMutex();
        }

        public void RestartLoop()
        {
            _listener = new HttpListener();
            Global.mut.ReleaseMutex();
        }

        

        public void AddRestAPI(string apiName)
        {
            _listener.Prefixes.Add(_address + apiName + "/");
        }

        public void Run()
        {
            _listener.Start();
            Console.WriteLine("Listening...");


            while (true)
            {
                ///문제4: 응답이 들어올경우 mutex를 이용하여 아래 코드를 새로운 접속이 생길때마다 1개의 Thread가 생기도록 개선하시오
                Global.mut.WaitOne();
                Thread t = new Thread(new ParameterizedThreadStart(Global.ContextHandle));
                t.Start(_listener);
                Global.mut.ReleaseMutex();
            }
        }

        public void Close()
        {
            _listener.Stop();
        }

       

        void EndLoop()
        {
            if (_listener != null)
            {
                _listener.Stop();
                _listener.Close();
            }
        }
    }



    public class Program
    {
        public static int Main()
        {
            FileLogger fLog = new FileLogger("Log");
            Global.InitServer();

            //Log type 종류
            //<Exception>
            //<Debug>
            //<Record>            

            fLog.Write("Debug","I hate life.");

            ServerLoop server = new ServerLoop("http://127.0.0.1","3000");
            server.AddRestAPI("GodDamnChatService");
           
            server.Run();



            return 0;
        }
    }
}