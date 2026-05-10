using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager Instance { get; private set; }
    // 各个池的引用
    public BulletPool bulletPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        bulletPool = GetComponent<BulletPool>();
    }
    private void Start()
    {
        
    }

   
}
