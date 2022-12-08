using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


using System.Runtime.InteropServices;
static class Pro
{
    public const int bufsize = 1024;
    public const int PROT_BIT_SIZE = 8;

    // main 1~16
    public const int MAIN_LOGJOIN = 1;
    public const int GAME_SELECT = 2;
    public const int GAME_MAIN = 3;

    public const int GAME_Poppy = 4;
    public const int GAME_Rengar = 5;
    public const int GAME_Ezreal = 6;
    public const int GAME_Orianna = 7;


    //  GAME_Champion sub 1~16
    public const int MOVE = 1;
    public const int ATTACK = 2;
    public const int SKILL_Q = 3;
    public const int SKILL_W = 4;
    public const int SKILL_E = 5;
    public const int SKILL_R = 6;
    public const int SKILL_PV = 7;

    // MAIN_LOGJOIN sub 1~16
    public const int SUB_LOGJOIN_LOGIN = 1;



    // GAME_SELECT sub 1~16
    public const int SUB_Poppy = 1;
    public const int SUB_Rengar = 2;
    public const int SUB_Ezreal = 3;
    public const int SUB_Orianna = 4;

    // GAME_MAIN sub 1~16

    //MAIN_LOGJOIN detail 2�� 16�±��� �����
    public const int DETALI_LOGIN_RESULT = 1;
    public const int DETALI_JOIN_RESULT = 2;
    public const int DETALI_LOGIN_SUCCESS = 4;
    public const int DETALI_JOIN_SUCCESS = 8;
}

public struct P
{
    public int main;
    public int sub;
    public int detail;
}

public class Sever : SingleTonMonobehaviour<Sever>
{
    // Start is called before the first frame update

    public string serverIp = "127.0.0.1";
    Socket clientSocket = null;

    Queue<byte[]> s_que = new Queue<byte[]>();
    Queue<byte[]> r_que = new Queue<byte[]>();


    public ChampController poppy;
    public ChampController ganga;
   

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
            ThreadCreate();     //������ ���� �̽ù߷��� ������ �����̾��� ���ϻ���;;;

            poppy.serverConnected = true;
        }
        catch (SocketException e)
        {
            Debug.Log("Connection Failed:" + e.Message);
        }

        int protocol = Manager_Protocol.Instance.Packing_prot(Pro.MAIN_LOGJOIN, Pro.SUB_LOGJOIN_LOGIN);



        int euckrCodepage = 51949;
        Encoding EnKr = Encoding.GetEncoding(euckrCodepage);




        byte[] msg = EnKr.GetBytes("�ȳ��ϼ���\n");
        byte[] to_bytes = BitConverter.GetBytes(msg.Length);
        byte[] buf = new byte[sizeof(int) + msg.Length];


        Array.Copy(to_bytes, 0, buf, 0, sizeof(int));
        Array.Copy(msg, 0, buf, sizeof(int), msg.Length);
        Packing(protocol, buf);

        //Recv();
    }

    private void Update()
    {
        //SendQueCheck(this);  // %%%%%%%%%%% �̺�Ʈ�� �Ҳ��� �ʿ����� �����غ���
        RecvQueCheck(this);
    }

    public void SendQueCheck(Sever sever)
    {
        if (sever.s_que.Count > 0)
        {
            byte[] buf = s_que.Dequeue();
            //Send(data);
        }
    }

    public void RecvQueCheck(Sever sever)
    {
        if (sever.r_que.Count > 0)
        {
            byte[] buf = r_que.Dequeue();
            Debug.Log("RecvQueCheck ����");
            //���ú�Ȱ� ����ŷ�� ó��

            UnPacking(buf);
        }
    }

    private void ThreadCreate()
    {
        //���ú� ������ ����
        Thread recvThread = new Thread(r_Thread);
        recvThread.Start();
        Debug.Log("Recv������ ����");

        //���� ������ ����
        //Thread sendThread = new Thread(s_Thread);
        //sendThread.Start();
        //Debug.Log("Send������ ����");
    }


    private void s_Thread()
    {
        s_que = new Queue<byte[]>();
        s_event = new ManualResetEvent(false);

        //���ú� ������ ����
        //Thread recvThread = new Thread(r_Thread);
        //recvThread.Start();
        Debug.Log("Recv������ ����");

        

        while (true)
        {
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% ������ �̺�Ʈ �ɱ�

            if (s_event.WaitOne() == true)  // �̺�Ʈ �߻���
            {
                for (int i = 0; i < s_que.Count; i++)
                {
                    
                    byte[] temp = s_que.Dequeue();

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

        
        Sever.Instance.clientSocket.Send(packet, 0, packet.Length, SocketFlags.None);
    }

    public class Manager_Protocol : SingleTonMonobehaviour<Manager_Protocol>
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
            temp = temp | p_main << Pro.PROT_BIT_SIZE * 3;
            prot = prot | temp;
            temp = 0;

            // sub
            temp = temp | p_sub << Pro.PROT_BIT_SIZE * 2;
            prot = prot | temp;
            temp = 0;

            // detail
            foreach (int num in p_args)
            {
                temp = temp | num;
            }

            prot = prot | temp;
            temp = 0;


            return prot;
        }
        public P Unpacking_prot(int p_prot)
        {
            int temp = 0;
            int main = 0;
            int sub = 0;
            int detail = 0;
                      

            temp = 0xff00000 & p_prot;
            main = temp >> Pro.PROT_BIT_SIZE * 3;
            temp = 0;

            temp = 0xff000 & p_prot;
            sub = temp >> Pro.PROT_BIT_SIZE * 2;
            temp = 0;

            temp = 0xffff & p_prot;
            detail = temp;

            Debug.Log("main :" + main);
            Debug.Log("sub :" + sub);
            Debug.Log("detail :" + detail);

            cleanList();
            detail_list = UnpackingDetail(detail);

            P p;
            p.main = main;
            p.sub = sub;
            p.detail = detail;

            return p;
        }

        bool[] UnpackingDetail(int args)
        {
            bool[] temp = new bool[Pro.PROT_BIT_SIZE * 2];
            int complement = 1;

            for (int i = 0; i < Pro.PROT_BIT_SIZE * 2; i++)
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


    static public string byteArrayout(byte[] bytes)
    {
        return string.Join(", ", bytes);
    }
    public void Packing(int p_prot, byte[] p_data) // ��������, ������
    {
        // ��������
        byte[] type_bytes = BitConverter.GetBytes(p_prot); 
        // �ø��� �ѹ�
        byte[] Snum_bytes = BitConverter.GetBytes(SirealN++); // �ø��� ����
        // ������
        byte[] m_data = new byte[p_data.Length];
        m_data = p_data;

        int total_size = sizeof(int) + sizeof(int) + m_data.Length;
        // �Ѱ���
        byte[] send_bytes = new byte[total_size + sizeof(int)];
        // �� ������

        


        Byte[] total_bytes = BitConverter.GetBytes(total_size);

        

        //�� ������
        Array.Copy(total_bytes, 0, send_bytes, 0, sizeof(int));

        //��������
        Array.Copy(type_bytes, 0, send_bytes, total_bytes.Length, type_bytes.Length);

        //�ø��� �ѹ�
        Array.Copy(Snum_bytes, 0, send_bytes, total_bytes.Length + type_bytes.Length, type_bytes.Length);

        //������ ����
        Array.Copy(m_data, 0, send_bytes, total_bytes.Length + type_bytes.Length + Snum_bytes.Length, m_data.Length);







        Debug.Log(byteArrayout(send_bytes));

        Send(send_bytes);
        // ť�� Ǫ��
    }

    public void MovePack(int p_protocol, Vector3 p_pos)
    {
        byte[] pos_x = BitConverter.GetBytes(p_pos.x);
        byte[] pos_y = BitConverter.GetBytes(p_pos.y);
        byte[] pos_z = BitConverter.GetBytes(p_pos.z);

        byte[] send_buf = new byte[sizeof(float) + sizeof(float) + sizeof(float)];
        int len = 0;

        Array.Copy(pos_x, 0, send_buf, len, pos_x.Length);
        len += pos_x.Length;

        Array.Copy(pos_y, 0, send_buf, len, pos_y.Length);
        len += pos_y.Length;

        Array.Copy(pos_z, 0, send_buf, len, pos_z.Length);
        len += pos_z.Length;


        Packing(p_protocol, send_buf);
    }

    void UnPacking(byte[] p_buf)
    {
        byte[] pt = new byte[sizeof(int)];
        Array.Copy(p_buf, pt, sizeof(int));
        int protocol = BitConverter.ToInt32(pt);
        int len = 0;
        len += sizeof(int);

        P p = Manager_Protocol.Instance.Unpacking_prot(protocol);

        

        Array.Copy(p_buf, len, pt, 0 ,sizeof(int));        //pt�� �ø���ѹ� ����
        int S_num = BitConverter.ToInt32(pt);
        Debug.Log("�ø��� �ѹ� : " + S_num);
        len += sizeof(int);

        switch (p.main)
        {
            case Pro.GAME_SELECT:
                switch (p.sub)
                {
                    case Pro.SUB_Poppy:
                        
                        break;
                    case Pro.SUB_Rengar:
                        
                        break;
                }
                break;


            case Pro.GAME_Poppy:
                switch(p.sub)
                {
                    case Pro.MOVE:
                        Vector3 pos;
                        Array.Copy(p_buf, len, pt, 0, sizeof(float));
                        pos.x = BitConverter.ToSingle(pt);
                        len += sizeof(float);

                        Array.Copy(p_buf, len, pt, 0, sizeof(float));
                        pos.y = BitConverter.ToSingle(pt);
                        len += sizeof(float);

                        Array.Copy(p_buf, len, pt, 0, sizeof(float));
                        pos.z = BitConverter.ToSingle(pt);
                        len += sizeof(float);

                        Debug.Log("Pos : " + pos.x + "," + pos.y + "," + pos.z);

                        poppy.SetMovePos(pos);
                        break;
                }
                break;
        }

        //Array.Copy(p_buf, sizeof(int) + sizeof(int), pt, 0, sizeof(int));        //pt�� ������ ������ ����
        //int data_size = BitConverter.ToInt32(pt);


        
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

        Sever.Instance.clientSocket.Receive(m_packet, retval, sizeof(int), SocketFlags.None);

        m_size = BitConverter.ToInt32(m_packet, 0);
        

        Sever.Instance.clientSocket.Receive(m_packet, retval, m_size, SocketFlags.None);



        //int length = 0;

        
        //Sever.Instance.UnPacking(m_packet);


        Sever.Instance.r_que.Enqueue(m_packet);
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

    
    // Update is called once per frame
}
