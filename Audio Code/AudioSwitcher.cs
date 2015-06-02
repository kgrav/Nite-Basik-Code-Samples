using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
//Audio Mask system.
//Loads in clip filenames and masks that will be used
//to produce dynamic background music in a scene.
[AddComponentMenu("Nite-Basik/Sound/Audio Switchboard")]
    public class AudioSwitcher : MonoBehaviour
    {
    public static Transform Audio;
    public static AudioSwitcher Statref;
        public string manny;
        public Transform timer;
        string[][] LUT;
        AudioMask[] patterns;
        MidiStereoRegister[] ms;
        public int members;
        public int defaultMask;
        int[] ActiveMasks;
        int[] AMInds;

        //Default function
        public void MergeAdditive(int i)
        {
            int[,] s = patterns[i].Init();
            for (int q = 0; q < patterns[i].Count(); ++q)
            {
                if(ActiveMasks[s[0,q]] < 0){
                ActiveMasks[s[0, q]] = s[1, q];
                AMInds[q] = i;}
            }
        }

        public void MergeSubtractive(int i)
        {
            int[,] s = patterns[i].Init();
            for (int q = 0; q < patterns[i].Count(); ++q)
            {
                ActiveMasks[s[0, q]] = s[1, q];
                AMInds[q] = i;
            }
        }
        
        public void MuteMask(int i)
        {
            for (int j = 0; j < members; ++j)
            {
                if(AMInds[j] == i)
                {
                    AMInds[j] = -1;
                    ActiveMasks[j] = -1;
                    
                }
            }
        }
        //Once a mask has been loaded and confiremd, signal appropriate registers to fade to next mask.
        public void SwitchToLoadedMask()
        {
            foreach (int i in ChangeOnLoad)
            {
                ms[i].SetupFade();
            }
            ChangeOnLoad = new List<int>();
        }
        
        //Load the next set of songs into the appropriate register from the mask at index m
        List<int> ChangeOnLoad;
        public void LoadMask(int m)
        {
            int[,] ld = patterns[m].Init();
            for (int i = 0; i < patterns[i].Count(); ++i)
            {
                int track, song;
                track = ld[0, i];
                song = ld[1, i];
                ActiveMasks[track] = song;
                AMInds[track] = m;
                if (song >= 0)
                {
                    AudioClip sng = (AudioClip)Resources.Load(LUT[track][song]);
                    ChangeOnLoad.Add(track);
                    ms[track].Load(sng, song);
                }
                else if (song == -1)
                {
                    ChangeOnLoad.Add(track);
                    ms[track].Load(MidiTimer.NullClip, -1);
                    
                }
            }
        }


        void Start()
        {
            ChangeOnLoad = new List<int>();
            Audio = transform;
            Statref = this;
            ReadManifest();
            ActiveMasks = new int[members];
            AMInds = new int[members];
            MidiStereoRegister[] mss = GetComponentsInChildren<MidiStereoRegister>();
            ms = new MidiStereoRegister[members];
            for (int i = 0; i < members; ++i)
            {
                ActiveMasks[i] = -1;
                
                ms[mss[i].MemberNum] = mss[i];
                AMInds[i] = -1;
            }
            int[,] s = patterns[defaultMask].Init();
            AudioClip r1;
            for (int i = 0; i < patterns[defaultMask].Count(); ++i)
            {
                ActiveMasks[s[0, i]] = s[1, i];
                AMInds[s[0, i]] = defaultMask;
            }
            for (int i = 0; i < members; ++i)
            {
                if (ActiveMasks[i] > -1)
                {
                    r1 = (AudioClip)Resources.Load(LUT[i][ActiveMasks[i]]);
                 
                    print(ms[i] + ": " + r1);
                    ms[i].Init(r1, ActiveMasks[i]);

 
                }
                else
                {
                    print(ms[i] + ": " + ActiveMasks[i]);
                    ms[i].Init();
                }
            }
            for (int i = 0; i < members; ++i)
            {
                ms[i].GreenLight();
            }

        }
    /********
     * MANIFEST GUIDELINES FOR AUDIO SWITCHER:
     * FIRST LINE: INT1:INT2:
     * INT1: #of Audio Masks INT2: # of instruments
     * Each Instrument:
     * LINE 1: INT1:INT2
     * INT1: Index of instrument, INT2: number of clips the instrument represents
     * LINES 2-n: INT1:INT2:FNAME
     * INT1: instrument index, INT2: clip index, FNAME: path of the clip file (within Assets/Resources)
     * Example midi manifests can be found below.
     * *******/

        public void ReadManifest()
        {
            string path = "Assets/MIDIMANIFEST" + manny + ".txt";
            StreamReader mani = new StreamReader(path);
            string[] splits;

            int phase = 0;
            string ts = mani.ReadLine();
            splits = ts.Split(':');
            while (ts != null)
            {
                if (ts[0] == '.')
                    phase++;
                else
                {
                    switch (phase)
                    {

                        case 0:
                            print(splits[0] + ", " + splits[1]);
                            int t1 = Convert.ToInt32(splits[0]);
                            int t2 = Convert.ToInt32(splits[1]);
                            LUT = new string[t1][];
                            patterns = new AudioMask[t2];
                            break;
                        case 1:
                            if (splits[0].Equals("INS"))
                            {

                                t1 = Convert.ToInt32(splits[1]);
                                t2 = Convert.ToInt32(splits[2]);
                                LUT[t1] = new string[t2];

                            }
                            else
                            {

                                t1 = Convert.ToInt32(splits[0]);
                                t2 = Convert.ToInt32(splits[1]);
                                LUT[t1][t2] = splits[2];
                            }
                            break;
                        case 2:
                            t1 = Convert.ToInt32(splits[0]);
                            patterns[t1] = new AudioMask(splits[1]);
                            break;
                    }
                }

                try
                {
                    ts = mani.ReadLine();
                    if (ts != null)
                        splits = ts.Split(':');
                }
                catch (EndOfStreamException e)
                {
                    ts = null;
                }
            }
        }
    }

