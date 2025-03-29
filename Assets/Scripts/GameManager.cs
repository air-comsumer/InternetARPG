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
        //NetMgrAsync.Instance().SendTest(Encoding.UTF8.GetBytes("你好世界，再见世界"));
        Walk.Instance().StartGame("高潇屹");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
    }
}
