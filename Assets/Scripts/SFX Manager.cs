using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip Herb_grinding;
    [SerializeField] private AudioClip Coal_grinding;
    [SerializeField] private AudioClip Clay_grinding;
    [SerializeField] private AudioClip Pigments;
    [SerializeField] private AudioClip Picking;
    [SerializeField] private AudioClip Paitning;
    [SerializeField] private AudioClip PaintSwapper;
    [SerializeField] private AudioClip BrushAttaching;
    [SerializeField] private AudioClip FullyAssembeled;
    [SerializeField] private AudioClip Dropped;
    [SerializeField] private AudioClip Water;
    AudioSource sfx;
    // Start is called before the first frame update
    void Start()
    {
        sfx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Herb"))
        {
            sfx.clip = Herb_grinding;
            sfx.PlayOneShot(sfx.clip);
        }
        else if (collision.gameObject.CompareTag("Coal"))
        {
            sfx.clip = Coal_grinding;
            sfx.PlayOneShot(sfx.clip);
        }
        else if (collision.gameObject.CompareTag("Clay"))
        {
            sfx.clip = Clay_grinding;
            sfx.PlayOneShot(sfx.clip);
        }
        else if (collision.gameObject.CompareTag("Water"))
        {
            sfx.clip = Water;
            sfx.PlayOneShot(sfx.clip);
        }
        else if (collision.gameObject.CompareTag("Environement"))
        {
            if (collision.relativeVelocity.magnitude > 2)
            {
                sfx.clip = Dropped;
                sfx.PlayOneShot(sfx.clip);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pigments"))
        {
            sfx.clip = Pigments;
            sfx.PlayOneShot(sfx.clip);
        }
    }
}
