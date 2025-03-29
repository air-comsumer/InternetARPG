using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Walk : SingletonMono<Walk>
{
    public GameObject prefab;//��ɫԤ����
    Dictionary<string,GameObject> players = new Dictionary<string, GameObject> ();//�������������
    string playerID = "";
    //��һ���ƶ�ʱ��
    public float lastMoveTime;
    //������
    public void AddPlayer(string id,Vector3 pos)
    {
        GameObject player = Instantiate(prefab,pos,Quaternion.identity);
        players.Add(id, player);
    }
    //ɾ�����
    public void DelPlayer(string id)
    {
        if (players.ContainsKey(id))
        {
            Destroy(players[id]);
            players.Remove(id);
        }
    }
    //������Ϣ
    public void UpdateInfo(BaseMsg baseMsg)
    {
        if (baseMsg is UpdateInfoServerMsg)
        {
            UpdateInfoServerMsg msg = baseMsg as UpdateInfoServerMsg;
            string id = msg.id;
            float x = msg.x;
            float y = msg.y;
            float z = msg.z;
            Vector3 pos = new Vector3 (x, y, z);
            UpdatePosInfo(id, pos);
        }
    }
    private void Start()
    {
    }
    /// <summary>
    /// ���ɽ�ɫ��ʼ��Ϸ
    /// </summary>
    /// <param name="id"></param>
    public void StartGame(string id)
    {
        playerID = id;
        float x =0;
        float y = 0;
        float z =0;
        Vector3 pos = new Vector3(x,y,z);
        AddPlayer(id, pos);
        //ͬ��
        SendPos();
        NetMgrAsync.Instance().Send(new GetListMsg(),GetList);//���ͻ�ȡ���н�ɫ��Ϣλ�õ�����
        NetMgrAsync.Instance().AddListener(1002,UpdateInfo);//��ӵ��ֵ��У��յ����Ż��Զ�ִ��
        NetMgrAsync.Instance().AddListener(1004, PlayerLeave);
    }
    /// <summary>
    /// ������������Լ���λ��
    /// </summary>
    private void SendPos()
    {
        GameObject player = players[playerID];
        Vector3 pos = player.transform.position;
        UpdateInfoMsg updateInfo = new UpdateInfoMsg();
        updateInfo.id = playerID;
        updateInfo.x = pos.x;
        updateInfo.y = pos.y;
        updateInfo.z = pos.z;
        NetMgrAsync.Instance().Send(updateInfo);
    }
    /// <summary>
    /// �������н�ɫλ��
    /// </summary>
    /// <param name="baseMsg"></param>
    private void GetList(BaseMsg baseMsg)
    {
        if (baseMsg is GetListMsg)
        {
            GetListMsg msg = baseMsg as GetListMsg;
            foreach (var player in msg.playerList)
            {
                string id = player.id;
                float x = player.x;
                float y = player.y;
                float z = player.z;
                Vector3 pos = new Vector3(x, y, z);
                UpdatePosInfo(id, pos);
            }
        }
    }

    private void UpdatePosInfo(string id, Vector3 pos)
    {
        if (players.ContainsKey(id))
        {
            players[id].transform.position = pos;
        }
        else
        {
            AddPlayer(id,pos);
        }
    }

    private void PlayerLeave(BaseMsg baseMsg)
    {
        if(baseMsg is PlayerLeave)
        {
            PlayerLeave msg = baseMsg as PlayerLeave;
            string id = msg.playerID;
            DelPlayer(id);
        }
    }
    void Move()
    {
        if (playerID == "") return;
        if (players[playerID]==null) return;
        if (Time.time - lastMoveTime < 0.1) return;
        lastMoveTime = Time.time;
        GameObject player = players[playerID];
        if (Input.GetKey(KeyCode.W))
        {
            player.transform.position += new Vector3(0, 0, 1);
            SendPos();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            player.transform.position += new Vector3(0, 0, -1);
            SendPos();

        }
        else if (Input.GetKey(KeyCode.D))
        {
            player.transform.position += new Vector3(1, 0, 0);
            SendPos();
        }
        else if(Input.GetKey(KeyCode.A))
        {
            player.transform.position += new Vector3(-1, 0, 0);
            SendPos();
        }
        
    }
    private void Update()
    {
        Move();
    }
}
