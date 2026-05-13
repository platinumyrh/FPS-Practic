using UnityEngine;

public class DamageEventData : GameEventData
{
    public GameObject Target { get; set; }
    public GameObject Attacker { get; set; }
    public int DamageAmount { get; set; }
    public Vector3 HitPosition { get; set; }
}