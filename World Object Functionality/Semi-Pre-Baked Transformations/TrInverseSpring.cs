using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
    public class TrInverseSpring : Transformation
    {
        float dnspeed, bot;
        float upspeed, top;
        float coif;
        Vector3 down;
        public TrInverseSpring(float amnt, float rate, Transform t)
        {
            bot = t.position.y - amnt;
            top = t.position.y;
            coif = rate / amnt;
            dnspeed = coif*2.0f;
            upspeed = coif/-10.0f;
            down = t.TransformDirection(Vector3.down);

        }

        public bool Exec(Transform t)
        {
            t.Translate(down * dnspeed * Time.deltaTime);
            if (t.position.y < bot)
                return true;
            return false;
        }

        public bool ExecInverse(Transform t)
        {
            t.Translate(down * upspeed * Time.deltaTime);
            if (t.position.y > top)
                return true;
            return false;
        }
    }

