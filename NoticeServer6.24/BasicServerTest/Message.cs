using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalTestSample
{
    public class RequestMessage
    {
        //요청 이벤트 타입
        //0:로그인
        //1:로그아웃
        //2:메시지조회
        //3:메시지발송
        //4:에러출력

        //응답 이벤트 타입
        //0: 커넥션 오류
        //1: 신규 메시지
        //2: 서버id
        //3: 서버종료

        public int Event { get; set; }
        public string TextMessage { get; set; }
    }

    public class ResponseMessage
    {
        //3: 커넥션 오류
        //4: 커넥션 오류
        public int Event { get; set; }
        public string TextMessage { get; set; }
    }
}
