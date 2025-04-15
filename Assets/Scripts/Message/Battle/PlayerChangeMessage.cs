using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerChangeMessage : BaseMsg
{
    public string playerID;
    public float rotY;
    public float rotationAngle;
    public float rotationSmoothTime;
    public override int GetBytesNum()
    {
        return 4+4+
            4+Encoding.UTF8.GetBytes(playerID).Length
            +4+4+4;
    }

    public override int GetID()
    {
        return 2009;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        playerID = ReadString(bytes, ref index);
        rotY = ReadFloat(bytes, ref index);
        rotationAngle = ReadFloat(bytes, ref index);
        rotationSmoothTime = ReadFloat(bytes, ref index);
        return index-beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes,GetID(), ref index);
        WriteInt(bytes,GetBytesNum()-8, ref index);
        WriteString(bytes, playerID, ref index);
        WriteFloat(bytes, rotY, ref index);
        WriteFloat(bytes, rotationAngle, ref index);
        WriteFloat(bytes, rotationSmoothTime, ref index);
        return bytes;
    }

}
