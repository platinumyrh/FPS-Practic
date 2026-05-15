using System.Collections;
using UnityEngine;

public abstract class GunControl : MonoBehaviour
{
   
    [Header("弹药设置")]
    public int magazineSize = 30;          // 弹匣容量
    public int currentMagazineAmmo;        // 当前弹匣中的弹药量
    public int maxAmmo = 120;              // 最大弹药量
    public int currentAmmo;                // 当前弹药量
    public bool isReloading = false;

    [Header("伤害设置")]
    public int damage = 10;                // 每发子弹的伤害

    public Transform firePoint;              // 子弹发射点

    [Header("射速相关")]
    public float fireRate = 0.1f;          // 射击间隔时间
    protected float nextFireTime = 0f;      // 下一次可以射击的时间

    [Header("动画")]
    public Animator gunAnimator;            // 枪械动画控制器

    [Header("握持动画")]
    public RuntimeAnimatorController holdingAnimator;  // 这把枪对应的握持动画 Controller

    [Header("UI 资源")]  
    public Sprite weaponBodySprite;       // 枪身图片
    public Sprite magazineSprite;         // 弹匣图片
    public Sprite scopeSprite;            // 瞄准镜图片
    public bool useDotCrosshair = false;  // 是否使用点状准星

    [Header("散射设置")]
    [Tooltip("腰射时的基础散射角度")]
    public float hipFireSpreadAngle = 3f;

    [Tooltip("瞄准时的基础散射角度")]
    public float aimSpreadAngle = 0.5f;

    [Tooltip("每发子弹增加的散射角度")]
    public float spreadIncreasePerShot = 0.3f;

    [Tooltip("散射恢复速度（每秒减少的角度）")]
    public float spreadRecoverySpeed = 3f;

    [Tooltip("最大散射角度")]
    public float maxSpreadAngle = 10f;

    // 运行时变量
    protected float currentSpreadPenalty = 0f;  // 连续射击累积的额外散射
    protected float currentTotalSpread = 0f;    // 当前总散射角度

    PlayerControl playerControl;



    private void Start()
    {
        playerControl = GetComponentInParent<PlayerControl>();
    }
    protected virtual void Awake()
    {
        // 初始化弹药
        currentMagazineAmmo = magazineSize;
        currentAmmo = maxAmmo - magazineSize;


    }
    protected virtual void Update()
    {
        //散射恢复
        if (currentSpreadPenalty > 0f)
        {
            currentSpreadPenalty -= spreadRecoverySpeed * Time.deltaTime;
            currentSpreadPenalty = Mathf.Max(0f, currentSpreadPenalty);
        }
    }
    /// <summary>
    /// 尝试射击（带射速控制），由 PlayerShoot 调用
    /// </summary>
    public void TryShoot()
    {
      
        if (playerControl != null)
        {
            if (playerControl.isInspecting)
            {
                playerControl.StopInspecting();
            }
            if (!playerControl.CanFire())
            {
                return;
            }
        }

        // 如果正在换弹，打断换弹进入开火
        if (isReloading)
        {
            isReloading = false;
            // 可以在这里添加中断换弹的逻辑
            // 获取 Layer Actions 层的索引（从截图看应该是第3层，但建议用名称）
            int layerIndex =playerControl.curAnimator.GetLayerIndex("Layer Actions");
            // 播放 Default 状态，让该层回到空状态
            playerControl.curAnimator.Play("Default", layerIndex, 0f);
            Debug.Log("中断换弹");
            
        }

        if (Time.time < nextFireTime)
            return;  // 射速没到，不开枪

        if (currentMagazineAmmo <= 0)
        {
            //Debug.Log("弹匣空了，请重新装弹！");
           
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentMagazineAmmo--;
        OnShotFired();  // 增加散射惩罚
        UpdateUI();  // ← 射击后更新弹药UI
        Shoot();  // 调用子类具体的射击实现
        //发布开枪事件
        GameEventBus.instance.Publish(GameEventType.GunFired, new GunFiredEventData
        {
            Gun = this,
            FirePosition = firePoint.position,
            FireDirection = firePoint.forward
        });


    }
    public void OnReloadComplete()
    {
        if (!isReloading) return;

        // 计算需要装填的弹药量
        int ammoToReload = Mathf.Min(magazineSize - currentMagazineAmmo, currentAmmo);

        // 更新弹药
        currentMagazineAmmo += ammoToReload;
        currentAmmo -= ammoToReload;
        UpdateUI();

        // 发布事件
        GameEventBus.instance.Publish(GameEventType.GunReloaded, new GunReloadedEventData
        {
            Gun = this,
            position = transform.position,
            AmmoAdded = ammoToReload
        });

        isReloading = false;

        ResetSpread();
    }
    public virtual void Shoot()
    {
        gunAnimator.CrossFade("Fire", 0.05f, -1, 0f);
    }
    public virtual void Reload()
    {
        // 1. 检查条件
        if (currentAmmo <= 0)
        {
            Debug.Log("没有足够的弹药来重新装弹！");
            return;
        }

        if (isReloading)
        {
            Debug.Log("正在换弹中！");
            return;
        }

        // 2. 开始换弹
        isReloading = true;
        GameEventBus.instance.Publish(GameEventType.StartReload, new StartReloadEventData
        {
            Gun = this,
            position = transform.position,
           
        });
        gunAnimator.CrossFade("Reload", 0.05f, -1, 0f);

        // 注意：这里不更新弹药！等待动画事件触发 OnReloadComplete
    }

    public void UpdateUI()
    {
        if (WeaponUIController.Instance != null)
        {
            WeaponUIController.Instance.UpdateAmmo(currentMagazineAmmo, currentAmmo);
            WeaponUIController.Instance.UpdateWeaponImages(weaponBodySprite, magazineSprite, scopeSprite);
            WeaponUIController.Instance.SetCrosshair(useDotCrosshair);
        }
    }

    public bool AmmoIsEmpty()
    {
        return currentAmmo==0?false:true;
    }

    ///<summary>
    ///计算当前散射角度（基础角度+连射惩罚）
    ///
    protected void CaculateSpread()
    {
        float baseSpread = playerControl.isAiming ? aimSpreadAngle : hipFireSpreadAngle;

        currentTotalSpread = Mathf.Min(baseSpread + currentSpreadPenalty, maxSpreadAngle);
    }

    /// <summary>
    /// 射击时调用，增加散射惩罚
    /// </summary>
    protected void OnShotFired()
    {
        currentSpreadPenalty += spreadIncreasePerShot;
    }
    /// <summary>
    /// 获取散射射击方向
    ///<summary>
    protected Vector3 GetSpreadDirection()
    {
        CaculateSpread();
        
        if(currentTotalSpread<=0.001f)
            return firePoint.forward; // 没有散射，直接返回原方向

        // 在圆锥内随机取方向
        // 使用球面均匀分布算法
        float spreadRad = currentTotalSpread * Mathf.Deg2Rad;
        float randomRadius = Random.Range(0f, Mathf.Tan(spreadRad));
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);

        // 构建局部坐标系
        Vector3 forward = firePoint.forward;
        Vector3 up = firePoint.up;
        Vector3 right = firePoint.right;

        // 计算偏移
        float x = Mathf.Cos(randomAngle) * randomRadius;
        float y = Mathf.Sin(randomAngle) * randomRadius;

        Vector3 spreadDir = (forward + right * x + up * y).normalized;

        return spreadDir;
    }
    /// <summary>
    /// 获取带散射的旋转
    /// </summary>
    protected Quaternion GetSpreadRotation()
    {
        Vector3 direction = GetSpreadDirection();
        return Quaternion.LookRotation(direction);
    }
    /// <summary>
    /// 重置散射（换弹、切枪等情况下调用）
    /// </summary>
    public void ResetSpread()
    {
        currentSpreadPenalty = 0f;
        currentTotalSpread = 0f;
    }




}