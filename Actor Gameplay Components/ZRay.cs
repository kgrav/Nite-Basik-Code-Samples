using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Similar to X-Ray, but casts a sphere directly in front of holder,
//rather than a ray in an arbitrary direction.

//As this also uses Raycast Hits, XRay's "ValidHit" function is valid for this
//class's output, and a duplicate method is not needed.
    public class ZRay
    {
        int ferror;
        int contextignore;
        float eyesize;
        float range;
        bool hit;
        RaycastHit hitDefined;
        public ZRay(float iss, float ct, int err, int contextig)
    {
        ferror = err;
        eyesize = iss;
        range = ct;
        contextignore = contextig;
    }

        public bool ScanFrom(Transform t)
        {
            Vector3 dir = t.forward * ferror;
            Vector3 pt = t.position + dir*(2.0f * eyesize);
            if(Physics.SphereCast(new Ray(pt, dir), eyesize, out hitDefined, range))
            {
                return true;
            }
            hitDefined = default(RaycastHit);
            return false;
        }

        public RaycastHit Info()
        {
            return hitDefined;
        }
    }

