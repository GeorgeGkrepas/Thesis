using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandling : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlaySound()
    {
        audioSource.Play();
    }

}