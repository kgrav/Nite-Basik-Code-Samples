using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

    public class TrPause : Transformation
    {
        int fr, frn;

        public TrPause(int length)
        {
            fr = length;
            frn = 0;
        }

        public bool Exec(Transform t)
        {
            frn++;
            if (frn == fr)
                return true;
            return false;
        }

        public bool ExecInverse(Transform t)
        {
            return true;
        }
    }

