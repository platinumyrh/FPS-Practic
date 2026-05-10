using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGun : GunControl
{


    public override void Shoot()
    {
        // 不需要再判断弹药和射速了，基类 TryShoot 已经处理了
        if (PoolManager.Instance == null || PoolManager.Instance.bulletPool == null) return;

        BulletControl bullet = PoolManager.Instance.bulletPool.Get();
        if (bullet != null)
        {
            bullet.Init(firePoint.position, firePoint.rotation);
        }

        currentMagazineAmmo--;
        Debug.Log($"手枪射击！弹匣: {currentMagazineAmmo}/{magazineSize}");
    }

    public override void Reload()
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
            Debug.Log("手枪枪重新装弹！");
            // 更新当前弹匣中的弹药量和总弹药量
            currentMagazineAmmo += ammoToReload;
            currentAmmo -= ammoToReload;
        }
        else
        {
            Debug.Log("没有足够的弹药来重新装弹！");
        }
    }
}
