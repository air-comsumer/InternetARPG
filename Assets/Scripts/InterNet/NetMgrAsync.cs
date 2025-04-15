using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;

public class NetMgrAsync : SingletonMono<NetMgrAsync>
{
    private Socket socket;
    private byte[] cacheBytes = new byte[1024*1024];
    private int cacheNum = 0;
    private Queue<BaseMsg> receiveQueue = new Queue<BaseMsg>();
    private HeartMsg heartMsg = new HeartMsg();
    private int SEND_HEART_MSG_TIME = 2;
    //消息监听表
    //发送消息的时候传入委托然后在事件里面添加对应监听
    public Dictionary<int,UnityAction<BaseMsg>> eventDict = new Dictionary<int,UnityAction<BaseMsg>>();
    public Dictionary<int,UnityAction<BaseMsg>> onceDict = new Dictionary<int, UnityAction<BaseMsg>>();
    public static int consoleNum = 15;//每帧处理num条消息
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InvokeRepeating("SendHeartMsg",0,SEND_HEART_MSG_TIME);
    }
    private void Update()
    {
        for(int i=0;i<consoleNum;i++)
        {
            if (receiveQueue.Count > 0)
            {
                BaseMsg baseMsg = receiveQueue.Dequeue();
                if(baseMsg.GetID()==2003)
                {
                    Debug.Log("房间列表包");
                }
                if(baseMsg.GetID() ==888)
                {
                    Debug.Log("bool包");
                    if(eventDict.ContainsKey((baseMsg as BoolMsg).id))
                    {
                        eventDict[(baseMsg as BoolMsg).id].Invoke(baseMsg);
                    }
                    if(onceDict.ContainsKey((baseMsg as BoolMsg).id))
                    {
                        onceDict[(baseMsg as BoolMsg).id].Invoke(baseMsg);
                        onceDict[(baseMsg as BoolMsg).id] = null;
                        onceDict.Remove((baseMsg as BoolMsg).id);
                    }
                }
                if(eventDict.ContainsKey(baseMsg.GetID()))
                {
                    eventDict[baseMsg.GetID()].Invoke(baseMsg);
                }
                if(onceDict.ContainsKey(baseMsg.GetID()))
                {
                    onceDict[baseMsg.GetID()].Invoke(baseMsg);
                    onceDict[baseMsg.GetID()] = null;
                    onceDict.Remove(baseMsg.GetID());
                }
            }
        }
    }
    public void AddListener(int id, UnityAction<BaseMsg> action)
    {
        if (eventDict.ContainsKey(id))
            eventDict[id] += action;
        else
            eventDict.Add(id, action);
    }
    public void AddOnceListener(int id , UnityAction<BaseMsg> action)
    {
        if(onceDict.ContainsKey(id))
            onceDict[id] += action;
        else
            onceDict.Add(id, action);
    }
    public void RemoveListener(int id, UnityAction<BaseMsg> action)
    {
        if (eventDict.ContainsKey(id))
        {
            eventDict[id] -= action;
            if (eventDict[id] == null)
            {
                eventDict.Remove(id);
            }
        }
    }
    public void RemoveOnceListener(int id,UnityAction<BaseMsg> action)
    {
        if (!onceDict.ContainsKey(id))
        {
            onceDict[id] -= action;
            if (onceDict[id] == null)
            {
                onceDict.Remove(id);
            }
        }
    }
    private void SendHeartMsg()
    {
        if (socket != null && socket.Connected)
        {
            Send(heartMsg);
        }
    }
    public void Connect(string ip,int port)
    {
        if (socket != null && socket.Connected)
            return;
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.RemoteEndPoint = iPEndPoint;
        args.Completed += (socket, args) =>
        {
            if (args.SocketError == SocketError.Success)
            {
                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(cacheBytes, 0, cacheBytes.Length);
                receiveArgs.Completed += ReceiveCallBack;
                this.socket.ReceiveAsync(receiveArgs);
            }
            else
            {
                Debug.Log("连接失败"+ args.SocketError);
            }
        };
        socket.ConnectAsync(args);

    }

    private void ReceiveCallBack(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.Success)
        {
            lock(receiveQueue)
            {
                HandleReceiveMsg(e.BytesTransferred);
            }
            e.SetBuffer(cacheNum, e.Buffer.Length - cacheNum);
            if (socket != null && socket.Connected)
                socket.ReceiveAsync(e);
            else
                Close();
        }
        else
        {
            Debug.Log("接受消息出错");
            Close();
        }
    }
    public void SendTest(byte[] bytes)
    {
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.SetBuffer(bytes, 0, bytes.Length);
        args.Completed += (socket, args) =>
        {
            if (args.SocketError != SocketError.Success)
            {
                Debug.Log("发送失败");
                Close();
            }
        };
        socket.SendAsync(args);
    }
    public void Send(BaseMsg msg,UnityAction<BaseMsg> action)
    {
        if(socket != null&&socket.Connected)
        {
            AddOnceListener(msg.GetID(), action);
            byte[] bytes = msg.Writing();
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SetBuffer(bytes, 0, bytes.Length);
            args.Completed += (socket, args) =>
            {
                if (args.SocketError!=SocketError.Success)
                {
                    Debug.Log("发送失败");
                    Close();
                }
            };
            socket.SendAsync(args);
        }

    }
    public void Send(BaseMsg msg)
    {
        if (socket != null && socket.Connected)
        {
            byte[] bytes = msg.Writing();
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SetBuffer(bytes, 0, bytes.Length);
            args.Completed += (socket, args) =>
            {
                if (args.SocketError != SocketError.Success)
                {
                    Debug.Log("发送失败");
                    Close();
                }
            };
            socket.SendAsync(args);
        }

    }
    public void Close()
    {
        if(socket != null)
        {
            QuitMsg msg = new QuitMsg();
            socket.Send(msg.Writing());
            socket.Shutdown(SocketShutdown.Both);
            socket.Disconnect(false);
            socket.Close();
            socket = null;
        }
    }

    private void HandleReceiveMsg(int receiveNum)
    {
        int msgID = 0;
        int msgLength = 0;
        int nowIndex = 0;
        cacheNum += receiveNum;
        while (true)
        {
            msgLength = -1;
            if (cacheNum - nowIndex >= 8)
            {
                msgID = BitConverter.ToInt32(cacheBytes,nowIndex);
                nowIndex += 4;
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
            }
            if (cacheNum - nowIndex >= msgLength && msgLength != -1)
            {
                BaseMsg baseMsg = null;
                Debug.Log(msgID);
                switch (msgID)
                {
                    case 1002:
                        baseMsg = new UpdateInfoServerMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 1001:
                        baseMsg = new GetListMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 1004:
                        baseMsg = new PlayerLeave();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 888:
                        baseMsg = new BoolMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 2003:
                        baseMsg = new GetRoomListServerMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 2004:
                        baseMsg = new CreateRoomServerMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 2006:
                        baseMsg = new GetRoomInfoServerMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;
                    case 2008:
                        baseMsg = new StartFightServerMsg();
                        baseMsg.Reading(cacheBytes, nowIndex);
                        break;

                }
                if (baseMsg != null)
                    receiveQueue.Enqueue(baseMsg);
                nowIndex += msgLength;
                if (nowIndex == cacheNum)
                {
                    cacheNum = 0;
                    break;
                }
            }
            else
            {
                if (msgLength != -1)
                    nowIndex -= 8;
                Array.Copy(cacheBytes,nowIndex,cacheBytes,0,cacheNum - nowIndex);
                cacheNum = cacheNum - nowIndex;
                break;
            }
        }
    }
    private void OnDestroy()
    {
        Close();
    }
}
