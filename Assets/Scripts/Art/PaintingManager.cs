using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    public Color curColorLeft = Color.green;
    public Color curColorRight = Color.green;
    public Material leftFinger;
    public Material rightFinger;
    public void SwapColor(Color color, string hand)
    {
        Debug.Log("manager swapped color");
        if (hand=="left")
        {
            curColorLeft = color;
            leftFinger.color = color;
        }
        if (hand == "right") { 
            rightFinger.color = color;
            curColorRight = color;
        }

            
    }

    
}
