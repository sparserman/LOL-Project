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

    // detail 2�� 16�±��� �����
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
        
        //Ŭ���̾�Ʈ���� ����� ���� �غ�
        this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("Sever ���ӽõ�");
        //Ŭ���̾�Ʈ�� ���ε��� �ʿ� ����

        //������ ������ �������(������)
        IPAddress serverIPAdress = IPAddress.Parse(this.serverIp);
        IPEndPoint serverEndPoint = new IPEndPoint(serverIPAdress, 9000);

        //������ ���� ��û
        try
        {
            Debug.Log("Connecting to Server");
            this.clientSocket.Connect(serverEndPoint);
            // ���� ���� ������ 
            ThreadCreate();     //������ ����
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
        Debug.Log("Send������ ����");

        //���� ������ ����

        Thread recvThread = new Thread(r_Thread);
        recvThread.Start();
        Debug.Log("Recv������ ����");

        //���ú� ������ ����
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
        //prefSize[0] = (byte)packet.Length;    //������ ���� �պκп� �� ������ ���̿� ���� ������ �ִµ� �̰��� 
        //Sever.Instance.clientSocket.Send(prefSize);    //���� ������.
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
