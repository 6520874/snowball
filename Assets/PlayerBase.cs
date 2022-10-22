using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//使用一个枚举记录当前的推动方向，使用H和V值进行判断当前方向
public enum PushDirection
{
    Forward,
    Back,
    Left,
    Right
}
public class PlayerBase : MonoBehaviour {
    const int WalkSpeed = 2, RunSpeed = 4;
    const string Anim_Move = "Move", Anim_Push = "Push",
        Anim_IsPush = "IsPush";

    public static Transform m_trans;
    Rigidbody rb;
    Animator anim;

    float H, V;
    Vector3 Dir = Vector3.zero;
    float MoveSpeed;
    public float RotSpeed;

    bool IsShiftPress;
    public bool IsGround;
    private void Awake()
    {
        m_trans = transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        MoveSpeed = WalkSpeed;
    }
    private void Update()
    {
        GetValue();
        KeyCheck();
        RotFunc();
        PlayerAnim();
        GroundChcek();
        SlopeCheck();//斜坡检测
        BoxCheck();//箱体检测
    }
    //箱体检测使用射线检测的方法
    public float BoxCheckDis;
    public LayerMask BoxCheckLayer;
    [SerializeField]private BoxTestScript currentBox;//使用一个变量储存当前推动的箱子
    public bool IsLockH, IsLockV;//使用Bool值记录是否锁定相应的轴向
    public PushDirection CurrentPushDir;
    private void BoxCheck()
    {
        RaycastHit hit;
        //m_trans= transform;
        if(Physics.Raycast(m_trans.position+Vector3.up*0.3f,m_trans.forward,out hit,BoxCheckDis,BoxCheckLayer))
        {
            if(H!=0&&V!=0)
            {
                return;//不想要斜向运动的时候也进入推箱子状态，这里只想实现前后和左右推动
            }
            //判断方向
            if(anim.GetBool(Anim_IsPush)==false)
            {
                AnimSetBool(Anim_IsPush, true);//进入推箱子动画
                                               //根据获取Horizontal和Vertical的值判断是否进行推动
                if(H!=0)
                {
                    IsLockV = true;
                    if(H>0)
                    {
                        CurrentPushDir = PushDirection.Right;
                    }
                    else
                    {
                        CurrentPushDir = PushDirection.Left;
                    }

                    //角色位置修正
                    Vector3 TargetPos = new Vector3(m_trans.position.x ,m_trans.position.y, hit.collider.transform.position.z);
                    m_trans.position = TargetPos;
                }
                if(V!=0)
                {
                    IsLockH = true;
                    if (V> 0)
                    {
                        CurrentPushDir = PushDirection.Forward;
                    }
                    else
                    {
                        CurrentPushDir = PushDirection.Back;
                    }

                    //角色位置和方向修正
                    Vector3 TargetPos = new Vector3(hit.collider.transform.position.x, m_trans.position.y, m_trans.position.z);
                    m_trans.position = TargetPos;
                }
                AnimSetFloat(Anim_Push, 1f);
            }
            //并且需要判断推动的方向以及锁定对应的轴向值，如果左右推则锁定Vertical，前后推则锁定住Horizontal
            if (currentBox==null)
            {
                currentBox = hit.collider.GetComponent<BoxTestScript>();
            }
            //获取纯正的方向值
            Vector3 Dir = Vector3.zero;
            switch (CurrentPushDir)
            {
                case PushDirection.Forward:
                    Dir = Vector3.forward;
                    break;
                case PushDirection.Back:
                    Dir = Vector3.back;
                    break;
                case PushDirection.Left:
                    Dir = Vector3.left;
                    break;
                case PushDirection.Right:
                    Dir = Vector3.right;
                    break;
            }
            m_trans.forward = Dir;//Forward
            currentBox.PushFunc(Dir, MoveSpeed);
        }
        else
        {
            IsLockV = false;
            IsLockH = false;
            AnimSetBool(Anim_IsPush, false);
            AnimSetFloat(Anim_Push, 0f);
            if (currentBox != null)
            {
                currentBox = null;
            }
        }
    }

    private void FixedUpdate()
    {
        MoveFunc();
    }

    public void GetValue()
    {
        if (IsGround)
        {
            //根据Bool锁定相应轴向
            if(IsLockH)
            {
                H = 0;
            }
            else
            {
                H = Input.GetAxis("Horizontal");
            }
            if(IsLockV)
            {
                V = 0;
            }
            else
            {
                V = Input.GetAxis("Vertical");
            }
        }
        else
        {
            H = 0;
            V = 0;
        }
        Dir.Set(H, 0, V);
        if (IsOnSlope())
        {
            Dir = Vector3.ProjectOnPlane(Dir, HitNormal);
        }
    }//获取轴向值
    public void KeyCheck()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            IsShiftPress = true;
            MoveSpeed = RunSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            IsShiftPress = false;
            MoveSpeed = WalkSpeed;
        }
    }//按键检测

    public void MoveFunc()
    {
        rb.MovePosition(rb.position + Dir.normalized * Time.fixedDeltaTime * MoveSpeed);
    }//移动方法
    public void RotFunc()
    {
        if (Dir != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, Dir, Time.deltaTime * RotSpeed);
    }//角色旋转

    public void PlayerAnim()
    {
        if (H != 0 || V != 0)
        {
            if (IsShiftPress)
            {
                AnimSetFloat(Anim_Move, 1f);
            }
            else
            {
                AnimSetFloat(Anim_Move, 0.5f);
            }
        }
        else if (H == 0 && V == 0)
        {
            AnimSetFloat(Anim_Move, 0f);
        }
    }//角色动画

    Collider[] GroundCols;
    public Transform GroundCheckPoint;
    public LayerMask GroundCheckLayer;
    public float GroundCheckRadius;
    public void GroundChcek()
    {
        GroundCols = Physics.OverlapSphere(GroundCheckPoint.position, GroundCheckRadius, GroundCheckLayer);
        if (GroundCols.Length > 0)
        {
            if (!IsGround)
            {
                IsGround = true;
            }
        }
        else
        {
            IsGround = false;
        }
    }//地面监测

    public float SlopeCheckDis;
    public LayerMask SlopeLayer;
    Vector3 HitNormal;
    public void SlopeCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_trans.position, Vector3.down, out hit, SlopeCheckDis, SlopeLayer))
        {
            HitNormal = hit.normal;
        }
    }//斜坡检测

    bool IsOnSlope()
    {
        float Angle = Vector3.Angle(HitNormal, Vector3.up);
        if (Angle >= 10)
        {
            return true;
        }
        else
        {
            return false;
        }
    }//斜坡

    public void AnimSetFloat(string FloatName, float Value)
    {
        anim.SetFloat(FloatName, Value);
    }
    public void AnimSetBool(string BoolName, bool Value)
    {
        anim.SetBool(BoolName, Value);
    }
}
