using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

    public class VitalStat
    {
        char callback;
        int ind;
        string name;


        float currval;
        float maxval;

        public void NewMax(float newm)
    {
        maxval = newm;
        if (currval > maxval)
            currval = maxval;
    }
        public void IncrMax(float inc)
        {
            maxval += inc;
            if (currval > maxval)
                currval = maxval;
        }

        public VitalStat(char c, int i, string n, float m, bool startatmax)
        {
            callback = c;
            ind = i;
            name = n;
            maxval = m;

            currval = 0;
            if (startatmax)
                currval = maxval;
        }

        public bool Query(float f)
        {
            if (currval - f < 0)
                return false;
            else
                return true;
        }

        public float Percentage()
        {
            return currval / maxval;
        }

        public bool Query(int i)
        {
            if (currval - i < 0)
                return false;
            else
                return true;
        }

        public float GetVal()
        {
            return currval;
        }

        public int GetValDiscrete()
        {
            return (int)Math.Round(currval);
        }

        public void Use(float f)
        {
            currval -= f;
            if (currval < 0)
                currval = 0;
        }

        public void Use(int i)
        {
            currval -= i;
            if (currval < 0)
                currval = 0;
        }

        public void Regen(float f)
        {
            currval += f;
            if (currval > maxval)
                currval = maxval;
        }

        public void Regen(int i)
        {
            currval = i;
            if (currval > maxval)
                currval = maxval;
        }
    }

