using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class TrScale : Transformation
    {
        Vector3 from, to, dist;
        Vector3 incr;
        int fptr;
        int frames;
        public TrScale(int frames, Transform t, Vector3 goal)
        {
            this.frames = frames;
            fptr = 0;
            from = t.localScale;
            to = goal;
            dist = to - from;
            incr = dist / frames;
        }

        public bool Exec(Transform t)
        {
            if (fptr < frames)
            {
                t.localScale += incr;
                fptr++;
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ExecInverse(Transform t)
        {
            if(fptr > 0)
            {
                t.localScale -= incr;
                fptr--;
                return false;
            }
            else
            {
                return true;
            }
        }
    }

