using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction {Forward,Back,Left,Right}
public class PlayerMove : MonoBehaviour {
    const int WalkSpeed=2, RunSpeed=4;
    const string Anim_Move = "Move", Anim_Push = "Push",
        Anim_IsPush="IsPush";

    public static Transform m_trans;
    Rigidbody rb;
    Animator anim;

    float H, V;
    Vector3 Dir=Vector3.zero;
    float MoveSpeed;
    public float RotSpeed;

    bool IsShiftPress;
    public bool IsGround;

    public LayerMask BoxLayer;
    public float BoxCheckDis;

    public Transform GroundCheckPoint;
    public LayerMask GroundCheckLayer;
    public float GroundCheckRadius;

    public bool IsLockH, IsLockV;

    public BoxScript CurrentBox;
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
        BoxCheck();
        SlopeCheck();
        GroundChcek();
    }
    private void FixedUpdate()
    {
        MoveFunc();
    }

    public void GetValue()
    {
        if(IsGround)
        {
            if (!IsLockH)
            {
                H = Input.GetAxis("Horizontal");
            }
            else
            {
                H = 0;
            }
            if (!IsLockV)
            {
                V = Input.GetAxis("Vertical");
            }
            else
            {
                V = 0;
            }
        }
        else
        {
            H = 0;
            V = 0;
        }
        Dir.Set(H, 0, V);
        if(IsOnSlope())
        {
            Dir = Vector3.ProjectOnPlane(Dir, HitNormal);
        }
    }//获取轴向值
    public void KeyCheck()
    {
        if (Input.GetKey(KeyCode.LeftShift) ||Input.GetKey(KeyCode.RightShift))
        {
            IsShiftPress = true;
            MoveSpeed = RunSpeed;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
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
        if(Dir!=Vector3.zero)
        transform.forward = Vector3.Slerp(transform.forward, Dir, Time.deltaTime * RotSpeed);
    }//角色旋转

    public void PlayerAnim()
    {
        if(H!=0||V!=0)
        {
            if(IsShiftPress)
            {
                AnimSetFloat(Anim_Move, 1f);
            }
            else
            {
                AnimSetFloat(Anim_Move, 0.5f);
            }
        }
        else if(H==0&&V==0)
        {
            AnimSetFloat(Anim_Move, 0f);
        }
    }//角色动画

    public void BoxCheck()//箱体检测
    {
        RaycastHit hit;
        if(Physics.Raycast(m_trans.position+new Vector3(0,0.3f,0),m_trans.forward,out hit,BoxCheckDis,BoxLayer))
        {
            if (H != 0 && V != 0) return;
            CurrentBox = hit.collider.GetComponent<BoxScript>();
            if (H != 0)
            {
                if (H > 0)
                {
                    direction = Direction.Right;
                }
                else
                {
                    direction = Direction.Left;
                }
                if(!IsLockV)
                {
                    IsLockV = true;
                    switch (direction)
                    {
                        case Direction.Left:
                            transform.forward = Vector3.left;
                            break;
                        case Direction.Right:
                            transform.forward = Vector3.right;
                            break;
                    }
                }
                Vector3 TargetPos = new Vector3(m_trans.position.x, m_trans.position.y, CurrentBox.transform.position.z);
                m_trans.position = TargetPos;
                //m_trans.position = Vector3.MoveTowards(m_trans.position, TargetPos, Time.deltaTime);
            }

            if (V != 0)
            {
                if (V > 0)
                {
                    direction = Direction.Forward;
                }
                else
                {
                    direction = Direction.Back;
                }
                if(!IsLockH)
                {
                    IsLockH = true;
                    switch (direction)
                    {
                        case Direction.Forward:
                            transform.forward = Vector3.forward;
                            break;
                        case Direction.Back:
                            transform.forward = Vector3.back;
                            break;
                    }
                }
                Vector3 TargetPos = new Vector3(CurrentBox.transform.position.x, m_trans.position.y, m_trans.position.z);
                m_trans.position = TargetPos;
            }
            AnimSetBool(Anim_IsPush, true);
            if(H!=0||V!=0)
            {
                AnimSetFloat(Anim_Push, 1f);
            }
            if(H==0&&V==0)
            {
                AnimSetFloat(Anim_Push, 0);
            }
            PushCheck();
        }
        else
        {
            if (IsLockV)
                IsLockV = false;
            if (IsLockH)
                IsLockH = false;

            if(anim.GetFloat(Anim_Push)!=0)
            {
                AnimSetFloat(Anim_Push, 0);
            }
            AnimSetBool(Anim_IsPush, false);
            if(CurrentBox!=null)
            {
                CurrentBox = null;
            }
        }
    }

    public Direction direction;
    Vector3 Direc;
    public void PushCheck()
    {
        Direc = Vector3.zero;
        switch (direction)
        {
            case Direction.Forward:
                Direc = Vector3.forward;
                break;
            case Direction.Back:
                Direc = Vector3.back;
                break;
            case Direction.Left:
                Direc = Vector3.left;
                break;
            case Direction.Right:
                Direc = Vector3.right;
                break;
        }
        //transform.forward = Direc;
        CurrentBox.PushFunc(Direc, MoveSpeed);
    }

    Collider[] GroundCols;
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
    }

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
    }

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
    }


    public void AnimSetFloat(string FloatName, float Value)
    {
        anim.SetFloat(FloatName, Value);
    }
    public void AnimSetBool(string BoolName, bool Value)
    {
        anim.SetBool(BoolName, Value);
    }

}
