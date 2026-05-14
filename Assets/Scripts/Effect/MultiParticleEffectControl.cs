using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiParticleEffectControl : MonoBehaviour
{
  public float effectDuration = 1f; //特效持续时间
  public MultiParticleEffectPool ownerPool; //回收引用

  private ParticleSystem[] particleSystems;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }

    public void Init(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        //播放所有子粒子系统
        foreach (var ps in particleSystems)
        {
            ps.Clear();//清除残留粒子
            ps.Play();
        }
        StartCoroutine(EffectLifeCycle());


    }

    IEnumerator EffectLifeCycle()
    {
        // 等待最长时间的粒子播放完
        float maxDuration = 0f;
        foreach (var ps in particleSystems)
        {
            float duration = ps.main.duration + ps.main.startLifetime.constant;
            maxDuration = Mathf.Max(maxDuration, duration);
        }

        yield return new WaitForSeconds(maxDuration);

        // 回收或销毁
        if (ownerPool != null)
        {
            ownerPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 停止所有粒子（回收前调用）
    /// </summary>
    public void StopAll()
    {
        foreach (var ps in particleSystems)
        {
            ps.Stop();
        }
    }
}
