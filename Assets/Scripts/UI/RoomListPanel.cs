using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListPanel : BasePanel
{
    public GameObject roomPrefab;
    public RectTransform content;
    private int myRoomID = -1;
    private void Start()
    {
        UIManager.Instance.AddPanel("RoomListPanel", this);
    }

    protected override void OnClick(string btn)
    {
        switch (btn)
        {
            case "LogoutBtn":
                EventCenter.Instance.EventTrigger("ChangeScene", "SampleScene");
                break;
            case "NewBtn":
                //��������
                Debug.Log("�������䰴ť�����");
                NetMgrAsync.Instance.Send(new CreateRoomMsg(), CreateRoomHandle);
                break;
            case "RefreshBtn":
                //ˢ�·����б�
                NetMgrAsync.Instance.Send(new GetRoomListMsg());
                break;
        }
    }

    private void CreateRoomHandle(BaseMsg msg)
    {
        if((msg as CreateRoomServerMsg).result)
        {
            myRoomID = (msg as CreateRoomServerMsg).roomID;
            EnterRoomMsg enterRoomMsg = new EnterRoomMsg();
            enterRoomMsg.roomID = myRoomID;
            NetMgrAsync.Instance.Send(enterRoomMsg, (msg) =>
            {
                BoolMsg boolMsg = msg as BoolMsg;
                if (boolMsg.result)
                {
                    Debug.Log("���뷿��ɹ�");
                    UIManager.Instance.GetPanel("RoomPanel").ShowMe();
                }
                else
                {
                    Debug.Log("���뷿��ʧ��");
                }
            });
            NetMgrAsync.Instance.Send(new GetRoomListMsg());
        }
        else
        {
            Debug.Log("��������ʧ��");
        }
    }

    private void OnEnable()
    {
        NetMgrAsync.Instance.AddListener(2003,ReceiveGetRoomList);
        NetMgrAsync.Instance.Send(new GetRoomListMsg());
    }
    private void OnDisable()
    {
        NetMgrAsync.Instance.RemoveListener(2003, ReceiveGetRoomList);
    }

    private void ReceiveGetRoomList(BaseMsg msg)
    {
        Debug.Log("�õ������б���");
        GetRoomListServerMsg getRoomListServerMsg = msg as GetRoomListServerMsg;
        foreach(var a in getRoomListServerMsg.roomList)
        {
            Debug.Log("ʵ����֮���"+a.num);
        }
        GameDataManager.Instance.RefreshRoomDic(getRoomListServerMsg);
        int i=0;
        foreach (var roomInfo in GameDataManager.Instance.RoomDic.Values)
        {
            var roomItem = Instantiate(roomPrefab, content).GetComponent<RoomItemPrefab>();
            roomItem.Init(i++,roomInfo);
        }
    }
}
