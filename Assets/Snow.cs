using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEditor;
using UnityEngine;
using VSCodeEditor;

public class Snow : MonoBehaviour
{

    public RenderTexture rt;

    public Texture drawImg;

    public Camera mainCam;

    public Rigidbody rb;  //圆球

    public float speed = 0.4f;
    
    void Start()
    {
        mainCam = Camera.main.GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
        GetComponent<Renderer>().material.mainTexture = rt;
    }
    
    
    public void Draw(int x,int y){
        
        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0,rt.width,rt.height,0);
        Rect rect = new Rect(0, 0, drawImg.width, drawImg.height);
        
        Graphics.DrawTexture(rect,drawImg);
        GL.PopMatrix();
        RenderTexture.active = null;
    }
 

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Debug.Log("按下");
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("点击到"+hit.transform.name);
                int x = (int)(hit.textureCoord.x * rt.width);
                int y  = (int)(hit.textureCoord.y * rt.height);
                Draw(x,y);
            }

        }


       

        //var transformPosition = rb.transform.position;
        // transformPosition.x = Input.GetAxis("Horizontal");

    }
}
