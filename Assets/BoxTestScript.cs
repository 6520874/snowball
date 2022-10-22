using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTestScript : MonoBehaviour {
    Transform m_trans;
    private void Awake()
    {
        m_trans = transform;
    }
    private void Update()
    {
        SlopeCheck();//忘记执行了
    }
    public void PushFunc(Vector3 Dir,float Speed)//推动接口,传入方向和速度
    {
        //使用translate的方法进行推动
        m_trans.Translate(Dir * Time.deltaTime * Speed,Space.World);//以世界空间移动

        ////实现箱子与地形方向贴合
        if (IsInSlope())
        {
            m_trans.up = Vector3.Lerp(m_trans.up, HitNormal, Time.deltaTime*5);//插值旋转
        }
        else
        {
            m_trans.forward = Dir;//非斜坡时
        }
    }

    //斜坡检测
    public float SlopeDis;
    public LayerMask SlopeLayer;
    public Vector3 HitNormal;
    public void SlopeCheck()
    {
        RaycastHit hit;
        if(Physics.Raycast(m_trans.position+Vector3.up*0.5f,Vector3.down,out hit,SlopeDis,SlopeLayer))
        {
            //使用V3变量储存HitNormal，用于计算于斜坡垂直的方向
            HitNormal = hit.normal;
        }
    }
    public bool IsInSlope()
    {
        if(Vector3.Angle(HitNormal,Vector3.up)>1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
