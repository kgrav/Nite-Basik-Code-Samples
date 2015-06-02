using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Creates a 'magnetic field,'
//attracting all colliders with an attached Ion
//that pass into its trigger zone.
//Attracts these objects towards the Pole object's zero point;
[RequireComponent(typeof(Collider))]
[AddComponentMenu("Nite-Basik/Mechanical Constructs/Pole (magnetic field)")]
    public class Pole : MonoBehaviour
    {
    public bool repelfield;
    //field which repels ions;
    public bool blackfield;
    //field which has either positive energy on the shell and negative energy within, (repelfield = true)
    //or negative energy on the shell and positive energy inside (repelfield = false)
        public float radius;
        //The point at which 
        public float initialpull;
        //The force applied upon entering the zone.
        

        public float inertialpull;
        //The force applied while in the zone
    
        public float radialpower;
        //The rate at which     

        public int[] attractTheseIons;
        //This pole will attract Ions which have Ion keys included in this array.
        
        public int polekey;
        bool active = true;
        public void Deactivate()
        {
            active = false;
        }

        public void Reactivate()
        {
            active = true;
        }

        public void SwitchPoles()
        {
            repelfield = !repelfield;
        }

        public void Deactivate(float t)
        {
            active = false;
            Invoke("Reactivate", t);
        }


        public void SwitchPoles(float t)
        {
            repelfield = !repelfield;
            Invoke("SwitchPoles", t);
        }
        void OnTriggerEnter(Collider c)
        {
            if (active)
            {
                if (c.transform.name.Equals(transform.name))
                    return;
                if (c.GetComponent<Ion>() && c.GetComponent<Ion>().enabled)
                {
                    int q = c.GetComponent<Ion>().IonKey;
                    bool valid = false;
                    for (int i = 0; i < attractTheseIons.Length; ++i)
                    {
                        if (q == attractTheseIons[i])
                            valid = true;
                    }
                    if (valid)
                    {
                        Vector3 dir = transform.position - c.transform.position;
                        Rigidbody rdj = c.GetComponent<Rigidbody>();
                        if ((repelfield && !blackfield) || (!repelfield && blackfield))
                            dir *= -1;
                        rdj.AddForce(dir * initialpull);
                    }
                }
            }
        }
        void OnTriggerExit(Collider c)
        {
            if (blackfield&&c.GetComponent<Ion>())
            {
                c.GetComponent<Rigidbody>().AddForce((transform.position - c.transform.position) * 1.5f * c.GetComponent<Rigidbody>().velocity.magnitude);
            }
        }
        void OnTriggerStay(Collider c)
        {
            if (active)
            {
                if (c.transform.name.Equals(transform.name))
                    return;
                if (c.GetComponent<Ion>() && c.GetComponent<Ion>().enabled)
                {
                    int q = c.GetComponent<Ion>().IonKey;
                    bool valid = false;
                    for (int i = 0; i < attractTheseIons.Length; ++i)
                    {
                        if (q == attractTheseIons[i])
                            valid = true;
                    }
                    if (valid)
                    {
                        Vector3 dir = transform.position - c.transform.position;
                        Rigidbody rdj = c.GetComponent<Rigidbody>();

                        if (repelfield && !blackfield || !repelfield && blackfield)
                            dir *= -1;
                        float force = (radius / Vector3.Distance(transform.position, c.transform.position)) * inertialpull;
                        rdj.AddForce(dir * inertialpull);
                    }
                }
            }
        }
    }
