using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARGun : GunControl
{

    
    public override void Shoot()
    {
        // 不需要再判断弹药和射速了，基类 TryShoot 已经处理了
        if (PoolManager.Instance == null || PoolManager.Instance.bulletPool == null) return;

        base.Shoot();
        BulletControl bullet = PoolManager.Instance.bulletPool.Get();
        if (bullet != null)
        {
            Quaternion spreadRotation = GetSpreadRotation();
            bullet.Init(firePoint.position, spreadRotation);
        }

        
       // Debug.Log($"AR射击！弹匣: {currentMagazineAmmo}/{magazineSize}");
    }

    public override void Reload()
    {
       base.Reload();
    }
}
