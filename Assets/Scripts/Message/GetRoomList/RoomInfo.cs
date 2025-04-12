using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : BaseData
{
    public int num;//��������
    public int Status;

    public override int GetBytesNum()
    {
        return 4+4;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        num = ReadInt(bytes, ref index);
        Status = ReadInt(bytes, ref index);
        return index-beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes, num, ref index);
        WriteInt(bytes, Status, ref index);
        return bytes;
    }
}
