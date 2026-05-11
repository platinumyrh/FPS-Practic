using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


using UnityEngine.UI;

public class WeaponUIController : MonoBehaviour
{
    // 弹药显示
    [Header("弹药UI")]
    public TextMeshProUGUI textCurrentAmmo;      // Text Ammunition Current
    public TextMeshProUGUI textTotalAmmo;        // Text Ammunition Total

    // 枪械图片显示
    [Header("枪械图片")]
    public Image imageWeaponBody;     // Image Weapon Body
    public Image imageMagazine;       // Image Weapon Attachment Magazine
    public Image imageScope;          // Image Weapon Attachment Scope Default
    public Image imageLine;           // Image Line

    // 准星显示
    [Header("准星")]
    public GameObject crosshairClassic;
    public GameObject crosshairDot;

    //单例模式 方便访问
    public static WeaponUIController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); 
    }

    public void UpdateAmmo(int current, int total)
    {
        textCurrentAmmo.text = current.ToString();
        textTotalAmmo.text = total.ToString();
    }
    public void UpdateWeaponImages(Sprite body, Sprite magazine, Sprite scope)
    {
        if (imageWeaponBody != null) imageWeaponBody.sprite = body;
        if (imageMagazine != null) imageMagazine.sprite = magazine;
        if (imageScope != null) imageScope.sprite = scope;
    }

    // 切换准星
    public void SetCrosshair(bool useDot)
    {
        if (crosshairClassic != null) crosshairClassic.SetActive(!useDot);
        if (crosshairDot != null) crosshairDot.SetActive(useDot);
    }
}
