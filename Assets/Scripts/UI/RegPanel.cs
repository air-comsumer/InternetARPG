using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegPanel : BasePanel
{
    private void Start()
    {
        UIManager.Instance.AddPanel(this.gameObject.name, this);
        HideMe();
    }
    
    protected override void OnClick(string btnName)
    {
        switch(btnName)
        {
            case "RegBtn":
                // ע��
                EventCenter.Instance.EventTrigger("RegBtn");
                break;
            case "BackBtn":
                // ����
                HideMe();
                break;
        }
    }
}
