using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamearFollow : MonoBehaviour {
    Transform m_trans;
    public Transform Target;
    public float FollowSpeed;
    Vector3 Offset;
    private void Awake()
    {
        m_trans = transform;
        Offset = m_trans.position- Target.position;
    }
    private void Update()
    {
        //一个基础的相机跟随角色
        m_trans.position = Vector3.MoveTowards(m_trans.position, Target.position + Offset, Time.deltaTime * FollowSpeed);
    }
}
