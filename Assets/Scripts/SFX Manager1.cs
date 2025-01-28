using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SFXManager1 : MonoBehaviour
{
    [Header("VFX Sounds")]
    [SerializeField] private AudioClip Herb_grinding;
    [SerializeField] private float Volume1;
    [SerializeField] private AudioClip Coal_grinding;
    [SerializeField] private float Volume2;
    [SerializeField] private AudioClip Clay_grinding;
    [SerializeField] private float Volume3;
    [SerializeField] private AudioClip Pigments;
    [SerializeField] private float Volume4;
    [SerializeField] private AudioClip Picking;
    [SerializeField] private float Volume5;
    [SerializeField] private AudioClip Paitning;
    [SerializeField] private float Volume6;
    [SerializeField] private AudioClip PaintSwapper;
    [SerializeField] private float Volume7;
    [SerializeField] private AudioClip BrushAttaching;
    [SerializeField] private float Volume8;
    [SerializeField] private AudioClip FullyAssembeled;
    [SerializeField] private float Volume9;
    [SerializeField] private AudioClip Dropped;
    [SerializeField] private float Volume10;
    [SerializeField] private AudioClip Water;
    [SerializeField] private float Volume11;
    // Start is called before the first frame update
    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Herb"))
        {
            AudioSource.PlayClipAtPoint(Herb_grinding, transform.position, Volume1);
            Destroy(Herb_grinding);
        }
        else if (collision.gameObject.CompareTag("Coal"))
        {
            AudioSource.PlayClipAtPoint(Coal_grinding, transform.position, Volume2);
            Destroy(Coal_grinding);
        }
        else if (collision.gameObject.CompareTag("Clay"))
        {
            AudioSource.PlayClipAtPoint(Clay_grinding, transform.position, Volume3);
            Destroy(Clay_grinding);
        }
        else if (collision.gameObject.CompareTag("Water"))
        {
            AudioSource.PlayClipAtPoint(Water, transform.position, Volume11);
            Destroy(Water);
        }
        else if (collision.gameObject.CompareTag("Environement"))
        {
            if (collision.relativeVelocity.magnitude > 2)
            {
                AudioSource.PlayClipAtPoint(Dropped, transform.position, Volume10);
                Destroy(Dropped);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pigments"))
        {
            AudioSource.PlayClipAtPoint(Pigments, transform.position, Volume4);
            Destroy(Pigments);
        }
    }
}
