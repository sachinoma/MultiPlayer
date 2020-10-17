using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plMove3D : Photon.MonoBehaviour
{
    public bool devTesting = false;

    public PhotonView photonView;

    public GameObject plCam;

    public TextAlignment plNameText;

    public float moveSpeed = 100f;
    public float jumpForce = 800f;

    public Animator animator;

    private Vector3 selfPos;

    private GameObject sceneCam;


    //找到角色控制器
    public CharacterController controller;
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



    private void Awake()
    {
        if(!devTesting && photonView.isMine)
        {
            sceneCam = GameObject.Find("Main Camera");
            sceneCam.SetActive(false);
            plCam.SetActive(true);
        }
    }

    private void Update()
    {
        //if(!devTesting)
        //{
        //    if (photonView.isMine)
        //    {
        //        checkInput();
        //    }
        //    else
        //    {
        //        smoothNetMovemeny();
        //    }
        //}

        //else
        //{
        //    checkInput();
        //}


        if (photonView.isMine)
        {

            //水平垂直值
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            //方向訊息
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;


            if (direction.magnitude >= 0.1f)  //direction向量長度
            {
                //指定一float數為我們想指向的方向的角度值
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + plCam.transform.eulerAngles.y;

                //將角度值賦予平滑阻尼角度
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                //增加Animator跑步參數，當數字>=1的時候停止
                if (animatorNum < 1f)
                {
                    animatorNum += 0.05f;
                }
                animator.SetFloat("Run", animatorNum);

                //將數字套用至角色旋轉上
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                //將移動所依靠的值使用所想要指定的角度，再乘上往Z軸與-Y軸的方向。
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * new Vector3(0, -1, 1);

                //移動角色
                controller.Move(moveDir * speed * Time.deltaTime);
            }

            //角色停止的時候也希望貼齊地面
            if (direction.magnitude < 0.1f)
            {
                //當角色不移動時也要往-Y軸方向移動，避免浮空
                Vector3 moveDir = Quaternion.Euler(0f, 0f, 0f) * new Vector3(0, -1, 0);
                controller.Move(moveDir * speed * Time.deltaTime);

                //減少Animator跑步參數，當數字<=0的時候停止
                if (animatorNum > 0f)
                {
                    animatorNum -= 0.05f;
                }
                animator.SetFloat("Run", animatorNum);
            }

        }

        else
            {
                smoothNetMovemeny();
            }

    }

    private void checkInput()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));
        transform.position += move * moveSpeed * Time.deltaTime;
        if(Input.GetAxis("Vertical") >= 0.5f)
        {
            animator.SetFloat("Run", 1f);
        }
        if(Input.GetAxis("Vertical") == 0)
        {
            animator.SetFloat("Run", 0f);
        }
    }


    private void smoothNetMovemeny()
    {
        transform.position = Vector3.Lerp(transform.position, selfPos, Time.deltaTime * 8);
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            selfPos = (Vector3)stream.ReceiveNext();
        }
    }



}
