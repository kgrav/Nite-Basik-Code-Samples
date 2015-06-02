using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;


    public class Audiomation
    {
        int laps; //Length of the audio animation in # of repetitions
        int place;
        int sequence;
        int S;
        int SN;
        int members;

        string fname;

        bool isplaying;
        bool isqueued;

        int[][] DB;
        int[] Transition;
        //Holds song names at (Frame, Bandmember)

        public Audiomation(string file, bool initanimation)
        {
            fname = file;
            ReadFile();
            if (initanimation)
            {
                isplaying = true;
            }
        }


        public int[][] Start()
        {
            int[][] r = new int[2][];
            if (laps > 2)
            {
                SN = 2;
            }
            else
            {
                SN = 0;
            }
            r[0] = DB[0];
            r[1] = DB[1];
            return r;
        }

        public int[] TransitionFrom()
        {
            SN = 0;
            return Transition;
        }

        public int[] ChangeFrames()
        {
            S = SN;
            int[] r = DB[S];
            SN++;
            if(SN >= laps)
            {
                SN = 0;
            } 
            return r;
        }
        public int Pos()
        {
            return SN;
        }

            public void ReadFile()
            {
                StreamReader animfile = new StreamReader("Assets/" + fname + ".am");
                string ts = animfile.ReadLine();
                string[] splits = ts.Split(':');
                int phase = 0;
                int frame = -1;
                while (ts != null)
                {
                    if (ts[0] == '.')
                    {
                        phase++;
                        try
                        {
                            ts = animfile.ReadLine();
                            splits = ts.Split(':');
                        }
                        catch (EndOfStreamException e)
                        {
                            ts = null;
                        }
                        continue;
                    }
                    else if(ts[0] == '>')
                    {
                        frame = -1;

                    }
                    else if(ts[0] == '<')
                    {
                        break;
                    }
                    switch (phase)
                    {
                        case 0:
                            int t1 = Convert.ToInt32(splits[0]);
                            int t2 = Convert.ToInt32(splits[1]);
                            int t3 = Convert.ToInt32(splits[2]);
                            DB = new int[t1][];
                            laps = t1;
                            members = t2;
                            for (int i = 0; i < t1; ++i)
                            {
                                DB[i] = new int[t2];
                            }
                            Transition = new int[t2];
                            break;
                        case 1: 
                            t1 = Convert.ToInt32(splits[0]);
                            t2 = Convert.ToInt32(splits[1]);
                            Transition[t1] = t2;
                            break;
                        case 2:
                            if (frame == -1 && ts[0] != '>')
                            {
                                frame = Convert.ToInt32(splits[0]);
                            }
                            else if (ts[0] != '>')
                            {
                                t1 = Convert.ToInt32(splits[0]);
                                t2 = Convert.ToInt32(splits[1]);
                                DB[frame][t1] = t2;


                            }
                            break;
                    }
                    
                try
                {
                    ts = animfile.ReadLine();
                    splits = ts.Split(':');
                }
                catch (EndOfStreamException e)
                {
                    ts = null;
                }
                }
            }

    }

