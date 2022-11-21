using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


using System.Runtime.InteropServices;
static class min
{
    public const int bufsize = 1024;

    // main 1~16
    public const int MAIN_LOGJOIN = 2;

    // sub 1~16
    public const int SUB_LOGJOIN_LOGIN = 1;

    // detail 2의 16승까지 배수만
    public const int DETALI_LOGIN_RESULT = 1;
    public const int DETALI_JOIN_RESULT = 2;
    public const int DETALI_LOGIN_SUCCESS = 4;
    public const int DETALI_JOIN_SUCCESS = 8;
}

public class Sever : SingleTonMonobehaviour<Sever>
{
    // Start is called before the first frame update
    
    public string serverIp = "127.0.0.1";
    Socket clientSocket = null;

    Queue<socket_data> s_que;
    Queue<socket_data> r_que;

    ManualResetEvent s_event;

    // Start is called before the first frame update
    void Start()
    {
        
        //클라이언트에서 사용할 소켓 준비
        this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("Sever 접속시도");
        //클라이언트는 바인딩할 필요 없음

        //접속할 서버의 통신지점(목적지)
        IPAddress serverIPAdress = IPAddress.Parse(this.serverIp);
        IPEndPoint serverEndPoint = new IPEndPoint(serverIPAdress, 9000);

        //서버로 연결 요청
        try
        {
            Debug.Log("Connecting to Server");
            this.clientSocket.Connect(serverEndPoint);
            // 서버 연결 성공시 
            ThreadCreate();     //쓰레드 생성
        }
        catch (SocketException e)
        {
            Debug.Log("Connection Failed:" + e.Message);
        }
    }



    private void Update()
    {
        
    }

    public void SendQueCheck(Sever sever)
    {
        if(sever.s_que.Count > 0)
        {
            socket_data data = s_que.Dequeue();
            Send(data);
        }
    }

    public void RecvQueCheck(Sever sever)
    {
        if (sever.r_que.Count > 0)
        {
            socket_data data = r_que.Dequeue();
            //리시브된걸 언패킹후 처리
        }
    }

    private void ThreadCreate()
    {
        //샌드 쓰레드 생성
        Thread sendThread = new Thread(s_Thread);
        sendThread.Start();
        Debug.Log("Send쓰레드 실행");
    }



    private void s_Thread()
    {
        s_que = new Queue<socket_data>();
        s_event = new ManualResetEvent(false);

        //리시비 쓰레드 생성
        Thread recvThread = new Thread(r_Thread);
        recvThread.Start();
        Debug.Log("Recv쓰레드 실행");

        Packpaket();

        while(true)
        {
            if (s_event.WaitOne() == true)  // 이벤트 발생시
            {
                for (int i = 0; i < s_que.Count; i++)
                {
                    socket_data temp = new socket_data();
                    temp = s_que.Dequeue();
                    //Sever.Instance.clientSocket.Send(packet, 0, packet.Length, SocketFlags.None);
                }
            }

            //if() 스레드 탈출 조건 걸기
            {

            }
        }
    }

    private void r_Thread()
    {
        while(true)
        {
            Recv();

            // 큐에서 리시브 빼서 적용하기

            // 스레드 탈출 조건 걸기
        }
       
    }

    public static void Send(Byte[] packet)
    {
        if (Sever.Instance.clientSocket == null)
        {
            return;
        }
    
        //byte[] prefSize = new byte[1];
        //prefSize[0] = (byte)packet.Length;    //버퍼의 가장 앞부분에 이 버퍼의 길이에 대한 정보가 있는데 이것을 
        //Sever.Instance.clientSocket.Send(prefSize);    //먼저 보낸다.
        Sever.Instance.clientSocket.Send(packet, 0, packet.Length, SocketFlags.None);

    }

    public static void Recv()
    {
        int m_size = 0;
        int retval = 0;
        byte[] m_packet = new byte[1024];

        if (Sever.Instance.clientSocket == null)
        {
            return;
        }

        Sever.Instance.clientSocket.Receive(m_packet, sizeof(int), retval, SocketFlags.None);

        m_size = BitConverter.ToInt32(m_packet, 0);
        Debug.Log(m_size);

        Sever.Instance.clientSocket.Receive(m_packet, m_size, retval, SocketFlags.None);



        Sever.Instance.r_que.Enqueue(/*언패킹해서 만든 socket_data*/);
        // 언패킹하고 큐에 넣기
    }

    private void OnApplicationQuit()
    {
        if (this.clientSocket != null)
        {
            this.clientSocket.Close();
            this.clientSocket = null;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class socket_data
    {
        public int msg;
        public short size;
        public short type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public char[] data;
    }

    private void Packpaket()
    {
        socket_data s_data = new socket_data();
        s_data.data = new char[100];
        string strData = "testString";
        s_data.msg = 1;
        s_data.size = (short)Marshal.SizeOf(typeof(socket_data));
        s_data.type = 5;

        int len = strData.Length;
        for(int i =0;i<len;i++)
        {
            s_data.data[i] = strData[i];
        }
        byte[] packet = new byte[1];
        StruckToBytes(s_data, ref packet);
        Sever.Instance.clientSocket.Send(packet, 0, packet.Length, SocketFlags.None);
    }

    private void StruckToBytes(object obj, ref byte[] packet)
    {
        int size = Marshal.SizeOf(obj) + sizeof(int);
        //packet = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size + 1);

        int k = sizeof(int);  // 인트 크기
        byte[] kk = new byte[k];  // 바이트 크기
        byte[] kkk = new byte[size];  // 배열 크기

        kk = BitConverter.GetBytes(k);  // kk에 인트 넣기

        packet = new byte[kkk.Length + size];

        Marshal.StructureToPtr(obj, ptr, false);
        Marshal.Copy(ptr, kkk, 0, size);

        Array.Copy(kk, 0, packet, 0, kk.Length);
        Array.Copy(kkk, 0, packet, 0, kkk.Length);

        //Marshal.Copy(ptr, packet, 0, size);
        Marshal.FreeHGlobal(ptr);
    }

    public T ByteArrayToStruct<T>(byte[] buffer) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));
        if (size > buffer.Length)
        {
            throw new Exception();
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 0, ptr, size);
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }

    // Update is called once per frame
}
