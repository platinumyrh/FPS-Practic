using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameObjectPool<T> where T : class
{
    List<T> pool;

    private Func<T> m_createFunc;
    private Action<T> m_actionOnGet;
    private Action<T> m_actionOnRelease;
    private Action<T> m_actionOnDestory;

    private int maxSize;

    public int CountAll { get; private set; }//总数量
    public int CountActive { get { return CountAll - CountInactive; } }//活跃数量
    public int CountInactive { get { return pool.Count; } }//pool内非活跃数量

    public bool collectionCheck = true;

    public GameObjectPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOndestory = null, int defualtSize = 10, int maxSize = 50)
    {
        if (createFunc == null)
        {
            throw new ArgumentNullException("createFunc");
        }
        if (maxSize <= 0)
        {
            throw new ArgumentException("Max Size must be greater than 0", "maxSize");
        }
        pool = new List<T>(defualtSize);
        m_createFunc = createFunc;
        m_actionOnGet = actionOnGet;
        m_actionOnRelease = actionOnRelease;
        m_actionOnDestory = actionOndestory;
        this.maxSize = maxSize;
    }


    public T Get()
    {
        T element;
        if (pool.Count == 0)
        {
            element = m_createFunc();
            CountAll++;
        }
        else
        {
            var index = pool.Count - 1;
            element = pool[index];
            pool.RemoveAt(index);
        }
        m_actionOnGet?.Invoke(element);
        return element;
    }
    public void Release(T element)
    {
        if (collectionCheck && pool.Count > 0)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (ReferenceEquals(pool[i], element))
                   // throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
                   return;
            }
        }
        m_actionOnRelease?.Invoke(element);
        if (CountInactive < maxSize)
        {
            pool.Add(element);
        }
        else
        {
            m_actionOnDestory?.Invoke(element);
        }
    }
}
