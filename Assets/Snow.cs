using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow : MonoBehaviour
{
    // Start is called before the first frame update

    public RenderTexture rt;

    public Texture drawImg;

    void Start()
    {

        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0,0,rt.width,rt.height);
        Rect rect = new Rect(0, 0, drawImg.width, drawImg.height);

        Graphics.DrawTexture(rect,drawImg);
        GL.PopMatrix();

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
