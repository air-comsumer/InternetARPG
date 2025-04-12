using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : SingletonMono<GameDataManager>
{
    private Dictionary<int,RoomInfo> roomDic = new Dictionary<int, RoomInfo>();
    public Dictionary<int, RoomInfo> RoomDic
    {
        get { return roomDic; }
    }
    public void RefreshRoomDic(GetRoomListServerMsg msg)//存储刷新之后的房间列表数据
    {
        roomDic.Clear();
        int i = 0;

        foreach (var roomInfo in msg.roomList)
        {
            roomDic.Add(i,roomInfo);
            i++;
        }
    }
}
