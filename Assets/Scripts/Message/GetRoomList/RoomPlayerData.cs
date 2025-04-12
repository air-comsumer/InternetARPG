using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RoomPlayerData : BaseData
{
    public string id;
    public override int GetBytesNum()
    {
       return 4+Encoding.UTF8.GetBytes(id).Length;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        id = ReadString(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteString(bytes, id, ref index);
        return bytes;
    }

  
}
