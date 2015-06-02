using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Nite-Basik/Data/Plankton Colony")]
    public class PlanktonColony : Droogs
    {

        public int Count;
        public GameObject[] species;
        public float miny, maxy;
        protected override void ExtensionInit()
        {

            int sp = 0;
            float discretion = species.Length * 3;
            float random = UnityEngine.Random.Range(0.0f, discretion);
            Vector3 posit = new Vector3(0, miny, 0) +  transform.forward;
            Quaternion rotate = Quaternion.Euler(0, 10, 0);
            int flip = 1;
            for (int i = 0; i < Count; ++i)
            {
                for(int q = 0; q < species.Length; ++q)
                {
                    if(random < 3*(q+1))
                    {
                        sp = q;
                        break;
                    }
                }
                random = UnityEngine.Random.Range(0.0f, discretion);
                GameObject plankt = (GameObject)GameObject.Instantiate(species[sp], transform.position + posit, Quaternion.identity);
                Plankton plankton = plankt.GetComponent<Plankton>();
                plankton.Centerpiece = transform.position;
                plankton.LoadingGroup = ZoneID;
                plankton.SetZone(this);
                posit = rotate * posit;
                if (posit.y > maxy)
                {
                    flip = -1;
                }
                else if (posit.y < miny)
                {
                    flip = 1;
                }
            }
        }
    }

