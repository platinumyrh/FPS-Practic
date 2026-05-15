// GameEventType.cs - 事件类型定义
public enum GameEventType
{
    // 枪械相关
    GunFired,           // 开枪
    GunReloaded,        // 换弹完成
    GunSwitched,        // 切换武器
    BulletHit,          // 子弹击中
    StartReload,         // 开始换弹
    StartAiming,         // 开始瞄准
    HolsterWeapon,        // 收起武器
    UnHolsterWeapon,     // 拿出武器

    //玩家相关
    PlayerMove,          // 玩家移动
    PlayerRun,            // 玩家奔跑


    // 音效相关
    PlaySound,          // 播放音效
    PlayMusic,          // 播放音乐

    // 特效相关
    SpawnEffect,        // 生成特效

    // 战斗相关
    DamageDealt,        // 造成伤害
    EnemyHit,           // 击中敌人
    PlayerHit,          // 玩家受伤
    EnemyDeath,         // 敌人死亡

    // UI相关
    UpdateAmmoUI,       // 更新弹药UI
    ShowNotification,   // 显示通知

    // 游戏状态
    GameStarted,        // 游戏开始
    GamePaused,         // 游戏暂停
    GameResumed,        // 游戏继续
    GameOver            // 游戏结束
}