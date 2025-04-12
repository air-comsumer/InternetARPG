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
            0 => "׼��",
            1 => "��Ϸ��",
            _ => "δ֪״̬"
        };
    }
    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "JoinBtn":
                //���뷿��
                Debug.Log("���뷿�䰴ť�����");
                EnterRoomMsg enterRoomMsg = new EnterRoomMsg();
                enterRoomMsg.roomID = int.Parse(GetControl<Text>("nameText").text);
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
                break;
            case "DeleteBtn":
                //ɾ������
                Debug.Log("ɾ�����䰴ť�����");
                EventCenter.Instance.EventTrigger("DeleteRoom", int.Parse(GetControl<Text>("nameText").text));
                break;
        }
    }

}
