using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;
    float H, V;
    Vector3 Movement;
    float WalkSpeed = 2, RunSpeed = 5;
    float PushNormalSpeed =3.5f, PushQuickSpeed =5;
    public bool IsGround;
    public bool IsPushing;
    [SerializeField] private Vector3 Dir;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float RotSpeed;
    [SerializeField] private float JumpHeight;
    Vector3 PlayerHitNormal;
    Vector3 BoxHitNormal;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {

    }
    void Update()
    {
        if (!IsPushing)
        {
            H = Input.GetAxis("Horizontal");
            V = Input.GetAxis("Vertical");
            transform.forward = Vector3.Slerp(transform.forward, Dir, Time.deltaTime * RotSpeed);
            Dir.Set(H, 0, V);
            if (OnSlope(transform.position,true))
            {
                Dir = Vector3.ProjectOnPlane(Dir, PlayerHitNormal).normalized;
            }
        }
        Movement = Dir.normalized * MoveSpeed;
        if (Dir != Vector3.zero)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if(!IsPushing)
                {
                    anim.SetBool("Walk", true);
                    anim.SetBool("Run", true);
                }
                MoveSpeed = IsPushing ? PushQuickSpeed : RunSpeed;
            }
            else
            {
                if(!IsPushing)
                {
                    anim.SetBool("Run", false);
                    anim.SetBool("Walk", true);
                }
                MoveSpeed = IsPushing ? PushNormalSpeed : WalkSpeed;
            }
        }
        else
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", false);
        }
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Movement * Time.fixedDeltaTime);
        if (Input.GetKeyDown(KeyCode.Space) && IsGround)
        {
            anim.CrossFade("Jump", 0);
            rb.velocity = Vector3.up * JumpHeight;
        }
    }
    float PushTime = 0;
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PushTrigger"))
        {
            IsPushing = true;
            Vector3 Pos = other.transform.GetChild(0).position;
            Vector3 TargetPos = new Vector3(Pos.x, transform.position.y, Pos.z);
            transform.position =Vector3.MoveTowards(transform.position,TargetPos,Time.deltaTime*3f);
            if(PushTime>=0.3f)
            {
                if(Vector3.Distance(transform.position,TargetPos)<=0.1f)
                {
                    transform.position = TargetPos;
                    PushTime = 0;
                }
            }
            else
            {
                PushTime += Time.deltaTime;
            }
            string Direction = other.name;
            Rigidbody BoxRb = other.GetComponentInParent<Rigidbody>();
            Vector3 TargetDir = Vector3.zero;
            Vector3 TargetPushDir = Vector3.zero;
            switch (Direction)
            {
                case "Forward":
                    TargetDir = Vector3.forward;
                    if (anim.GetBool("Pushing"))
                        TargetPushDir=V * BoxRb.transform.forward;
                    break;
                case "Back":
                    TargetDir = Vector3.back;
                    if (anim.GetBool("Pushing"))
                        TargetPushDir=V * BoxRb.transform.forward;
                    break;
                case "Left":
                    TargetDir = Vector3.left;
                    if (anim.GetBool("Pushing"))
                        TargetPushDir = H * BoxRb.transform.right;
                    break;
                case "Right":
                    TargetDir = Vector3.right;
                    if (anim.GetBool("Pushing"))
                        TargetPushDir = H * BoxRb.transform.right;
                    break;
            }
            transform.forward = TargetDir;
            if (transform.forward == TargetDir)
            {
                if (!anim.GetBool("Push"))
                {
                    anim.SetBool("Push", true);
                }

                if (H == 0 && V == 0)
                {
                    anim.SetBool("Pushing", false);
                }
                if ((Direction == "Forward" || Direction == "Back"))
                {
                    V = Input.GetAxis("Vertical");
                    H = 0;
                    Dir.Set(0, 0, V);
                    if (V != 0)
                    {
                        anim.SetBool("Pushing", true);
                    }
                }
                else if ((Direction == "Left" || Direction == "Right"))
                {
                    H = Input.GetAxis("Horizontal");
                    V = 0;
                    Dir.Set(H, 0, 0);
                    if (H != 0)
                    {
                        anim.SetBool("Pushing", true);
                    }
                }
            }
            if(OnSlope(BoxRb.position,false))
            {
                TargetPushDir = Vector3.ProjectOnPlane(TargetPushDir, PlayerHitNormal).normalized;
            }
                BoxRb.AddForce(TargetPushDir*MoveSpeed, ForceMode.Force);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PushTrigger"))
        {
            PushTime = 0;
            IsPushing = false;
            anim.SetBool("Push", false);
            anim.SetBool("Pushing", false);
        }
    }

    private bool OnSlope(Vector3 Pos,bool IsPlayer)
    {
        RaycastHit hit;
        bool IsSlope = false;
        if (Physics.Raycast(Pos, Vector3.down, out hit, 1f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if(IsPlayer)
            {
                PlayerHitNormal = hit.normal;
            }
            else
            {
                BoxHitNormal = hit.normal;
            }
            IsSlope = angle >= 5 ? true : false;
        }
        return IsSlope;
    }
}
