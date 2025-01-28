using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropsound : MonoBehaviour
{
    public AudioSource dropsound;

    void Start()
    {
        dropsound = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2)
            dropsound.Play();
    }
}
