using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Loads individual audio tracks in a dynamic BGM system.
//Loads the next track it gets immediately upon receipt, and seeks to
//previous track's position.
//It is imperative that audio tracks are standardized before lo0ading.
//Making a bunch of tracks that are the same length and tempo is pretty easy
//if you're using anything digital to make your music.
//To prevent obvious audio skipping,
//these are coordinated in pairs by the audio system.
//(i.e. one loader switches to the next track while muted, the other plays the current track)
[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Nite-Basik/Sound/Midi Loader")]
public class MidiHOTLoader : MonoBehaviour
{
    public int index;
    AudioSource player;
    AudioClip play;
    int current;
    int next;
    float timetot;
    float timecurr;
    bool swap;
    public int measures;
    bool switchonnext = false;
    bool switchoffnext = false;

    int state;
    void Start()
    {
        current = -1;
        next = -1;
        timetot = 0.0f;
        timecurr = 0.0f;
        swap = false;
    }

    void Update()
    {
        player = GetComponent<AudioSource>();
    }

    void OnEdge()
    {
            if (switchonnext)
            {

                GetComponent<AudioSource>().Play();
                float f = Time.deltaTime - 0.0002f;
                GetComponent<AudioSource>().time = MidiTimer.MidiClipLength + (Time.deltaTime - 0.0002f);
                switchonnext = false;
            }
            Invoke("OnEdge", MidiTimer.MidiClipLength);
    }
    bool init = false;
    public void Init(AudioClip loada)
    {
        play = loada;
        player = GetComponent<AudioSource>();
        player.clip = play;
    }
    
    public void Switchto(AudioClip loada)
    {
        
        player = GetComponent<AudioSource>();
        player.clip = loada;
        switchonnext = true;
    }

    public void HotFlex(AudioClip load)
    {
        AudioSource ass = GetComponent<AudioSource>();
        float tt = Time.time;
        float g = ass.time;
        ass.clip = load;
        ass.Play();
        ass.time = g;
    }

    public bool SetVolume(float f)
    {
        player = GetComponent<AudioSource>();
        player.volume = f;
        return player.volume > 0.1;
    }

    public void GreenLight()
    {
        player = GetComponent<AudioSource>();
        player.Play();

        Invoke("OnEdge", MidiTimer.MidiClipLength);
    }
}

