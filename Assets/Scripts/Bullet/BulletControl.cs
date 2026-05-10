using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class BulletControl : MonoBehaviour
{
    private float speed = 30f;
    private Rigidbody rb;
     public BulletPool ownerPool; // 由池设置

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Init(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.velocity = transform.forward * speed;  // 直接设置速度
        }
        Physics.SyncTransforms();//强制同步速度
        CancelInvoke("AutoRelease");
        Invoke("AutoRelease", 5f);
    }

    private void OnCollisionEnter(Collision collision)
    { 
       
        if (collision.gameObject.CompareTag("Destroyable"))
        {
            Debug.Log($"Bullet hit destroyable object: {collision.gameObject.name}");
            Rigidbody hitRb = collision.gameObject.AddComponent<Rigidbody>();
            hitRb.AddForceAtPosition(transform.forward * 10f, collision.contacts[0].point, ForceMode.Impulse);

            Destroy(collision.gameObject, 20f);
        }
        AutoRelease();
    }
    private void AutoRelease()
    {
        CancelInvoke("AutoRelease");

        if (ownerPool != null)
        {
            ownerPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        CancelInvoke("AutoRelease");
    }
}

