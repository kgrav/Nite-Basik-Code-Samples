using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class TrTranslation : Transformation
    {
        Vector3 too, toon;
        Vector3 vec, vecn;
        float rate;
        public string debugmessage;

        bool local;

        public TrTranslation(Transform t, Vector3 q, float r)
        {
            too = t.position + t.TransformDirection(q);
            debugmessage = too + ", " + q + " , " + t; 
            toon = t.position;
            rate = r;
            vec = -(too - t.position).normalized;
            Debug.DrawLine(t.position, too, Color.cyan, 2.0f);
            vecn = -(t.position - q).normalized;
        }

        public bool Exec(Transform t)
        {
            t.Translate(vec * rate * Time.deltaTime);
            if (Vector3.Magnitude(t.position - toon) > Vector3.Magnitude(too - toon))
                return true;
            return false;
        }

        public bool ExecInverse(Transform t)
        {
            t.Translate(vecn * rate * Time.deltaTime);
            if (Vector3.Magnitude(t.position - too) > Vector3.Magnitude(toon - too))
                return true;
            return false;
        }
    }

