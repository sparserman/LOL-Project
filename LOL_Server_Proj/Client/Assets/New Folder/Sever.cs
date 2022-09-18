using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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


    private static void s_Thread()
    {

        
    }

    private static void r_Thread()
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

   

    // Update is called once per frame
  
}
