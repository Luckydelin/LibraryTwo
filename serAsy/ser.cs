using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
public class ser
{
    public Socket socket;
    public Conn[] conns;
    public int maxConn = 50;
    public class Conn
    {
        public const int bufferSize = 1024;
        public Socket socket;
        public bool isUse = false;
        public byte[] readBuff = new byte[bufferSize];
        public int bufferCount = 0;
        public Conn()
        {
            readBuff = new byte[bufferSize];
        }
        //初始化
        public void Init(Socket socket)
        {
            this.socket = socket;
            isUse = true;
            bufferCount = 0;
        }
        public int buffRemain()
        {
            return bufferSize - bufferCount;

        }
        public string getAddress()
        {
            if (!isUse)
                return "无法获取地址";
            return socket.RemoteEndPoint.ToString();
        }
        public void Close()
        {
            if (!isUse)
                return;
            Console.WriteLine("断开连接！");
            socket.Close();
            isUse = false;
        }
    }
    public int NewIndex()
    {
        if (conns == null)
            return -1;
        for(int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
            {
                conns[i] = new Conn();
                return i;
            }
            else if(conns[i].isUse==false)
            {
                return i;
            }
        }
        return -1;
    }

    public void Start(string host,int port)
    {
        conns = new Conn[maxConn];
        for(int i=0;i<conns.Length;i++)
        {
            conns [i] = new Conn();
        }

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipadr=IPAddress.Parse (host);
        IPEndPoint ep = new IPEndPoint (ipadr, port);
        socket.Bind (ep);
        socket.Listen (maxConn);
        socket.BeginAccept(AcceptCb, null);
        Console.WriteLine("服务器启动！");
    }

    private void AcceptCb(IAsyncResult ar)
    {
        Socket socketC=socket.EndAccept(ar);
        int index=NewIndex();
        if(index<0)
        {
            socketC.Close();
            Console.WriteLine("连接已满！");

        }
        else
        {
            Conn conn=conns[index];
            conn.Init(socketC);
            string adr=conn.getAddress();
            Console.WriteLine("客户端："+adr+"已连接！");
            conn.socket.BeginReceive(conn.readBuff, conn.bufferCount, conn.buffRemain(),SocketFlags.None, ReceiveCb, conn);
        }
        socket.BeginAccept(AcceptCb,null);
    }

    private void ReceiveCb(IAsyncResult ar)
    {
        
        
        Conn conn =(Conn)ar.AsyncState;
        try
        {
            
            int count = conn.socket.EndReceive(ar);
            if (count <=0)
            {
                Console.WriteLine("收到：" + conn.getAddress() + "断开连接！");
                conn.Close();
                return;
            }
            string str =System.Text.Encoding.UTF8.GetString(conn.readBuff,0,count);// conn.readBuff

            Console.WriteLine("收到:" + conn.getAddress() + "数据：" + str);

            str = conn.getAddress() + ":" + str;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isUse)
                    continue;
                Console.WriteLine("将消息传给" + conns[i].getAddress());
                conns[i].socket.Send(bytes);
            }
            conn.socket.BeginReceive(conn.readBuff, conn.bufferCount, conn.buffRemain(), SocketFlags.None, ReceiveCb, conn);
        }
        catch (Exception ex)
        {

        }
        
        
   
    }

}

