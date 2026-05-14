using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFireEffectControl : MonoBehaviour
{
    //枪口火焰特效控制脚本
    public GameObject effectPrefab; //特效预制体
    private float effectDuration = 0.02f; //特效持续时间
    public GunFireEffectPool ownerPool; // 由池设置

    public void Init(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        // 将方向向量转换为旋转
        transform.rotation = Quaternion.LookRotation(direction);

        // 如果有 ParticleSystem，播放它
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        StartCoroutine(EffectLifeCycle());
    }


    IEnumerator EffectLifeCycle()
    {
        yield return new WaitForSeconds(effectDuration);
        if (ownerPool != null)
        {
            ownerPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
