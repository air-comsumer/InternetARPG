using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    public List<PlayerPrefab> playerPrefabs;
    public StartFightServerMsg startFightServerMsg=null;
    public GameObject playerFightPrefab;
    void Start()
    {
        UIManager.Instance.AddPanel("RoomPanel", this);
        HideMe();
    }
    private void OnEnable()
    {
        NetMgrAsync.Instance.AddListener(2006, GetRoomInfo);
        //NetMgrAsync.Instance.Send(new GetRoomInfoMsg());
        NetMgrAsync.Instance.AddListener(2008, OnFightBegin);
    }
    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "StartBtn":
                Debug.Log("��ʼ��Ϸ");
                int roomID = (UIManager.Instance.GetPanel("RoomListPanel") as RoomListPanel).myRoomID;
                if (roomID!=-1)
                {
                    var msg = new StartFightMsg();
                    msg.myRoomID = roomID;
                    NetMgrAsync.Instance.Send(msg);
                }
                break;
        }
    }
    private void OnFightBegin(BaseMsg msg)
    {
        Debug.Log("��ʼ��Ϸ");
        startFightServerMsg = msg as StartFightServerMsg;
        //������Ϸ����
        SceneToLoadManager.Instance.ChangeScene("Battle", AfterSceneLoad);

    }
    private void AfterSceneLoad()
    {
        Debug.Log("�����?");
        if (startFightServerMsg == null) return;
        foreach (var player in startFightServerMsg.players)
        {
            Debug.Log("���ID" + player.id);
            //�������ID������Ӧ�Ľ�ɫ
            if (player.id != GameManager.Instance.playerName)//���Ƿ���
            {
                GameObject playerObj = Instantiate(playerFightPrefab);
                playerObj.transform.position = new Vector3(player.x, player.y, player.z);
                playerObj.GetComponentInChildren<TextMeshPro>().text = player.id;
            }
            else
            {
                //����
                GameObject playerobj = GameObject.FindGameObjectWithTag("Player");
                playerobj.GetComponent<PlayerMovementControl>()._playerName.text = player.id;
            }

        }
    }
    private void OnDisable()
    {
        //NetMgrAsync.Instance.RemoveListener(2006, GetRoomInfo);
    }
    private void GetRoomInfo(BaseMsg msg)
    {
        Debug.Log("��ȡ������Ϣ");
        GetRoomInfoServerMsg getRoomInfoServerMsg = msg as GetRoomInfoServerMsg;
        int i = 0;
        foreach(var a in getRoomInfoServerMsg.roomPlayers)
        {
            Debug.Log("���������"+a.id);
            playerPrefabs[i].GetControl<Text>("PlayerText").text = a.id;
            i++;
        }
    }
}
