using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitEventData : GameEventData
{
    public Vector3 HitPosition;      // 击中位置
    public Vector3 HitNormal;        // 击中法线（用于旋转特效）
    public GameObject HitObject;     // 被击中的物体
    public string HitTag;            // 被击中物体的标签
}
