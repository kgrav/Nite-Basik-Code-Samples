using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

    public class TrLateralPush : Transformation
    {
        
        Vector3 imp;
        float forc, forci, forcn;
        float drag;
        public TrLateralPush(Vector3 impulse, float force, float frict)
        {
            drag = frict;
            imp = impulse;
            forc = force;
            forci = forcn = forc;
        }

        public bool Exec(Transform t)
        {
            forcn = forc;
            t.Translate(imp * forci * Time.deltaTime);
            forci *= drag;
            if (forc < 1.0f)
                return true;
            return false;
        }
        public bool ExecInverse(Transform t)
        {
            forci = forc;
            t.Translate(-imp * forcn * Time.deltaTime);
            forci *= drag;
            if (forc < 1.0f)
                return true;
            return false;
        }


    }
