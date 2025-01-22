using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    public TextMeshProUGUI displayText; // Reference to the TextMeshProUGUI component
    private int avgFrameRate;

    private void Update()
    {
        // Calculate frames per second
        avgFrameRate = (int)(1f / Time.unscaledDeltaTime);
        displayText.text = avgFrameRate + " FPS"; // Update the UI text
    }
}