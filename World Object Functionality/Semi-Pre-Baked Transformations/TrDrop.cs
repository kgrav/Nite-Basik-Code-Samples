using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class TrDrop : Transformation
    {
        Vector3 dir;
        float r;
        int g;

        public TrDrop(Transform t, float ra)
        {
            g = 0;
            r = ra;
            dir = Vector3.down;
        }

        public bool Exec(Transform t)
        {
            if (g >= 100)
                return true;
            ++g;
            t.Translate(dir * r * Time.deltaTime);
            return false;
        }
        public bool ExecInverse(Transform t)
        {
            if (g <= 0)
                return true;
            --g;
            t.Translate(-dir * r * Time.deltaTime);
            return false;
        }
    }
