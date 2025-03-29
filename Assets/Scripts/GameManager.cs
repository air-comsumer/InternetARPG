using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        NetMgrAsync.Instance().Connect("127.0.0.1", 12345);
    }
    // Start is called before the first frame update
    void Start()
    {
        //NetMgrAsync.Instance().SendTest(Encoding.UTF8.GetBytes("������磬�ټ�����"));
        Walk.Instance().StartGame("������");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
    }
}
