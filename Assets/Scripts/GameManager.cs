using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : SingletonMono<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        NetMgrAsync.Instance.Connect("127.0.0.1", 12345);
    }
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.Instance.AddEventListener("LoginBtn", Login);
        EventCenter.Instance.AddEventListener("RegBtn", Reg);
        EventCenter.Instance.AddEventListener("StartRegBtn", () =>
        {
            UIManager.Instance.GetPanel("RegPanel").ShowMe();
        });
        //NetMgrAsync.Instance().SendTest(Encoding.UTF8.GetBytes("你好世界，再见世界"));
        //Walk.Instance.StartGame("高潇屹");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener("LoginBtn", Login);
        EventCenter.Instance.RemoveEventListener("RegBtn", Reg);
        EventCenter.Instance.RemoveEventListener("StartRegBtn", () =>
        {
            UIManager.Instance.GetPanel("RegPanel").ShowMe();
        });
    }
    private void Login()
    {
        StartPanel startPanel =  UIManager.Instance.GetPanel("StartPanel") as StartPanel;
        if (startPanel != null)
        {
            LoginMsg loginMsg = new LoginMsg() {username = startPanel.GetControl<InputField>("IDInputField").text 
            ,password = startPanel.GetControl<InputField>("PWInputField").text};
            Debug.Log("用户名"+loginMsg.username+"密码"+loginMsg.password);
            NetMgrAsync.Instance.Send(loginMsg,LoginHandle);
        }
    }
    private void LoginHandle(BaseMsg baseMsg)//服务端返回消息后处理的
    {
        BoolMsg msg = baseMsg as BoolMsg;
        Debug.Log("登录结果" + msg.result);
        if (msg.result)
        {
            //登录成功
            //Walk.Instance.StartGame(msg.id);
            EventCenter.Instance.EventTrigger<string, UnityAction>("ChangeScene", "SelectRoom",null);
        }
        else
        {
            //登录失败
            Debug.Log("登录失败");
        }
    }


    private void Reg()
    {
        RegPanel regPanel = UIManager.Instance.GetPanel("RegPanel") as RegPanel;
        if (regPanel != null)
        {
            var password = regPanel.GetControl<InputField>("RegPWInputField").text;
            if(password == regPanel.GetControl<InputField>("RegPWReInputField").text)
            {
                RegMsg regMsg = new RegMsg()
                {
                    username = regPanel.GetControl<InputField>("RegAdminInputField").text,
                    password = password
                };
                NetMgrAsync.Instance.Send(regMsg, RegHandle);
            }
        }
    }

    private void RegHandle(BaseMsg baseMsg)
    {
        BoolMsg msg = baseMsg as BoolMsg;
        if (msg.result)
        {
            //注册成功
            UIManager.Instance.GetPanel("RegPanel").HideMe();
        }
        else
        {
            //注册失败
            Debug.Log("注册失败");
        }
    }
}
