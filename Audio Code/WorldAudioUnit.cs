using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//We don't need too much functionality here,
//WorldAudio does the hard work.
[RequireComponent(typeof(AudioSource))]
    public class WorldAudioUnit : MonoBehaviour
    {
    AudioSource aud;
    void Start()
    {
        aud = GetComponent<AudioSource>();

    }

    public void Call(Vector3 pos, AudioClip ply)
    {
        transform.position = pos;
        if(aud)
        aud.PlayOneShot(ply);
    }
    }

