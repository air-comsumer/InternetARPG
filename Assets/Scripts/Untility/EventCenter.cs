using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public interface IEventInfo { }
public class EventInfo : IEventInfo
{
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}
public class EventInfo<T1, T2> : IEventInfo
{
    public UnityAction<T1, T2> actions;
    public EventInfo(UnityAction<T1, T2> action)
    {
        actions += action;
    }
}
[DefaultExecutionOrder(-1)]
public class EventCenter : SingletonMono<EventCenter>
{
    public Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();
    public void AddEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }
    public void AddEventListener<T1,T2>(string name, UnityAction<T1, T2> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1, T2>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T1, T2>(action));
        }
    }
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;
        }
    }
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }
    public void RemoveEventListener<T1, T2>(string name, UnityAction<T1, T2> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T1, T2>).actions -= action;
        }
    }
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            Debug.Log("执行了事件" + name);
            Debug.Log(name);
            if ((eventDic[name] as EventInfo).actions != null)
            {
                (eventDic[name] as EventInfo).actions.Invoke();
            }
        }
    }
    public void EventTrigger<T>(string name, T info)
    {
        Debug.Log("执行了事件"+name);
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo<T>).actions != null)
            {
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
            }
        }
    }
    public void EventTrigger<T1,T2>(string name, T1 info1, T2 info2)
    {
        Debug.Log("执行了事件" + name);
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo<T1, T2>).actions != null)
            {
                (eventDic[name] as EventInfo<T1, T2>).actions.Invoke(info1, info2);
            }
        }
    }
}
