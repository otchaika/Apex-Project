using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    public Color curColor = Color.green;

    public void SwapColor(Color color)
    {
        curColor = color;
    }
}
