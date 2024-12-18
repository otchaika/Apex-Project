using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SmallStone : MonoBehaviour
{
    public Transform snapPoint;
    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
    public void SnapToPoint()
    {
        this.transform.position = snapPoint.position;
    }
    
}
