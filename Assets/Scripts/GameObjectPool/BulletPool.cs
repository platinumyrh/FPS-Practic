using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : BaseObjectPool<BulletControl>
{

    private void Awake()
    {
        base.Awake();
        Prewarm(20); //预热20个子弹对象
    }

    protected override void OnGet(BulletControl obj)
    {
        base.OnGet(obj);
        obj.ownerPool = this;
        //Debug.Log($"Bullet retrieved from pool. Active: {pool.CountActive}");
        //后续新增额外对对象的处理逻辑
    }

    protected override void OnRelease(BulletControl bullet)
    {
        base.OnRelease(bullet);
        //Debug.Log($"Bullet returned to pool. Inactive: {pool.CountInactive}");
    }

    protected override void OnDestroyCall(BulletControl obj)
    {
        base.OnDestroyCall(obj);
        //Debug.Log("Bullet destroyed.");
    }

    //预热池
    public void Prewarm(int count)
    {
        List<BulletControl> bullets = new List<BulletControl>();
        for (int i = 0; i < count; i++)
        {
            bullets.Add(Get());
        }
        foreach (var bullet in bullets)
        {
            Release(bullet);
        }
    }
}
