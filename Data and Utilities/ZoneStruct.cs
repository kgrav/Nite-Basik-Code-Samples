using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class ZoneStruct: MonoBehaviour
    {

        protected virtual void OnLoad()
        {

        }

        protected virtual void OnDisplay()
        {

        }

        protected virtual void OnDeload()
        {

        }

        public void Load()
        {
            if(pol)
            pol.enabled = true;
            foreach (Collider c in collids)
            {
                if(c)
                c.enabled = true;
            }
            OnLoad();
        }

        public void Display()
        {
            if(par)
            par.enableEmission = true;
            foreach (Light l in lites)
            {
                if(l)
                l.enabled = true;
            }
        }

        public void Deload()
        {
            if(pol)
            pol.enabled = false;
            if(par)
            par.enableEmission = false;
            for (int i = 0; i < Math.Max(lites.Length, collids.Length); ++i)
            {
                if (i < lites.Length)
                {
                    if(lites[i])
                    lites[i].enabled = false;
                }
                if (i < collids.Length)
                { if(collids[i])
                    collids[i].enabled = false; 
                }
            }
        }

        Pole pol;
        ParticleSystem par;
        Light[] lites;
        Collider[] collids;

         void Start()
        {
            pol = GetComponent<Pole>();
            par = GetComponent<ParticleSystem>();
            lites = GetComponents<Light>();
            collids = GetComponents<Collider>();
             if(pol)
            pol.enabled = false;
             if(par)
            par.enableEmission = false;
            for (int i = 0; i < Math.Max(lites.Length, collids.Length); ++i)
            {
                if (i < lites.Length)
                    lites[i].enabled = false;
                if (i < collids.Length)
                    collids[i].enabled = false;
            }
        }
    }

