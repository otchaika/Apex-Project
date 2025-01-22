using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTeleport : MonoBehaviour
{
    public GameObject teleport;
    private void OnTriggerEnter(Collider other)
    {            if (other.tag == "Player")
        {
            Debug.Log(other.tag);
        }
        if (other.tag == "MainCamera")
        {
            teleport.SetActive(true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            teleport.SetActive(false);
        }
    }
}
