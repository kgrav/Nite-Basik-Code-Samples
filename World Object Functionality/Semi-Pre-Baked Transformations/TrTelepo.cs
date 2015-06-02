using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

    public class TrTelepo : Transformation
    {
        Vector3 fom, too;

        public TrTelepo(Vector3 pos, Vector3 n)
        {
            fom = pos;
            too = n;
        }


        public bool Exec(Transform t)
        {
            t.position = too;
            return true;
        }

        public bool ExecInverse(Transform t)
        {
            t.position = fom;
            return true;
        }
    }

