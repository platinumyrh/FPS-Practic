﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GenericEffectPool : MonoBehaviour
{
    [System.Serializable]
    public class EffectConfig
    {
        [Tooltip("特效名称（如 GunFireEffect, Explosion, Blood）")]
        public string effectName;

        [Tooltip("特效预制体")]
        public GameObject prefab;

        [Tooltip("默认容量")]
        public int defaultCapacity = 10;

        [Tooltip("最大容量")]
        public int maxSize = 50;
    }

    [Tooltip("特效配置列表")]
    [SerializeField]
    public List<EffectConfig> effectConfigs = new List<EffectConfig>();

    //名称 -> 对象池 的字典
    private Dictionary<string,ObjectPool<GenericEffectControl>> pools = new Dictionary<string, ObjectPool<GenericEffectControl>>();

    // 名称 -> 预制体 的字典
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var config in effectConfigs)
        {
            if (config.prefab == null) continue;

            prefabs[config.effectName] = config.prefab;

            var pool = new ObjectPool<GenericEffectControl>(
                createFunc: () => CreateEffect(config.effectName),
                actionOnGet: OnGetEffect,
                actionOnRelease: OnReleaseEffect,
                actionOnDestroy: OnDestroyEffect,
                collectionCheck: true,
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxSize
            );

            pools[config.effectName] = pool;
        }
    }

    // 存储每种特效的父物体，用于组织场景层级
    private Dictionary<string, Transform> effectParents = new Dictionary<string, Transform>();

    private GenericEffectControl CreateEffect(string effectName)
    {
        if (!prefabs.TryGetValue(effectName, out var prefab)) return null;

        var obj = Instantiate(prefab);

        // 设置父物体为 GenericEffectPool，保持场景整洁
        if (!effectParents.ContainsKey(effectName))
        {
            // 创建一个空的父物体来存放同类型的特效
            var parent = new GameObject($"{effectName} Pool");
            parent.transform.SetParent(transform);
            parent.transform.localPosition = Vector3.zero;
            parent.transform.localRotation = Quaternion.identity;
            effectParents[effectName] = parent.transform;
        }
        obj.transform.SetParent(effectParents[effectName], false);

        obj.SetActive(false);
        return obj.GetComponent<GenericEffectControl>() ?? obj.AddComponent<GenericEffectControl>();
    }

    private void OnGetEffect(GenericEffectControl effect)
    {
        effect.gameObject.SetActive(true);
    }

    private void OnReleaseEffect(GenericEffectControl effect)
    {
        effect.gameObject.SetActive(false);
    }

    private void OnDestroyEffect(GenericEffectControl effect)
    {
        Destroy(effect.gameObject);
    }
    /// <summary>
    /// 播放特效
    /// </summary>
    /// <param name="effectName">特效名称</param>
    /// <param name="position">位置</param>
    /// <param name="rotation">旋转</param>
    public void Play(string effectName, Vector3 position, Quaternion rotation)
    {
        if (!pools.TryGetValue(effectName, out var pool))
        {
            Debug.LogWarning($"特效池不存在: {effectName}");
            return;
        }

        var effect = pool.Get();
        if (effect != null)
        {
            effect.Init(position, rotation, (e) => pool.Release(e));
        }
    }

    /// <summary>
    /// 播放特效（传入方向自动计算旋转）
    /// </summary>
    public void Play(string effectName, Vector3 position, Vector3 direction)
    {
        Play(effectName, position, Quaternion.LookRotation(direction));
    }
}


