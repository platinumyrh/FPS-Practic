using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Windows;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    public Animator curAnimator;

    

    private PlayerShoot playerShoot;


    //移动速度
    private float moveSpeed = 5f;

    private bool isRunning = false;
    //跳跃参数
    private float jumpForce = 8f;
    private bool isJumping = false; 

    //视角灵敏度
    private float xSensitivity = 5f;
    private float ySensitivity = 2f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    //瞄准参数
    public bool isAiming = false;

    public bool isInspecting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        curAnimator = GetComponentInChildren<Animator>();
        playerShoot = GetComponent<PlayerShoot>();
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update() 
    {
        Mouse();
        Move();
        Jump();
        Fall();
        Aim();
        Holstor();
        Inspect();

       // animator = playerShoot.currentGun.GetComponent<Animator>();

        Debug.DrawRay(transform.position+Vector3.up*0.2f, Vector3.down * 0.25f, Color.red);

    }
    private void FixedUpdate()
    {
        
    }

    //鼠标旋转
    private void Mouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * xSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity;

        //上下旋转
        xRotation += mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);//限制最大旋转角度
        curAnimator.transform.localRotation = Quaternion.Euler(-xRotation, 0f, 0f);

        //左右旋转 
        yRotation += mouseX;
        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        rb.MoveRotation(targetRotation);
    }


    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal") ;
        float moveZ = Input.GetAxis("Vertical") ;

        isRunning = Input.GetKey(KeyCode.LeftShift);
        moveSpeed = isRunning ? 8f : 5f;
      
        Vector3 moveDir = transform.forward * moveZ + transform.right * moveX;
        Vector3 horizontalVelocity = moveDir * moveSpeed;
        rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);

        //动画控制
        bool isMoving = moveX != 0 || moveZ != 0;
        
        curAnimator.SetFloat("Movement", isMoving ? 1 : 0);
        curAnimator.SetBool("Running", isRunning && isMoving);

    }
    public void Jump()
    {
       if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {   
            Vector3 jumpDir = Vector3.up * jumpForce;
            rb.AddForce(jumpDir, ForceMode.Impulse);
            isJumping = true;
            //Debug.Log("Jumped");
        }

    }
    public void Fall()
    {
        if (isJumping && rb.velocity.y < 0)
        {
            rb.AddForce(Vector3.down * 15f, ForceMode.Acceleration);
        }
    }
    public bool IsGrounded()
    {
        RaycastHit hit;
        bool res = Physics.Raycast(transform.position+Vector3.up*0.2f, Vector3.down, out hit, 0.25f,LayerMask.GetMask("Ground"));
        return res;
    }

    public void Aim()
    {
        float aim = curAnimator.GetFloat("Aiming");
        if (Input.GetMouseButton(1)&&!isRunning)
        {
            isAiming = true;
            curAnimator.SetBool("Aim", true);
            curAnimator.SetFloat("Aiming", Mathf.Lerp(aim,1,0.1f));
        }
        else
        {
            isAiming = false;
            curAnimator.SetBool("Aim", false);
            curAnimator.SetFloat("Aiming", Mathf.Lerp(aim, 0, 0.1f));
        }
    }
    public void Holstor()
    {
        bool holstered = curAnimator.GetBool("Holstered");
        if (Input.GetKeyDown(KeyCode.Q))
        {
            curAnimator.SetBool("Holstered",holstered?false:true);
            Debug.Log("Holster toggled: " + !holstered);
        }
      

    }
    public void Inspect()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // 如果正在换弹，不能检视
            GunControl currentGun = GetComponent<PlayerShoot>()?.currentGun;
            if (currentGun != null && currentGun.isReloading)
            {
                Debug.Log("换弹中无法检视！");
                return;
            }

            curAnimator.CrossFade("Inspect", 0.05f, -1, 0f);
            isInspecting = true;
        }

    }
    public void SetHolster(bool holster)
    {
        curAnimator.SetBool("Holstered", holster);
    }

    // ========== 切换握持动画 ==========
    public void SwitchHoldingAnimation(RuntimeAnimatorController holdingController)
    {
        if (curAnimator == null || holdingController == null) return;

        // 替换 Animator 的 Controller
        curAnimator.runtimeAnimatorController = holdingController;
    }
    public bool CanFire()
    {
        bool isHolstered = curAnimator.GetBool("Holstered");
        if(isHolstered) return false;
        if(isRunning) return false;
        return true;
    }
    public void StopInspecting()
    {
        if (isInspecting)
        { 
           isInspecting = false;
            curAnimator.CrossFade("Idle", 0.1f);
        }
    }

}

