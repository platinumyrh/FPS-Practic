using UnityEngine;

public class PlaySoundEventData : GameEventData
{
    public AudioClip Clip { get; set; }
    public Vector3 Position { get; set; }
    public float Volume { get; set; } = 1f;
    public bool Is3D { get; set; } = true;
}
