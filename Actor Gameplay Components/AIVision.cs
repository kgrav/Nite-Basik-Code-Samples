using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[AddComponentMenu("Nite-Basik/Action Coordination/AI/Vision")]
    public class AIVision : MonoBehaviour
    {
        public Vector3[] locations;
        public float[] offsets;
        public float periphAngle;


        ZRay fovea;
        XRay[] rays;
        int n;
        public float refractory;
        public float range;
        public float eyesize;
        public int fmod;
        
        float t;
        RaycastHit foval;
        RaycastHit[] los;
        bool turnoff = true;
        public bool newincenter;

        void Start()
        {
            n = locations.Length;
            rays = new XRay[n];
            los = new RaycastHit[n];
            t = 0;
            turnoff = false;
            fovea = new ZRay(eyesize, range, fmod, ContextBody.GroupOf(transform));
            foval = default(RaycastHit);//Initialize ray structures.
            for (int i = 0; i < n; ++i)
            {
                los[i] = default(RaycastHit);
                rays[i] = new XRay(Quaternion.Euler(0, offsets[i], 0)*transform.forward, locations[i], range);
            }
            fovea.ScanFrom(transform);
            foval = fovea.Info();
            for (int i = 0; i < n; ++i)
            {
                los[i] = rays[i].Scan(transform);
            }
            Invoke("ScanEmAll", refractory);
        }
        
        void ScanEmAll()
        {
            if (enabled)//Scan all rays, on a slightly offset loop in order to standardize AI performance
                //during irregular frame rates.
                //Additionally, this helps AI's "keep" their goal for a short while, to prevent crossing signals
                //in animation or audio.
            {
                if (fovea.ScanFrom(transform))
                { foval = fovea.Info(); newincenter = true; }
                else
                { newincenter = false; }
                for (int i = 0; i < n; ++i)
                {
                    los[i] = rays[i].Scan(transform);
                }
            }
            Invoke("ScanEmAll", refractory);
        }
        //Main AI Script receives this input manually
        public RaycastHit CenterRay()
        {
            return foval;
        }

        public RaycastHit[] Periphery()
        {
            return los;
        }
    }

