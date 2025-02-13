using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPosition : MonoBehaviour
{
    public Transform startPosition;
    public Transform xrOrigin;
    // Start is called before the first frame update
    void Awake()
    {
       xrOrigin.position = startPosition.position;
       xrOrigin.rotation = startPosition.rotation;
    }
}
