using UnityEngine;

public abstract class GunControl : MonoBehaviour
{
   
    [Header("弹药设置")]
    public int magazineSize = 30;          // 弹匣容量
    public int currentMagazineAmmo;        // 当前弹匣中的弹药量
    public int maxAmmo = 120;              // 最大弹药量
    public int currentAmmo;                // 当前弹药量

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
        if (Time.time < nextFireTime)
            return;  // 射速没到，不开枪

        if (currentMagazineAmmo <= 0)
        {
            Debug.Log("弹匣空了，请重新装弹！");
            Reload();
            return;
        }

        nextFireTime = Time.time + fireRate;
        Shoot();  // 调用子类具体的射击实现
    }
    public abstract void Shoot();          // 射击方法，子类必须实现
    public abstract void Reload();         // 重新装弹方法，子类必须实现
}