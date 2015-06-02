using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class TrRelocate : Transformation
    {
        Vector3 orig, too;
        Vector3 vec;
        float rate;
        public string debugmessage;


        public TrRelocate(Transform t, Vector3 tooo, float r)
        {
            orig = t.localPosition;
            too = t.TransformPoint(tooo);
            debugmessage = too + ", " + tooo + " , " + t; 
            vec = (t.TransformPoint(tooo) - t.localPosition).normalized;
            rate = r;
        }

        public bool Exec(Transform t)
        {
            t.Translate(vec * rate * Time.deltaTime);
            if (Vector3.Distance(t.localPosition, too) < 0.3f)
                return true;
            return false;
        }

        public bool ExecInverse(Transform t)
        {
            t.position = orig;
            return true;
        }
    }

