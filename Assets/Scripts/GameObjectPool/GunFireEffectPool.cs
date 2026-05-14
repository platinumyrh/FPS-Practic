using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFireEffectPool :BaseObjectPool<GunFireEffectControl>
{


    private void Awake()
    {
        base.Awake();   
        Prewarm(20); //预热20个枪口火焰特效对象
    }


    protected override void OnGet(GunFireEffectControl obj)
    {
        base.OnGet(obj);
        obj.ownerPool = this;//设置回收引用
    }

    protected override void OnRelease(GunFireEffectControl obj)
    {
            base.OnRelease(obj);
        //Debug.Log($"GunFireEffect returned to pool. Inactive: {pool.CountInactive}");
    }
    protected override void OnDestroyCall(GunFireEffectControl obj)
    {
            base.OnDestroyCall(obj);
        //Debug.Log("GunFireEffect destroyed.");
    }
    public void Prewarm(int count)
    {
        List<GunFireEffectControl> effects = new List<GunFireEffectControl>();
        for (int i = 0; i < count; i++)
        {
            effects.Add(Get());
        }
        foreach (var effect in effects)
        {
            Release(effect);
        }
    }
}
