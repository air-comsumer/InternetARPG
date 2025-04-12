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
                // ×¢²á
                EventCenter.Instance.EventTrigger("RegBtn");
                break;
            case "BackBtn":
                // ·µ»Ø
                HideMe();
                break;
        }
    }
}
