using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilControl : MonoBehaviour
{
    public float recoilX = -3f;         // 每次开枪X轴后坐力
    public float horizontalRange = 2f;  // 水平随机范围
    public float snapSpeed = 15f;       // 后坐施加后自动恢复的速度

    private float currentRecoilX;
    private float currentRecoilY;

    private float prevRecoilX;
    private float prevRecoilY;

    void Update()
    {
        prevRecoilX = currentRecoilX;
        prevRecoilY = currentRecoilY;

        // 直接用 MoveTowards 衰减到 0，不会卡
        currentRecoilX = Mathf.MoveTowards(currentRecoilX, 0f, snapSpeed * Time.deltaTime);
        currentRecoilY = Mathf.MoveTowards(currentRecoilY, 0f, snapSpeed * Time.deltaTime);

        float deltaX = currentRecoilX - prevRecoilX;
        float deltaY = currentRecoilY - prevRecoilY;

        transform.localRotation *= Quaternion.Euler(deltaX, deltaY, 0f);
    }

    public void ApplyRecoil()
    {
        currentRecoilX += recoilX;
        currentRecoilY = Random.Range(-horizontalRange, horizontalRange);
    }
}