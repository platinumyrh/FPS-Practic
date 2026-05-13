using UnityEngine;

/// <summary>
/// 处理换弹动画事件的脚本，挂载在带有 Animator 的角色手上
/// 通过 PlayerShoot 找到当前的枪械，调用其 OnReloadComplete 方法
/// </summary>
public class ReloadAnimationEventHandler : MonoBehaviour
{
    private PlayerShoot playerShoot;

    private void Start()
    {
        playerShoot = GetComponentInParent<PlayerShoot>();
        if (playerShoot == null)
        {
            Debug.LogError("[ReloadAnimationEventHandler] 找不到 PlayerShoot 组件！请确保此脚本挂载在角色手上，且角色有 PlayerShoot 组件。");
        }
    }

    /// <summary>
    /// 动画事件调用的方法 - 换弹完成
    /// </summary>
    public void OnReloadComplete()
    {
        if (playerShoot != null && playerShoot.currentGun != null)
        {
            playerShoot.currentGun.OnReloadComplete();
            Debug.Log("[ReloadAnimationEventHandler] 换弹完成事件已触发");
        }
        else
        {
            Debug.LogWarning("[ReloadAnimationEventHandler] 无法完成换弹：playerShoot 或 currentGun 为空");
        }
    }
}
