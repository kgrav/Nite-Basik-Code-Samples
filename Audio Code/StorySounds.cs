using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[AddComponentMenu("Lady Stardust/Sound/Story Soundboard")]
//If a sound needs to be played by a script that does NOT derive from MonoBehaviour,
//Or if a sound needs to be played by an object that will be destroyed shortly after making it,
//Or if a sound needs to be played by some part of the UI,
//It should be played through this class's "Sound" method.

//Attach this script to the main camera, or a child object attached to the main character
//Attach any number of AudioSources to child objects of this object, and the method
//will cycle through its children (so that if two or more signals are received in short succession,
//they will not cut each other off).
//this means fewer audio source components in scene.
//Sound effects can either be read in as strings from a manifest document
//and loaded when needed (good for music, which tends to be large), or loaded directly from
//components (good for fx, which have a comparatively small footprint)
//For further optimization, you can load sourceless sounds in from a Manifest file and retrieve them from
    // the Sound Table, given that SoundTable.GetSound returns an AudioClip, and StorySounds.Sound accepts an AudioClip
//as the argument.
    //Not sure how much performance that would actually save though, as most classes that call this are
    //singletons with pre-loaded clips anyway.

//Attach at least two Audiosources for best results, although it will work with only one,
//as long as it exists inside of a child object.
public enum SOUND_FLAGS {DOOR }

[AddComponentMenu("Nite-Basik/Sound/GUI Sound Structure")]
    public class StorySounds : MonoBehaviour
    {
    static AudioSource[] stsrc;
    static int ptr = 0, length;
    static AudioClip loadingdoorclip;
    static AudioClip nullclip;
    static bool init = false;
    public static void Sound(AudioClip s)
    {
        if (init)
        {
            //play sound
            stsrc[ptr].PlayOneShot(s);
            //move to next audiosource for next call
            ptr++;
            if (ptr >= length)
                ptr = 0;
        }
    }
    public static void Sound(SOUND_FLAGS s)
    {
        AudioClip r = nullclip;
        switch (s)
        {
            case SOUND_FLAGS.DOOR:
                r = loadingdoorclip;
                break;
        }
        Sound(r);


    }
    public AudioClip LoadingDoor;
    void Start()
    {
        AudioSource[] srcs = GetComponentsInChildren<AudioSource>();
        ptr = 0;
        length = srcs.Length;

        loadingdoorclip = LoadingDoor;
        stsrc = new AudioSource[length];
        for (int i = 0; i < length; ++i)
        {
            stsrc[i] = srcs[i];
        }
        init = true;
    }
    }

