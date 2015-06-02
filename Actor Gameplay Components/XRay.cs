using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//Designed for raycast functions which depend on
//a constant number of rays at pre-determined directions
//relative to a transform.

//the XRay will return the default RaycastHit if no connection is made,
//therefore, it is advisable to use the ValidHit function to check the hit
//before processing.

//This allows for a consistent "field of vision" to be present,
//indicating detailed information about hits,
//while implicitly indicating whether or not contact was made.
    public struct XRay
    {
        Vector3 posit;
        Vector3 castDir;
        float castLength;
        Vector3 localcastdir;
        Vector3 localcastpos;
        bool rotator;
        public XRay(Vector3 a, Vector3 t, float b)
        {
            rotator = false;
            posit = t;
            localcastpos = posit;
            castDir = a;
            localcastdir = castDir;
            castLength = b - 0.10f ;
        }

        public XRay(float a, Vector3 t, float b)
        {
            posit = t;
            localcastpos = t;
            castDir = new Vector3(0, a, 0);
            localcastdir = new Vector3(0, a, 0);
            castLength = b;
            rotator = true;
        }

        public static bool ValidHit(RaycastHit r)
        {
            return !(r.Equals(default(RaycastHit)));
        }
        public RaycastHit Scan()
        {
            RaycastHit rh;
            Debug.DrawRay(localcastpos, localcastdir * castLength, Color.cyan, 2.0f);
            if (Physics.Raycast(new Ray(localcastpos, localcastdir), out rh, castLength))
            {
                return rh;
            }
            else
                return default(RaycastHit);

        }

        public RaycastHit Scan(Transform inp)
        {
            if (!rotator)
            {
                localcastpos = inp.position + posit;
                localcastdir = inp.TransformDirection(castDir);
            }
            else
            {
                localcastpos = inp.position + posit;
                localcastdir = Quaternion.Euler(castDir) * inp.forward;
            }
            return Scan();
        }
    
        
    }
