using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObjectPool<T>:MonoBehaviour where T: Component
{
   protected GameObjectPool<T> pool;
    [SerializeField] protected T prefab;
    [SerializeField] protected int defaultSize = 10;
    [SerializeField] protected int maxSize = 50;

    protected Transform poolRoot;

    protected virtual void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError($"请将 {typeof(T).Name} 的Prefab拖到 {gameObject.name} 的 {GetType().Name} 组件上！");
        }
        poolRoot = new GameObject($"{typeof(T).Name}Pool").transform;
        poolRoot.SetParent(transform);
       

        InitializePool();

    }

    protected virtual void InitializePool()
    {
            pool = new GameObjectPool<T>(
                createFunc: CreatePooledItem,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOndestory: OnDestroyCall,
                defualtSize: defaultSize,
                maxSize: maxSize
            );
    }

    protected virtual T CreatePooledItem()
    {
        T newObj = Instantiate(prefab, poolRoot);
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    protected virtual void OnGet(T obj)
    {
        obj.gameObject.SetActive(true);
    }

    protected virtual void OnRelease(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    protected virtual void OnDestroyCall(T obj)
    {
        Destroy(obj.gameObject);
    }

    public virtual T Get()
    {
        return pool.Get();
    }

    public virtual void Release(T obj)
    {
        pool.Release(obj);
    }
}
