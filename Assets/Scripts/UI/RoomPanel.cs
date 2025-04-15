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
                Debug.Log("开始游戏");
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
        Debug.Log("开始游戏");
        startFightServerMsg = msg as StartFightServerMsg;
        //进入游戏场景
        SceneToLoadManager.Instance.ChangeScene("Battle", AfterSceneLoad);

    }
    private void AfterSceneLoad()
    {
        Debug.Log("这对吗?");
        if (startFightServerMsg == null) return;
        foreach (var player in startFightServerMsg.players)
        {
            Debug.Log("玩家ID" + player.id);
            //根据玩家ID创建对应的角色
            if (player.id != GameManager.Instance.playerName)//不是房主
            {
                GameObject playerObj = Instantiate(playerFightPrefab);
                playerObj.transform.position = new Vector3(player.x, player.y, player.z);
                playerObj.GetComponentInChildren<TextMeshPro>().text = player.id;
            }
            else
            {
                //房主
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
        Debug.Log("获取房间信息");
        GetRoomInfoServerMsg getRoomInfoServerMsg = msg as GetRoomInfoServerMsg;
        int i = 0;
        foreach(var a in getRoomInfoServerMsg.roomPlayers)
        {
            Debug.Log("房间里的人"+a.id);
            playerPrefabs[i].GetControl<Text>("PlayerText").text = a.id;
            i++;
        }
    }
}
