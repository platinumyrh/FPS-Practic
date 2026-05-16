﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
  public static SoundManager Instance { get; private set; }


    [Header("全局设置")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;


    [Header("音频源")]
    [Tooltip("2D音效播放器（UI、背景音乐等）")]
    public AudioSource bgmSource;

    [Tooltip("3D音效播放器池")]
    public int maxSFXSources = 10;
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();
    
    // 追踪正在使用的音源（用于停止特定类型的音效）
    private List<AudioSource> activeSources = new List<AudioSource>();

    [Header("音效库")]
    [Tooltip("音效配置列表")]
    [SerializeField]
    public List<SoundConfig> soundConfigs = new List<SoundConfig>();

    private Dictionary<string,AudioClip> soundLibrary = new Dictionary<string,AudioClip>();

    [System.Serializable]
    public class SoundConfig
    {
        [Tooltip("音效名称")]
        public string soundName;

        [Tooltip("音频片段")]
        public AudioClip clip;

        [Tooltip("音量")]
        [Range(0f, 1f)]
        public float volume = 1f;

        [Tooltip("是否是3D音效")]
        public bool is3D = true;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 初始化音效库
        InitializeSoundLibrary();

        // 初始化3D音效池
        InitializeSFXPool();
    }

    private void InitializeSoundLibrary()
    {
        foreach (var config in soundConfigs)
        {
            if (!soundLibrary.ContainsKey(config.soundName))
            {
                soundLibrary.Add(config.soundName, config.clip);
            }
            else
            {
                Debug.LogWarning($"音效名称重复: {config.soundName}");
            }
        }
    }
    private void InitializeSFXPool()
    {
        for (int i = 0; i < maxSFXSources; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxPool.Enqueue(source);
        }
    }

    private void Start()
    {
        SubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();

        if (Instance == this)
        {
            Instance = null;
        }
    }
    #region 事件订阅

    private void SubscribeEvents()
    {
        if (GameEventBus.instance == null) return;

        GameEventBus.instance.Subscribe<GunFiredEventData>(GameEventType.GunFired, OnGunFired);
        GameEventBus.instance.Subscribe<StartReloadEventData>(GameEventType.StartReload, OnGunReloaded);
        GameEventBus.instance.Subscribe<BulletHitEventData>(GameEventType.BulletHit, OnBulletHit);
        GameEventBus.instance.Subscribe<StartAimingEventData>(GameEventType.StartAiming, OnStartAiming);
        GameEventBus.instance.Subscribe<HolsterWeapon>(GameEventType.HolsterWeapon, OnHolster);
        GameEventBus.instance.Subscribe<UnHolsterWeapon>(GameEventType.UnHolsterWeapon, OnUnHolster);
        //GameEventBus.instance.Subscribe<PlayerMoveEventData>(GameEventType.PlayerMove, OnPlayerMove);
        //GameEventBus.instance.Subscribe<PlayerRunEventData>(GameEventType.PlayerRun, OnPlayerRun);
    }

    private void UnsubscribeEvents()
    {
        if (GameEventBus.instance == null) return;

        GameEventBus.instance.Unsubscribe<GunFiredEventData>(GameEventType.GunFired, OnGunFired);
        GameEventBus.instance.Unsubscribe<StartReloadEventData>(GameEventType.StartReload, OnGunReloaded);
        GameEventBus.instance.Unsubscribe<BulletHitEventData>(GameEventType.BulletHit, OnBulletHit);
        GameEventBus.instance.Unsubscribe<StartAimingEventData>(GameEventType.StartAiming, OnStartAiming);
        GameEventBus.instance.Unsubscribe<HolsterWeapon>(GameEventType.HolsterWeapon, OnHolster);
        GameEventBus.instance.Unsubscribe<UnHolsterWeapon>(GameEventType.UnHolsterWeapon, OnUnHolster);
        //GameEventBus.instance.Unsubscribe<PlayerMoveEventData>(GameEventType.PlayerMove, OnPlayerMove);
        //GameEventBus.instance.Unsubscribe<PlayerRunEventData>(GameEventType.PlayerRun, OnPlayerRun);
    }

    #endregion

    #region 事件回调

    private void OnGunFired(GunFiredEventData data)
    {
        // 根据枪械类型播放不同音效（可选）
        string gunName = data.Gun.name;

        // 简单做法：统一播放枪声音效
       // PlaySound3D("GunFire", data.FirePosition);

        // 或者根据枪的类型播放不同音效
        Debug.Log($"枪械开火事件: {gunName} 在位置 {data.FirePosition}");
        PlaySound3D($"{gunName}_Fire", data.FirePosition);
    }
    private void OnGunReloaded(StartReloadEventData data)
    {
        string gunName = data.Gun.name;
       // PlaySound3D("Reload", data.Gun.transform.position);
        PlaySound3D($"{gunName}_Reload",data.position);
    }

    private void OnBulletHit(BulletHitEventData data)
    {
        switch (data.HitTag)
        {
            case "Destroyable":
                PlaySound3D("Explosion", data.HitPosition);
                break;
            case "Enemy":
                PlaySound3D("Hit_Flesh", data.HitPosition);
                break;
            case "Metal":
                PlaySound3D("Hit_Metal", data.HitPosition);
                break;
            default:
                PlaySound3D("DefaultBulletHit", data.HitPosition);
                break;
        }
    }
    private void OnStartAiming(StartAimingEventData data)
    {
        PlaySound3D("Aiming",data.position );
    }
    private void OnHolster(HolsterWeapon data)
    {
        PlaySound3D("Holster", data.position);
    }
    private void OnUnHolster(UnHolsterWeapon data)
    {
        PlaySound3D("UnHolster", data.position);
    }

    private void OnPlayerMove(PlayerMoveEventData data)
    {
        // 可选：根据玩家移动状态播放脚步声
        PlaySound3D("Walk", data.position);
    }
    private void OnPlayerRun(PlayerRunEventData data)
    {
        // 可选：根据玩家跑步状态播放跑步声
        PlaySound3D("Run", data.position);
    }


    #endregion
    #region 公共方法

    /// <summary>
    /// 播放2D音效（全局，无位置）
    /// </summary>
    public void PlaySound2D(string soundName)
    {
        if (!soundLibrary.TryGetValue(soundName, out var clip))
        {
            Debug.LogWarning($"音效不存在: {soundName}");
            return;
        }

        var config = GetConfig(soundName);

        // 最终音量 = 配置音量 × 全局音量
        float finalVolume = (config?.volume ?? 1f) * masterVolume;
        bgmSource.PlayOneShot(clip, finalVolume);
    }

    public void PlaySound3D(string soundName, Vector3 position)
    {
        if (!soundLibrary.TryGetValue(soundName, out var clip))
        {
            Debug.LogWarning($"音效不存在: {soundName}");
            return;
        }

        var config = GetConfig(soundName);

        // 从池获取可用的 AudioSource
        AudioSource source = GetAvailableSource();

        if (source != null)
        {
            // 设置位置和属性
            source.transform.position = position;
            source.spatialBlend = 1f;
            source.clip = clip;

            // 最终音量 = 配置音量 × 全局音量
            source.volume = (config?.volume ?? 1f) * masterVolume;

            // 追踪正在使用的音源
            activeSources.Add(source);

            source.Play();

            // 播放完成后回收
            StartCoroutine(ReturnSourceWhenDone(source));
        }
        else
        {
            // 池满了，用 PlayClipAtPoint
            float finalVolume = (config?.volume ?? 1f) * masterVolume;
            AudioSource.PlayClipAtPoint(clip, position, finalVolume);
        }
    }

    /// <summary>
    /// 停止指定名称的所有音效
    /// </summary>
    public void StopSound(string soundName)
    {
        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            var source = activeSources[i];
            
            // 检查是否匹配（通过 clip 名称或 soundName 标签）
            if (source.clip != null && 
                (source.clip.name.Contains(soundName) || source.clip.name == soundName))
            {
                source.Stop();
                activeSources.RemoveAt(i);
                sfxPool.Enqueue(source);
            }
        }
    }

    /// <summary>
    /// 停止所有正在播放的音效
    /// </summary>
    public void StopAllSounds()
    {
        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            var source = activeSources[i];
            source.Stop();
            activeSources.RemoveAt(i);
            sfxPool.Enqueue(source);
        }
    }

    /// <summary>
    /// 从池中获取一个可用的 AudioSource
    /// </summary>
    private AudioSource GetAvailableSource()
    {
        // 从池中取出空闲的
        if (sfxPool.Count > 0)
        {
            return sfxPool.Dequeue();
        }

        // 池空了返回 null
        return null;
    }

    /// <summary>
    /// 等待播放结束后回收 AudioSource
    /// </summary>
    private System.Collections.IEnumerator ReturnSourceWhenDone(AudioSource source)
    {
        // 等待音频实际播放完成
        while (source != null && source.isPlaying)
        {
            yield return null;
        }

        // 额外等待一小段时间确保完全结束
        yield return new WaitForSeconds(0.05f);

        // 从活跃列表移除并回收到池
        if (source != null)
        {
            activeSources.Remove(source);
            sfxPool.Enqueue(source);
        }
    }

    private SoundConfig GetConfig(string soundName)
    {
        return soundConfigs.Find(c => c.soundName == soundName);
    }

    #endregion
}
