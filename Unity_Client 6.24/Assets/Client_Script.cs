using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using TMPro;


[System.Serializable]
public class User
{
    public string ID;
    public string Password;
    public string Message;
    public string Status;
    public string token;
}


public class Client_Script : MonoBehaviour
{
    public TextMeshProUGUI User_NameInfo;
    public InputField Member_Name;
    public InputField Member_ID;
    public InputField Member_password;

    public InputField input_ID;
    public InputField input_password;
    bool is_Login;

    public TMP_InputField Upload_Message;

    public GameObject Login_Screen;
    public GameObject MemberCreate_Screen;
    public GameObject Upload_Screen;
    public TextMeshProUGUI User_Message;
    bool Connect = false;

    string User_Name = "";
    string Response_Date = "";
    string Up_Message = "";

    bool is_Upload = false;
    bool Check_Logout = false;
    bool Send_Status = false;

    string Member_Number = "";

    float timer = 5.0f;


    void Start()
    {
        Login_Screen.SetActive(true);
        MemberCreate_Screen.SetActive(true);
        Member_Number = "";
        Response_Date = "";
        Up_Message = "";
        User_Name = "";
    }


    void Update()
    {
        if (Connect)
        {
            Login_Screen.SetActive(false);
            MemberCreate_Screen.SetActive(false);
            Upload_Screen.SetActive(true);
            User_NameInfo.text = "Connect : " + User_Name;
            User_Message.text = Up_Message;
        }

        Status_check();
    }

    public void Status_check()
    {
        if(0 > timer)
        {
            
            var User_upload = new User()
            {
                ID = User_Name,
                Status = "Status_Check",
                token = Member_Number
            };

            string Message_Json = JsonUtility.ToJson(User_upload);
            Send_Status = true;
            StartCoroutine(Restcall(Message_Json));
            timer = 5.0f;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    public void Button_UploadMessage()
    {
        string Messge_path = @"C:\Users\juwon\Desktop\NoticeServer6.24_Client\BasicServerTest\bin\Debug\net6.0\Noticeboard.json";
        Up_Message = "";
        if (File.Exists(Messge_path))
        {
            Up_Message = Upload_Message.text;
            Debug.Log(Up_Message);
            var User_upload = new User()
            {
                ID = User_Name,
                Message = Up_Message,
                Status = "Upload_Message_Success"
            };
            string test = $"{User_upload.ID} : {User_upload.Message}";

            string Message_Json = JsonUtility.ToJson(test);
            string MS_json = $"{Message_Json}\n";
            File.AppendAllText(Messge_path, MS_json.ToString());
            is_Upload = true;
            StartCoroutine(Restcall(Message_Json));
        }
        else
        {
            StreamWriter json_Write = File.CreateText(Messge_path);
            Debug.Log("메세지 로그 파일이 생성되었습니다.");
            Button_UploadMessage();
        }
        
    }

    public void Button_CreateMember()
    {
        string path = @"C:\Users\juwon\Desktop\NoticeServer6.24_Client\BasicServerTest\bin\Debug\net6.0\Member_Info.json";

        if (File.Exists(path))
        {
            User user1 = new User()
            {
                ID = Member_ID.text,
                Password = Member_password.text,
                Status = "Create_New_Member"
            };

            string json_Convert = JsonUtility.ToJson(user1);
            string Member_json = $"{json_Convert}\n";
            File.AppendAllText(path, Member_json.ToString());
            Debug.Log(json_Convert + "json 추가완료");
        }
        else
        {
            StreamWriter json_Write = File.CreateText(path);
            Debug.Log("회원정보 파일이 생성되었습니다.");
            Button_CreateMember();
        }
    }

    public void ButtonClickHandler()
    {

        string path = @"C:\Users\juwon\Desktop\NoticeServer6.24_Client\BasicServerTest\bin\Debug\net6.0\Member_Info.json";

        if (File.Exists(path))
        {
            var user = new User
            {
                ID = input_ID.text,
                Password = input_password.text,
                Status = "Try_Login"
            };

            string json = JsonUtility.ToJson(user);
            is_Login = true;
            StartCoroutine(Restcall(json));
        }
        else
        {
            StreamWriter json_Write = File.CreateText(path);
            Debug.Log("회원정보 파일이 생성되었습니다.");
            ButtonClickHandler();
        }

       
    }

    public void onLogoutBtn_Click()
    {
        
        string path = @"C:\Users\juwon\Desktop\NoticeServer6.24_Client\BasicServerTest\bin\Debug\net6.0\Connect_Member.json";

        if (File.Exists(path))
        {
            var user = new User
            {
                ID = User_Name,
                token = Member_Number,
                Status = "Check_Logout"
            };

            string json = JsonUtility.ToJson(user);
            Check_Logout = true;
            StartCoroutine(Restcall(json));
        }
    }

    IEnumerator Restcall(string json)
    {
        if (is_Login)
        {
            using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:3000/GodDamnChatService/", json))
            {
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("From upload Complete!");
                    Debug.Log(www.downloadHandler.text);
                    Response_Date = www.downloadHandler.text;
                    if(Response_Date.Length == 5)
                    {
                        Connect = true;
                        User_Name = input_ID.text;
                        Member_Number = Response_Date;

                        string path = @"C:\Users\juwon\Desktop\NoticeServer6.24_Client\BasicServerTest\bin\Debug\net6.0\Connect_Member.json";

                        if (File.Exists(path))
                        {
                            var user = new User
                            {
                                ID = input_ID.text,
                                token = Response_Date
                            };

                            string js = JsonUtility.ToJson(user);
                            string json2 = $"{js}\n";
                            File.AppendAllText(path, json2.ToString());
                            is_Login = true;
                            
                        }
                        else
                        {
                            StreamWriter json_Write = File.CreateText(path);
                            Debug.Log("회원정보 파일이 생성되었습니다.");
                           
                        }
                    }
                    
                }
            }
            is_Login = false;
        }
        else if (is_Upload)
        {
            using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:3000/GodDamnChatService/", json))
            {
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("From upload Complete!");
                    Debug.Log(www.downloadHandler.text);
                    Response_Date = www.downloadHandler.text;
                    Up_Message = Response_Date;
                    is_Upload = false;
                }
            }
        }else if (Check_Logout)
        {
            using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:3000/GodDamnChatService/", json))
            {
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("From upload Complete!");
                    Debug.Log(www.downloadHandler.text);
                    Response_Date = www.downloadHandler.text;
                    if(Response_Date == $"Connect_false")
                    {
                        Member_Number = "";
                        Connect = false;
                        Check_Logout = false;
                    }
                }
            }
        }else if (Send_Status)
        {
            using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:3000/GodDamnChatService/", json))
            {
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("From upload Complete!");
                    Debug.Log(www.downloadHandler.text);
                    Response_Date = www.downloadHandler.text;
                    if (Response_Date == $"Connect_true")
                    {
                        Send_Status = false;
                        Debug.Log("연결되어 있음");
                    }
                    else if(Response_Date == $"Connect_false")
                    {
                        Send_Status = false;
                        Debug.Log("접속 끊김");
                    }
                }
            }
        }


    }

}
