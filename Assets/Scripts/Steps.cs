using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steps : MonoBehaviour
{
    AudioSource step;
    void Start()
    {
        step = GetComponent<AudioSource>();
    }
    private float thresholdSpeed = 0.1f; // Adjust this value as desired

    // OnCollisionStay is called once per frame for every collider that is touching another collider.
    void OnCollisionStay(Collision collisionInfo)
    {
        // Check if the speed of the object exceeds the threshold
        if (GetComponent<Rigidbody>().velocity.magnitude > thresholdSpeed)
        {
            // If the AudioSource isn't playing, start it
            if (!step.isPlaying)
            {
                step.Play(); // Probably best to enable looping of the audio clip in the AudioSource settings or via audio.loop
            }
        }
        else
        { // Object is touching something, but isn't moving fast enough
          // If the AudioSource is playing, stop it
            if (step.isPlaying)
            {
                step.Pause(); // Pausing is probably better than stopping for looping sounds since it avoids always playing the loop from the start
            }
        }
    }
}
