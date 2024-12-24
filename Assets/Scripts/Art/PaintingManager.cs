using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    public Color curColorLeft = Color.clear;
    public Color curColorRight = Color.clear;
    //Brush color
    public Color curColorBrush  = Color.clear;

    public Material leftFinger;
    public Material rightFinger;
    //Brush material
    public Material brush;

    public void SwapColor(Color color, string hand)
    {
        Debug.Log("manager swapped color");
        if (hand=="left")
        {
            curColorLeft = color;
            leftFinger.color = color;
        }
        if (hand == "right") { 

            curColorRight = BlendAdditive(curColorRight, color, 0.5f);
            rightFinger.color = curColorRight;
        }
        //Checking the brush
        if (hand == "brush")
        {
            curColorBrush = BlendAdditive(curColorBrush, color, 0.5f);
            brush.color = curColorBrush;
        }
    }

    Color BlendAdditive(Color color1, Color color2, float mixAmount)
    {
        float inverseMix = 1.0f - mixAmount;

        return new Color(
            Mathf.Clamp01(color1.r * inverseMix + color2.r * mixAmount),
            Mathf.Clamp01(color1.g * inverseMix + color2.g * mixAmount),
            Mathf.Clamp01(color1.b * inverseMix + color2.b * mixAmount),
            Mathf.Clamp01(color1.a * inverseMix + color2.a * mixAmount)
        );
    }


}
