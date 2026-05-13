﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    public GunControl currentGun;            // Inspector 里拖入初始武器

    private List<GunControl> guns = new List<GunControl>();
    private int currentGunIndex = 0;
    private Animator animator;
    public PlayerControl playerControl;



    private void Awake()
    {
      

        playerControl = GetComponent<PlayerControl>();

        // 自动获取所有武器组件
        GunControl[] foundGuns = GetComponentsInChildren<GunControl>(includeInactive: true);
        guns.AddRange(foundGuns);

        if (currentGun == null && guns.Count > 0)
            currentGun = guns[0];
    }
    private void OnEnable()
    {
        GameEventBus.instance.Subscribe<GunFiredEventData>(GameEventType.GunFired, OnGunFired);
        GameEventBus.instance.Subscribe<GunReloadedEventData>(GameEventType.GunReloaded, OnGunReloaded);
    }
    private void OnDisable()
    {
        GameEventBus.instance.Unsubscribe<GunFiredEventData>(GameEventType.GunFired, OnGunFired);
        GameEventBus.instance.Unsubscribe<GunReloadedEventData>(GameEventType.GunReloaded, OnGunReloaded);
    }
    void OnGunFired(GunFiredEventData data)
    {
        playerControl.curAnimator.CrossFade("Fire", 0.01f, -1, 0f);
       // Debug.Log($"开火动画触发: {data.Gun.name}");
    }

    void OnGunReloaded(GunReloadedEventData data)
    {
       
      
       // Debug.Log($"换弹动画触发: {data.Gun.name}, 装填了 {data.AmmoAdded} 发");
    }
    void Update()
    {
        // 按住开火 = 连续射击，射速由 TryShoot 内部控制
        if (Input.GetButton("Fire1"))
        {
            
           // playerControl.curAnimator.CrossFade("Fire", 0.01f, -1, 0f);
            currentGun.TryShoot();//通过tryshoot内的publish事件来触发上面的ingunfired触发动画
        }

        // 数字键切换武器
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchGun(2);

        // 滚轮切换武器
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
            SwitchGun(currentGunIndex - 1);
        else if (scroll < 0f)
            SwitchGun(currentGunIndex + 1);

        // 换弹
        if (Input.GetKeyDown(KeyCode.R))
        {
          
            if (currentGun.currentMagazineAmmo < currentGun.magazineSize)
            {
                currentGun.Reload();
                playerControl.curAnimator.CrossFade("Reload", 0.01f, -1, 0f);
                
            }
            else
                Debug.Log("弹药已满，无需换弹！");

        }
        //检视

    }

    void SwitchGun(int index)
    {
        if (guns.Count == 0) return;
        if (index < 0 || index >= guns.Count) return;
        if (index == currentGunIndex) return;

        StartCoroutine(SwitchGunRoutine(index));
    }
    IEnumerator SwitchGunRoutine(int index)
    {
        // 1. 收枪
        playerControl.SetHolster(true);

        // 等收枪动画播放一段时间（根据你的动画时长调整）
        yield return new WaitForSeconds(0.3f);

        // 2. 切换武器
        currentGun.gameObject.SetActive(false);
        currentGunIndex = index;
        currentGun = guns[currentGunIndex];
        currentGun.gameObject.SetActive(true);
        

        // 3. 换握持动画
        if (playerControl != null && currentGun.holdingAnimator != null)
        {
            playerControl.SwitchHoldingAnimation(currentGun.holdingAnimator);
        }
        currentGun.UpdateUI();

        // 4. 拔枪
        playerControl.SetHolster(false);

        //Debug.Log($"切换到: {currentGun.name}");
    }
}