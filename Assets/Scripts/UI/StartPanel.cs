using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : BasePanel
{
    private void Start()
    {
        UIManager.Instance.AddPanel(this.gameObject.name, this);
    }
    protected override void OnClick(string btnName)
    {
        switch(btnName)
        {
            case "LoginBtn":
                // µÇÂ¼
                EventCenter.Instance.EventTrigger("LoginBtn");
                break;
            case "RegBtn":
                // ×¢²á
                EventCenter.Instance.EventTrigger("StartRegBtn");
                break;
        }
    }
}
