using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
//Class which holds the indexes of the Audio Tracks
//to be played at a certain times by the Audio Switcher.
//These are defined within external files which hold the prefix ".ak"
//The format is as follows:
/*LINE 1: INT1:INT2 -> where INT1 represents the number of Members (from the Midi Manifest) that are affected by this mask, and INT2 the number of frames (if using audiomation masks)
 * LINE 2: INT -> the number of the frame about to be declared,
 * LINES 3-n: INT1:INT2 -> the index of the instrument is INT1, and the index of the audio clip is INT2 (from the Midi Manifest)
 * LINE n: '<' -> end declaration with a '<' symbol, alternatively declare another frame with a '>' symbol.
 * An example Audio Mask file might look something like this:
 * 3:1
 * .
 * 0:
 * 1:2
 * 4:5
 * 5:1
 * <
 * 
 * 
 * When the example mask is switched to,
 * instrument 1 will fade in track 2, instrument 4 will fade in track 5, and instrument 5 will fade in track 1
 * /Use negative numbers for special ops:
 * -1 will be treated as the 'null clip' (properly timed silence) for the current audio set
 * -2 signals the audio manager to keep the track currently playing in the instrument it follows.
 *      This is the default behaviour for instruments not declared in the current mask, and is not needed most of the time.
 *        
 */
    public class AudioMask
    {
        int fptr;
        int frames;
        bool init = false;
        string fname;
        int[] mask;
        int[,] r;
        int[][] tracks;
        int[] key;
        public AudioMask(string file)
        {
            key = new int[16];
            for (int i = 0; i < key.Length; ++i)
            {
                key[i] = -1;
            }
            fname = file;
            ReadFile();
            r = new int[2, mask.Length];
            for (int i = 0; i < mask.Length; ++i)
            {
                key[mask[i]] = i;
            }
        }
        
        public int[,] Init()
        {
            frames = tracks.Length;
            fptr = 0;
            init = true;
            for (int i = 0; i < mask.Length; ++i)
            {
                r[0, i] = mask[i];

            }
            for (int j = 0; j < tracks[fptr].Length; ++j)
            {
                r[1,j] = tracks[fptr][j];
            }
            fptr++;
            if (fptr >= frames)
            {
                fptr = 0;
            }
            return r;

        }
        public int Translate(int index)
        {
            return key[index];
        }
        public int Count()
        {
            return tracks[0].Length;
        }
        public int[,] LastBuffer()
        {
            return r;
        }

        public int[,] Frame()
        {
            if (!init)
                return Init();
            for (int i = 0; i < mask.Length; ++i)
            {
                r[0, i] = mask[i];

            }
            for (int j = 0; j < tracks[fptr].Length; ++j)
            {
                r[1, j] = tracks[fptr][j];
            }
            fptr++;
            if (fptr >= frames)
            {
                fptr = 0;
            }
            return r;

        }
        public void ReadFile()
        {
            StreamReader animfile = new StreamReader("Assets/" + fname + ".ak");
            string ts = animfile.ReadLine();
            string[] splits = ts.Split(':');
            int phase = 0;
            int frame = -1;
            int mptr = 0;
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
                else if (ts[0] == '>')
                {
                    frame = -1;

                }
                else if (ts[0] == '<')
                {
                    break;
                }
                switch (phase)
                {
                    case 0:
                        int t1 = Convert.ToInt32(splits[0]);
                        int t2 = Convert.ToInt32(splits[1]);
                        mask = new int[t1];
                        tracks = new int[t2][];
                        for (int i = 0; i < t2; ++i)
                        {
                            tracks[i] = new int[t1];
                        }
                
                        break;
                    case 1:
                        if (frame == -1 && ts[0] != '>')
                        {
                            mptr = 0;
                            frame = Convert.ToInt32(splits[0]);
                        }
                        else if (ts[0] != '>')
                        {
                            t1 = Convert.ToInt32(splits[0]);
                            t2 = Convert.ToInt32(splits[1]);
                            
                            if (frame == 0)
                            {
                                mask[mptr] = t1;
                            }
                            tracks[frame][mptr] = t2;
                            mptr++;

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

