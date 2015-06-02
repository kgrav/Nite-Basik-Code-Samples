using UnityEngine;
using System;
using System.Collections.Generic;

    public struct UStickReceipt
    {
        public char a;
        public Vector3 v;
        public Vector3 raw;
        public float mag;
        public bool m;

        public UStickReceipt(char i, bool j, Vector3 k, float f, Vector3 r)
        {
            raw = r;
            a = i;
            v = k;
            m = j;
            mag = f;
        }
    }

