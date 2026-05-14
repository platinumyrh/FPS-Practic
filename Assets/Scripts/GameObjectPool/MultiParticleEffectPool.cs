using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiParticleEffectPool : BaseObjectPool<MultiParticleEffectControl>
{
    private void Awake()
    {
        base.Awake();
        Prewarm(10); //预热10个多粒子特效对象
    }

    protected override void OnGet(MultiParticleEffectControl obj)
    {
        base.OnGet(obj);
        obj.ownerPool = this;//设置回收引用
    }

    protected override void OnRelease(MultiParticleEffectControl obj)
    {
        base.OnRelease(obj);
        //Debug.Log($"MultiParticleEffect returned to pool. Inactive: {pool.CountInactive}");
    }
    protected override void OnDestroyCall(MultiParticleEffectControl obj)
    {
        base.OnDestroyCall(obj);
        //Debug.Log("MultiParticleEffect destroyed.");
    }

    protected void Prewarm(int count)
    {
        List<MultiParticleEffectControl> effects = new List<MultiParticleEffectControl>();
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
