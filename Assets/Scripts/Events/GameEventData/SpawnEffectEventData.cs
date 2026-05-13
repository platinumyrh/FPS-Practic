using UnityEngine;

public class SpawnEffectEventData : GameEventData
{
    public GameObject EffectPrefab { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; } = Quaternion.identity;
    public float DestroyDelay { get; set; } = 2f;
}
