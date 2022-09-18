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

    private void ThreadCreate()
    {
        Thread sendThread = new Thread(s_Thread);
        sendThread.Start();
        Debug.Log("Send쓰레드 실행");

        //샌드 쓰레드 생성

        Thread recvThread = new Thread(r_Thread);
        recvThread.Start();
        Debug.Log("Recv쓰레드 실행");

        //리시비 쓰레드 생성
    }


    private void s_Thread()
    {
        Packpaket();


    }

    private void r_Thread()
    {
        while(true)
        {
            Recv();
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
        //Sever.Instance.clientSocket.Send(packet, 0, packet.Length, SocketFlags.None);
    }

    private void StruckToBytes(object obj, ref byte[] packet)
    {
        int size = Marshal.SizeOf(obj);
        packet = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size + 1);

        Marshal.StructureToPtr(obj, ptr, false);
        Marshal.Copy(ptr, packet, 0, size);
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
