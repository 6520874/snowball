using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour {
    Rigidbody rb;
    Transform m_trans;
    private void Awake()
    {
        m_trans = transform;
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        SlopeCheck();
    }
    public void PushFunc(Vector3 dir,float Force)
    {
        m_trans.Translate(dir*Time.deltaTime,Space.World);
        if(IsOnSlope())
        {
            m_trans.up =Vector3.Lerp(m_trans.up,HitNormal,Time.deltaTime);
        }
        else
        {
            m_trans.forward = dir;
        }
    }
    public float SlopeCheckDis;
    public LayerMask SlopeLayer;
    Vector3 HitNormal;
    public void SlopeCheck()
    {
        RaycastHit hit;
        if(Physics.Raycast(m_trans.position+Vector3.up,Vector3.down,out hit,SlopeCheckDis, SlopeLayer))
        {
            HitNormal = hit.normal;
        }
    }

    bool IsOnSlope()
    {
        float Angle = Vector3.Angle(HitNormal, Vector3.up);
        if (Angle >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
