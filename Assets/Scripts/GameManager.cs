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
        //NetMgrAsync.Instance().SendTest(Encoding.UTF8.GetBytes("������磬�ټ�����"));
        //Walk.Instance.StartGame("������");
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
            Debug.Log("�û���"+loginMsg.username+"����"+loginMsg.password);
            NetMgrAsync.Instance.Send(loginMsg,LoginHandle);
        }
    }
    private void LoginHandle(BaseMsg baseMsg)//����˷�����Ϣ�����
    {
        BoolMsg msg = baseMsg as BoolMsg;
        Debug.Log("��¼���" + msg.result);
        if (msg.result)
        {
            //��¼�ɹ�
            //Walk.Instance.StartGame(msg.id);
            EventCenter.Instance.EventTrigger<string, UnityAction>("ChangeScene", "SelectRoom",null);
        }
        else
        {
            //��¼ʧ��
            Debug.Log("��¼ʧ��");
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
            //ע��ɹ�
            UIManager.Instance.GetPanel("RegPanel").HideMe();
        }
        else
        {
            //ע��ʧ��
            Debug.Log("ע��ʧ��");
        }
    }
}
