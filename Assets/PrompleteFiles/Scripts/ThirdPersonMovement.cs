using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    //找到角色控制器
    public CharacterController controller;
    //攝影機
    public Transform cam;
    //移動速度
    public float speed = 6f;
    //玩家轉向需要的時間
    public float turnSmoothTime = 0.1f;
    //讓角色平滑阻尼角度的速度進行更改所使用的參數
    float turnSmoothVelocity;
    //判別是否可以移動
    bool MoveCount = true;
    //改變animator裡跑步使用的值
    public float animatorNum = 0f;
    //animator元件，從子物件尋找的
    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //如果MoveCount是true，允許移動
        if(MoveCount == true)
        {
            //水平垂直值
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            //方向訊息
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            
            
            if (direction.magnitude >= 0.1f)  //direction向量長度
            {
                //指定一float數為我們想指向的方向的角度值
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

                //將角度值賦予平滑阻尼角度
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                //增加Animator跑步參數，當數字>=1的時候停止
                if(animatorNum < 1f)
                {
                    animatorNum += 0.05f;
                }               
                animator.SetFloat("speedPercent", animatorNum);

                //將數字套用至角色旋轉上
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                //將移動所依靠的值使用所想要指定的角度，再乘上往Z軸與-Y軸的方向。
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * new Vector3(0,-1,1);
        
                //移動角色
                controller.Move(moveDir * speed * Time.deltaTime);
            }

            //角色停止的時候也希望貼齊地面
            if (direction.magnitude < 0.1f)
            {
                //當角色不移動時也要往-Y軸方向移動，避免浮空
                Vector3 moveDir = Quaternion.Euler(0f, 0f, 0f) * new Vector3(0,-1,0);
                controller.Move(moveDir * speed * Time.deltaTime);

                //減少Animator跑步參數，當數字<=0的時候停止
                if (animatorNum > 0f)
                {
                    animatorNum -= 0.05f;
                }                    
                animator.SetFloat("speedPercent", animatorNum);
            }

            //按下指定按鈕時播放動畫，且限制不可移動
            if (Input.GetButtonDown("Fire1"))
            {
                animator.SetTrigger("attacking");
                MoveCount = false;
            }
        }       
    }
    //接收到此情節時，允許開始移動
    public void MoveCountStart()
    {
        MoveCount = true;
    }
}
