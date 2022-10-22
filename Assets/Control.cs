using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void FixedUpdate()
    {
        rb.AddForceAtPosition(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))*0.2f,transform.GetChild(0).transform.position,ForceMode.VelocityChange);
    }
}
