using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

[AddComponentMenu("Nite-Basik/Sound/Audio Animatronyx")]
    public class AudioAnimatronix : MonoBehaviour
    {
        MidiStereoRegister[] ms;

        public string manny;
        public int preload;
        public bool WholeAudiomation;
        public int members;
        Audiomation[] patterns;
        int activeanimation = 0;
        int nextanimation = -1;
        float clockedge;
        public String[][] LUT;
        
        bool once = false;
        bool transitiononframe, transitioned;
        int nextset;
        void Start()
        {
            ReadManifest();
            ms = new MidiStereoRegister[members];
            MidiStereoRegister[] t = GetComponentsInChildren<MidiStereoRegister>();
            foreach (MidiStereoRegister n in t)
            {
                ms[n.MemberNum] = n;
            }
            int[][] start = patterns[activeanimation].Start();
            for (int i = 0; i < members; ++i)
            {
                AudioClip x = (AudioClip)Resources.Load(LUT[i][start[0][i]], typeof(AudioClip));
                AudioClip a = (AudioClip)Resources.Load(LUT[i][start[1][i]], typeof(AudioClip));
                ms[i].Init(x, a, start[0][i], start[1][i]);
            }
            clockedge = MidiTimer.MidiClipLength/2;

        }

        public void Break(int i)
        {
            if (!WholeAudiomation)
            {
                activeanimation = i;
            }
            else
            {
                transitiononframe = true;
                transitioned = false;
                nextanimation = i;
            }
        }



        void Update()
        {
            if (!once)
            {
                for (int i = 0; i < members; ++i)
                {
                    ms[i].GreenLight();
                }
                Invoke("OnFrame", clockedge);
                once = true;
            }
        }

        int freg;

        public void OnFrame()
        {
            Invoke("OnFrame", MidiTimer.MidiClipLength);
                int[] nextload = patterns[activeanimation].ChangeFrames();
                if ((patterns[activeanimation].Pos() == 0) && WholeAudiomation == true)
                {
                    if (nextanimation != -1)
                    {
                        activeanimation = nextanimation;
                        nextanimation = -1;
                    }
                }
                for (int i = 1; i < members; ++i)
                {
                    AudioClip x = (AudioClip)Resources.Load(LUT[i][nextload[i]], typeof(AudioClip));
                    if (nextload[i] == -2)
                    {
                        ms[i].FrameBuffer(-2);
                    }
                    else
                    {
                        ms[i].FrameBuffer(x, nextload[i]);
                    }
                }
        }

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
                            patterns = new Audiomation[t2];
                            break;
                        case 1:
                            print(splits[0] + ", " + splits[1]);
                            t1 = Convert.ToInt32(splits[0]);
                                t2 = Convert.ToInt32(splits[1]);
                            if (splits.Length < 3)
                            {
                                LUT[t1] = new string[t2];

                            }
                            else
                            {
                                print(ts);
                                LUT[t1][t2] = splits[2]; 
                            }
                            break;
                        case 2:
                            t1 = Convert.ToInt32(splits[0]);
                            patterns[t1] = new Audiomation(splits[1], true);
                            break;
                    }
                }

                try
                {
                    ts = mani.ReadLine();
                    if(ts != null)
                    splits = ts.Split(':');
                }
                catch (EndOfStreamException e)
                {
                    ts = null;
                }
            }
        }

       

        
    }