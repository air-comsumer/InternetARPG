using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRoomListServerMsg : BaseMsg
{
    public int roomCount;
    public List<RoomInfo> roomList = new List<RoomInfo>();
    public override int GetBytesNum()
    {
        int num = 0;
        num += 4;//Э��ID
        num += 4;//����
        num += 4;
        foreach (var r in roomList)
        {
            num += r.GetBytesNum();
        }
        return num;
    }

    public override int GetID()
    {
        return 2003;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        roomCount = ReadInt(bytes, ref index);
        for (int i = 0; i < roomCount; i++)
        {
            var a = ReadData<RoomInfo>(bytes, ref index);
            Debug.Log("�����" + a.num);
            roomList.Add(a);
        }
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, GetBytesNum() - 8, ref index);
        WriteInt(bytes, roomCount, ref index);
        for (int i = 0; i < roomCount; i++)
        {
            WriteData(bytes, roomList[i], ref index);
        }
        return bytes;
    }
}
