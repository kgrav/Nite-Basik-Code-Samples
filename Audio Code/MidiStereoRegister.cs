using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Fades between two Audio Outputs.
//The next audio clip can be loaded into
//the inactive audio source,
//and faded into.
[AddComponentMenu("Nite-Basik/Sound/Midi Stereo Register")]
    public class MidiStereoRegister : MonoBehaviour
    {

        public int MemberNum; //Member callnumber, synonymous with index in manifes.t.
        public bool AnimStream; // TRUE if using Audio Animaions, FALSE if using Audio Masks
        public float fadestrength; // <1.  The amount volume will decrease per second while fading.
        MidiHOTLoader[] register; // Audio Sources (in children)
        float[] fades; //stored fade data, updated per-frame during fade
        AudioClip mem; //next audio clip
        int curr = 0; //index of current audio clip (from manifest)
        public int on = 0; //Midi Loader that is currently playing (or being faded from)
        public int off = 1; //Midi Loader that is currently muted (or being faded to)
        public bool CrossFader; //True for fadem itgerwuse fakse, don't set FadeStrength to 0.
    //From the manifest:
        int indexofcurrent; //current song
        int indexofnext; // next song
        bool fade; //is fading
        bool faded; //is finished

        void Start()
        {
            mem = null;
        }

        void Update()
        {
            if (fade)
            {//if fading, decrease volume of on register and increase volume of off register 
                fades[on] -= fadestrength*Time.deltaTime;
                fades[off] += fadestrength*Time.deltaTime;
                register[off].SetVolume(fades[off]);
                if (!register[on].SetVolume(fades[on]))
                {
                    faded = true;
                    fade = false;
                }
            }
            //If one reaches an invalid volume (< 0 or > 1), snap volumes to min and max, and update
            //the off and on volumes of this Stereo Register.
            //(volume binding routines are contained Midi Loader-level).
            if (faded)
            {
                faded = false;
                int t = on;
                on = off;
                off = t;
            }
        }
        //Load the starting audio clip, prepare registers.
        public void Init(AudioClip primary, int ind)
        {
            GetLoaders();
            register[on].Init(primary);
            register[off].Init(primary);
            mem = primary;
            indexofnext = ind;
            indexofcurrent = ind;
        }
        //Initialize with the Timer's null clip.
        public void Init()
        {
            GetLoaders();
            register[on].Init(MidiTimer.GetNullClip());
            register[off].Init(MidiTimer.GetNullClip());
            mem = MidiTimer.NullClip;
            indexofcurrent = -1;
            indexofnext = -1;
        }
        //Initialize with a primary (on) clip and a secondary (next) clip.
    //Use with old Audiomation system.
        public void Init(AudioClip primary, AudioClip secondary, int pi, int si)
        {
            GetLoaders();
            register[on].Init(primary);
            mem = secondary;
            register[off].Init(secondary);
            indexofcurrent = pi;
            indexofnext = si;
            if (CrossFader && AnimStream)
                Invoke("SetupFade", MidiTimer.MidiClipLength);
            
        }

        public void SetupFade(){
            if (indexofcurrent == indexofnext)
            {
                fade = false;
                faded = false;
            }
            else if (fadestrength < 1)
            {
                
                faded = false;
                fade = true;
                fades = new float[2];
                fades[on] = 1.0f;
                fades[off] = 0.0f;
            }
            }
        int next = -1;
  //Audio Masks: load and save next audio clip
    public void Load(AudioClip nextClip, int k)
        {
            //LADYSTARDUSTCAMERA.DBM("Loading Clip" + nextClip, transform.name);
            indexofnext = k;
            register[off].HotFlex(nextClip);
        }
        //Audiomation system, buffer an opcode instead of a clip:
    //-1 for switch to null track
    //-2 for repeat
        public void FrameBuffer(int k)
        {
            if (k == -2)
                register[on].SetVolume(0);
            else if (k == -1)
                FrameBuffer(MidiTimer.GetNullClip(), -1);
        }
    //Audiomations, Buffer the next frame's audio clip.
        public void FrameBuffer(AudioClip nextclip, int nextind)
        {

            mem = nextclip;
            print(mem + "BUFFERED");
            if (fadestrength == 0)
                Invoke("SnapFade", MidiTimer.MidiClipLength / 2);
            else
            {
                register[off].HotFlex(mem);
                Invoke("SetupFade", MidiTimer.MidiClipLength / 4.0f);
            }
        }
    //Audiomations, set up the first fade.
        public void InitialFade()
        {
            print("SNAPFADE" + on + ">" + off);
            register[on].SetVolume(0);
            register[off].SetVolume(1);
            int t = on;
            on = off;
            off = t;
        }
    //Audiomations, snap into next song without fadiung.
        public void SnapFade()
        {
            print("SNAPFADE" + on + ">" + off);
            register[off].Switchto(mem);
            register[on].SetVolume(0);
            register[off].SetVolume(1);
            int t = on;
            on = off;
            off = t;
            
        }

        public void Frame(AudioClip nextclip, int nextind)
        {
            indexofcurrent = indexofnext;
            indexofnext = nextind;
            mem = nextclip;
        }

        //Turn on registers,
    //if using Audiomations, start audiomation loop.
        public void GreenLight()
        {
            GetLoaders();
            register[0].GreenLight();
            register[1].GreenLight();
            register[on].SetVolume(1);
            register[off].SetVolume(0);
            if(AnimStream)
            Invoke("InitialFade", MidiTimer.MidiClipLength - MidiTimer.MidiTime);
        }

        void GetLoaders()
        {
            register = new MidiHOTLoader[2];
            MidiHOTLoader[] m = GetComponentsInChildren<MidiHOTLoader>();
            register[m[0].index] = m[0];
            register[m[1].index] = m[1];
        }
    }

