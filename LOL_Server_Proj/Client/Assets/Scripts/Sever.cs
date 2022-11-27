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
    public const int PROT_BIT_SIZE = 8;

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

    Queue<socket_data> s_que;
    Queue<socket_data> r_que;



    ManualResetEvent s_event;

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

        int protocol = Manager_Protocol.Instance.Packing_prot(min.MAIN_LOGJOIN, min.SUB_LOGJOIN_LOGIN, min.DETALI_LOGIN_RESULT);



        byte[] buf2 = new byte[1];
        

        //Packing(protocol, Encoding.UTF8.GetBytes("���Կ� �����߽��ϴ�.\n"));
        Packing(protocol, buf2);
    }

    private void Update()
    {
        //SendQueCheck(this);  // %%%%%%%%%%% �̺�Ʈ�� �Ҳ��� �ʿ����� �����غ���
        //RecvQueCheck(this);
    }

    public void SendQueCheck(Sever sever)
    {
        if (sever.s_que.Count > 0)
        {
            socket_data data = s_que.Dequeue();
            //Send(data);
        }
    }

    public void RecvQueCheck(Sever sever)
    {
        if (sever.r_que.Count > 0)
        {
            socket_data data = r_que.Dequeue();
            //���ú�Ȱ� ����ŷ�� ó��
        }
    }

    private void ThreadCreate()
    {
        //���� ������ ����
        Thread sendThread = new Thread(s_Thread);
        sendThread.Start();
        Debug.Log("Send������ ����");
    }


    private void s_Thread()
    {
        s_que = new Queue<socket_data>();
        s_event = new ManualResetEvent(false);

        //���ú� ������ ����
        //Thread recvThread = new Thread(r_Thread);
        //recvThread.Start();
        Debug.Log("Recv������ ����");

        Packpaket();

        while (true)
        {
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% ������ �̺�Ʈ �ɱ�

            if (s_event.WaitOne() == true)  // �̺�Ʈ �߻���
            {
                for (int i = 0; i < s_que.Count; i++)
                {
                    socket_data temp = new socket_data();
                    temp = s_que.Dequeue();

                    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% ����ť Ȱ��ȭ�ϴ� �̺�Ʈ �ֱ�

                    //Sever.Instance.clientSocket.Send(packet, 0, packet.Length, SocketFlags.None);
                }
            }

            //if() ������ Ż�� ���� �ɱ�
            {

            }
        }
    }

    private void r_Thread()
    {
        while (true)
        {
            Recv();

            // ť���� ���ú� ���� �����ϱ�

            // ������ Ż�� ���� �ɱ�
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

    class Manager_Protocol : SingleTonMonobehaviour<Manager_Protocol>
    {
        bool[] detail_list;
        int list_size;

        Manager_Protocol() 
        {
            detail_list = new bool[16];
            list_size = 0;
        }

        ~Manager_Protocol() 
        {
            cleanList(); 
        }

        void cleanList() 
        {
            if (detail_list != null)
                list_size = 0;
        }

        public int Packing_prot(int p_main, int p_sub, params int[] p_args)
        {
            // �������� ��ŷ
            int prot = 0;
            int temp = 0;

            // main
            temp = temp | p_main << min.PROT_BIT_SIZE * 3;
            prot = prot | temp;
            Debug.Log(temp);
            temp = 0;

            // sub
            temp = temp | p_sub << min.PROT_BIT_SIZE * 2;
            prot = prot | temp;
            Debug.Log(temp);
            temp = 0;

            // detail
            foreach (int num in p_args)
            {
                temp = temp | num;
            }

            prot = prot | temp;
            temp = 0;

            Debug.Log(prot);

            return prot;
        }
        void Unpacking_prot(int p_prot)
        {
            int temp = 0;
            int main = 0;
            int sub = 0;
            int detail = 0;
                      

            temp = 0xff00000 & p_prot;
            main = temp >> min.PROT_BIT_SIZE * 3;
            temp = 0;

            temp = 0xff000 & p_prot;
            sub = temp >> min.PROT_BIT_SIZE * 2;
            temp = 0;

            temp = 0xffff & p_prot;
            detail = temp;

            Debug.Log("main :" + main);
            Debug.Log("sub :" + sub);
            Debug.Log("detail :" + detail);

            cleanList();
            detail_list = UnpackingDetail(detail);
        }

        bool[] UnpackingDetail(int args)
        {
            bool[] temp = new bool[min.PROT_BIT_SIZE * 2];
            int complement = 1;

            for (int i = 0; i < min.PROT_BIT_SIZE * 2; i++)
            {
                if ((args & complement) != 0)
                {
                    temp[i] = true;
                    list_size++;
                }
                else
                {
                    temp[i] = false;
                }

                

                complement *= 2;
            }

            return temp;

        }
    }

    int SirealN = 3;
    void Packing(int p_prot, byte[] p_data) // ��������, ������
    {
        // ��������
        byte[] type_bytes = BitConverter.GetBytes(p_prot); 
        // �ø��� �ѹ�
        byte[] Snum_bytes = BitConverter.GetBytes(SirealN++); // �ø��� ����
        // ������
        byte[] m_data = new byte[p_data.Length];
        m_data = p_data;
        // �Ѱ���
        byte[] send_bytes = new byte[type_bytes.Length + Snum_bytes.Length + m_data.Length + 4];
        // �� ������
       
        byte[] total_bytes = BitConverter.GetBytes(send_bytes.Length);

        Debug.Log(total_bytes.Length);

        //�� ������
        Array.Copy(total_bytes, 0, send_bytes, 0, total_bytes.Length);

        //��������
        Array.Copy(type_bytes, 0, send_bytes, total_bytes.Length, type_bytes.Length);

        //�ø��� �ѹ�
        Array.Copy(Snum_bytes, 0, send_bytes, total_bytes.Length + type_bytes.Length, type_bytes.Length);

        //������ ����
        Array.Copy(m_data, 0, send_bytes, total_bytes.Length + type_bytes.Length + Snum_bytes.Length, m_data.Length);

        Send(send_bytes);
        // ť�� Ǫ��
        
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
        //Debug.Log(m_size);

        Sever.Instance.clientSocket.Receive(m_packet, m_size, retval, SocketFlags.None);





        //Sever.Instance.r_que.Enqueue(/*����ŷ�ؼ� ���� socket_data*/);
        // ����ŷ�ϰ� ť�� �ֱ�
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

        int k = sizeof(int);  // ��Ʈ ũ��
        byte[] kk = new byte[k];  // ����Ʈ ũ��
        byte[] kkk = new byte[size];  // �迭 ũ��

        kk = BitConverter.GetBytes(k);  // kk�� ��Ʈ �ֱ�

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
