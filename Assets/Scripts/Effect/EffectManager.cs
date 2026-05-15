using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{

    public static EffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        //GameEventBus.instance.Subscribe<GunFiredEventData>(GameEventType.GunFired, OnGunFired);
    }
    private void Start()
    {
        GameEventBus.instance.Subscribe<GunFiredEventData>(GameEventType.GunFired, OnGunFired);
        GameEventBus.instance.Subscribe<BulletHitEventData>(GameEventType.BulletHit, OnBulletHit);
    }
    private void OnDisable()
    {
        GameEventBus.instance.Unsubscribe<GunFiredEventData>(GameEventType.GunFired, OnGunFired);
        GameEventBus.instance.Unsubscribe<BulletHitEventData>(GameEventType.BulletHit, OnBulletHit);
    }

    /// <summary>
    /// 开火事件回调，触发枪口火焰特效
    /// </summary>
    /// <param name="data"></param>
    private void OnGunFired(GunFiredEventData data)
    {
        PoolManager.Instance.effectPool.Play("GunFireEffect", data.FirePosition, data.FireDirection);
       // Debug.Log($"开火特效触发: {data.Gun.name} at {data.FirePosition}");
    }




    /// <summary>
    /// 命中反馈回调，触发击中效果
    /// summary>
    private void OnBulletHit(BulletHitEventData data)
    {
        switch (data.HitTag)
        {
            case "Destroyable":
                PoolManager.Instance.effectPool.Play("Explosion", data.HitPosition, data.HitNormal);
                break;
            case "Enemy":
                PoolManager.Instance.effectPool.Play("Blood", data.HitPosition, data.HitNormal);
                break;
            case "Metal":
                PoolManager.Instance.effectPool.Play("Spark", data.HitPosition, data.HitNormal);
                break;
            default:
                PoolManager.Instance.effectPool.Play("BulletFeedback", data.HitPosition, data.HitNormal);
                break;
        }
       // Debug.Log($"命中特效触发: {data.HitTag} at {data.HitPosition}");
    }
    

}
