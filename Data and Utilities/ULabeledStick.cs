using UnityEngine;
using System;
using System.Collections.Generic;

    public class ULabeledStick : IControl
    {
        Vector3 stick;
        Vector3 forward;
        Vector3 mem;
        Vector3 raw, rawmem;
        bool motion;
        string nameh;
        string namev;
        float invx;
        float invy;
        float tolerance;
        float mag;
        public char ident;

        public ULabeledStick(string nh, string nv, float tol, char c, bool invertx, bool inverty)
        {
            
            tolerance = tol;
            nameh = nh;
            namev = nv;
            invx = 1.0f;
            invy = 1.0f;
            if (invertx)
                invx = -1.0f;
            if (inverty)
                invx = -1.0f;
            ident = c;
            raw = Vector3.zero;
            forward = Vector3.forward;
            mem = Vector3.zero;
            motion = false;
            stick = Vector3.zero;
        }

        public void Frame()
        {
            forward = Camera.main.transform.TransformDirection(Vector3.forward);
            forward.y = 0.0f;
            forward = forward.normalized;
            Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
            
            float h = invx*Input.GetAxis(nameh);
            float v = invy*Input.GetAxis(namev);
            rawmem = raw;
            if ((h > tolerance || h < -tolerance) || (v > tolerance || v < -tolerance))
            {
                mem = (h * right + v * forward).normalized;
                raw = new Vector3(Input.GetAxis(nameh), 0, Input.GetAxis(namev));
                motion = true;
            }
            mag = (h * right + v * forward).magnitude;
            stick = (h * right + v * forward).normalized;
        }

        public UStickReceipt GetMotion()
        {
            return new UStickReceipt(ident, motion, stick, mag, raw);
        }

        public UStickReceipt PrevMotion()
        {
            return new UStickReceipt(ident, true, mem, mag, rawmem);
        }
    }

