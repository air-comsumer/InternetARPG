using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonMono<UIManager> 
{
    public Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>();
    public void AddPanel(string panelName, BasePanel panel)
    {
        if (!panels.ContainsKey(panelName))
        {
            panels.Add(panelName, panel);
        }
    }
    public void RemovePanel(string panelName)
    {
        if (panels.ContainsKey(panelName))
        {
            panels.Remove(panelName);
        }
    }
    public BasePanel GetPanel(string panelName)
    {
        if (panels.ContainsKey(panelName))
        {
            return panels[panelName];
        }
        return null;
    }

}
