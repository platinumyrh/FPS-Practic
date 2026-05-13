using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunReloadedEventData : GameEventData
{
    public GunControl Gun { get; set; }
    public int AmmoAdded { get; set; }
}
