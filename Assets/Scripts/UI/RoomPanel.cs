using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    public List<PlayerPrefab> playerPrefabs;
    void Start()
    {
        UIManager.Instance.AddPanel("RoomPanel", this);
        
        HideMe();
    }
    private void OnEnable()
    {
        NetMgrAsync.Instance.AddListener(2006, GetRoomInfo);
        NetMgrAsync.Instance.Send(new GetRoomInfoMsg());
    }
    private void OnDisable()
    {
        NetMgrAsync.Instance.RemoveListener(2006, GetRoomInfo);
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
