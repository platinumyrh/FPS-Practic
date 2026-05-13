using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameEventBus : MonoBehaviour
{
    public static GameEventBus instance { get; private set; }
    /// <summary>
    /// 事件字典 ：事件类型-》回调列表
    /// </summary>
    private Dictionary<GameEventType,List<Delegate>>  eventListeners = new Dictionary<GameEventType, List<Delegate>>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    //订阅事件 把观察者的回调函数添加到事件字典中
    public void Subscribe<T>(GameEventType eventType, Action<T> listener)
    where T : GameEventData
    {
        if (!eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType] = new List<Delegate>();
        }
        eventListeners[eventType].Add(listener);
    }

    //取消订阅事件 从事件字典中移除观察者的回调函数
    public void Unsubscribe<T>(GameEventType eventType, Action<T> listener) where T : GameEventData
    {
        if (eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType].Remove(listener);
        }
    }

    //发布事件 触发事件字典中对应事件类型的所有回调函数(含参)
    public void Publish<T>(GameEventType eventType, T eventData) where T : GameEventData
    {
        if (eventListeners.TryGetValue(eventType, out var listeners))
        {
            foreach (var listener in listeners)
            {
                (listener as Action<T>)?.Invoke(eventData);
            }
        }
    }
    // 简化版发布（无参数事件）
    public void Publish(GameEventType eventType)
    {
        Publish(eventType, new EmptyEventData());
    }
}
