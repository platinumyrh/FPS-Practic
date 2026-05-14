using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericEffectControl : MonoBehaviour
{
    public float duration = 0f;

    private ParticleSystem[] particleSystems;
    private float actualDuration;
    private System.Action<GenericEffectControl> onRelease;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }
    ///summary>
    ///初始化特效
    ///summary>
    public void Init(Vector3 position, Quaternion rotation, System.Action<GenericEffectControl> releaseCallback)
    {
        transform.SetPositionAndRotation(position, rotation);
        onRelease = releaseCallback;

        // 计算实际持续时间
        actualDuration = duration > 0 ? duration : CalculateMaxDuration();

        // 播放所有粒子
        foreach (var ps in particleSystems)
        {
            ps.Clear(true);
            ps.Play(true);
        }

        StartCoroutine(LifeCycle());
    }

    /// <summary>
    /// 自动计算最长粒子持续时间
    /// </summary>
    private float CalculateMaxDuration()
    {
        float max = 0f;
        foreach (var ps in particleSystems)
        {
            float d = ps.main.duration + ps.main.startLifetime.constantMax;
            max = Mathf.Max(max, d);
        }
        return max > 0 ? max : 2f; // 保底2秒
    }
    IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(actualDuration);

        // 停止所有粒子
        foreach (var ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // 回调回收
        onRelease?.Invoke(this);
    }
}
