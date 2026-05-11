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




    protected virtual void Awake()
    {
        // 初始化弹药
        currentMagazineAmmo = magazineSize;
        currentAmmo = maxAmmo - magazineSize;
    }
    /// <summary>
    /// 尝试射击（带射速控制），由 PlayerShoot 调用
    /// </summary>
    public void TryShoot()
    {
        PlayerControl playerControl = GetComponentInParent<PlayerControl>();
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
        }

        if (Time.time < nextFireTime)
            return;  // 射速没到，不开枪

        if (currentMagazineAmmo <= 0)
        {
            Debug.Log("弹匣空了，请重新装弹！");
           
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentMagazineAmmo--;
        UpdateUI();  // ← 射击后更新弹药UI
        Shoot();  // 调用子类具体的射击实现
        
        
    }
    public virtual void Shoot()
    {
        gunAnimator.CrossFade("Fire", 0.05f, -1, 0f);
    }
    public virtual void Reload()
    {
        if (currentAmmo > 0)
        {
            // 计算需要装填的弹药量
            int ammoToReload = magazineSize - currentMagazineAmmo;
            if (ammoToReload > currentAmmo)
            {
                ammoToReload = currentAmmo; // 如果剩余弹药不足以装满弹匣，则只装剩余的弹药
            }
            // 执行重新装弹逻辑，例如播放装弹动画等
            Debug.Log("枪重新装弹！");
            isReloading = true;
            gunAnimator.CrossFade("Reload", 0.05f, -1, 0f);
           
            // 更新当前弹匣中的弹药量和总弹药量
            currentMagazineAmmo += ammoToReload;
            currentAmmo -= ammoToReload;
            UpdateUI();
            isReloading = false ;
        }
        else
        {
            Debug.Log("没有足够的弹药来重新装弹！");
        }
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

  
}