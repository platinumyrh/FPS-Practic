using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFiredEventData : GameEventData
{
    public GunControl Gun { get; set; }
    public Vector3 FirePosition { get; set; }
    public Vector3 FireDirection { get; set; }
}
