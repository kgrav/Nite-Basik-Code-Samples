using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Class that times dynamic BGM systems.
//Additionally, this holds the null clip,
//that is played by registers which are turned 'off',
//but still need to be updated according to the same pattern.
[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Nite-Basik/Sound/Midi Timer")]
    public class MidiTimer : MonoBehaviour
    {

    public static AudioClip GetNullClip()
    {
        if (!NullClip)
        {
            NullClip = GameObject.FindObjectsOfType<MidiTimer>()[0].TimeTrack;
        }
        return NullClip;
    }

    public static AudioClip NullClip;
    public static float MidiClipLength = 0.0f;
    public static float MeasLength;
    public static float MidiTime = 999.0f;
    public static float MidiUpdateTime = 0.0f;
    public AudioClip TimeTrack;
    AudioSource player;
    void Start()
    {
        NullClip = TimeTrack;
        player = GetComponent<AudioSource>();
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().clip = TimeTrack;
        MidiClipLength = TimeTrack.length;
        MidiTime = 0;
    }

    void Update()
    {
        MidiTime = player.time;
    }

    public void ChangeClip(AudioClip newtime)
    {
        TimeTrack = newtime;
        MidiClipLength = TimeTrack.length;
        MidiTime = player.time;
        player.clip = newtime;
        player.time = MidiTime;
    }

    }

