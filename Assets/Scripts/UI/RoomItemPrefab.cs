using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemPrefab : BasePanel
{
    public void Init(int index,RoomInfo roomInfo)
    {
        GetControl<Text>("nameText").text = index.ToString();
        GetControl<Text>("CountText").text = roomInfo.num.ToString();
        GetControl<Text>("StatusText").text = roomInfo.Status switch
        {
            0 => "准备",
            1 => "游戏中",
            _ => "未知状态"
        };
    }
    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "JoinBtn":
                //加入房间
                Debug.Log("加入房间按钮被点击");
                EnterRoomMsg enterRoomMsg = new EnterRoomMsg();
                enterRoomMsg.roomID = int.Parse(GetControl<Text>("nameText").text);
                NetMgrAsync.Instance.Send(enterRoomMsg, (msg) => 
                {
                    BoolMsg boolMsg = msg as BoolMsg;
                    if (boolMsg.result)
                    {
                        Debug.Log("加入房间成功");
                        UIManager.Instance.GetPanel("RoomPanel").ShowMe();
                    }
                    else
                    {
                        Debug.Log("加入房间失败");
                    }
                });
                break;
            case "DeleteBtn":
                //删除房间
                Debug.Log("删除房间按钮被点击");
                EventCenter.Instance.EventTrigger("DeleteRoom", int.Parse(GetControl<Text>("nameText").text));
                break;
        }
    }

}
