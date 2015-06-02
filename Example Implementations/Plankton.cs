using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[RequireComponent(typeof(Ion))]
[RequireComponent(typeof(ContextBody))]

[AddComponentMenu("Nite-Basik/Header Scripts/Plankton")]
public class Plankton : AICharacter
    {
    Renderer rend;
    Ion io;
    Rigidbody rigb;
    Collider[] cs;
    bool freezevelocity = false;
    public Vector3 Centerpiece;
    Vector3 storeVelocity;
        protected override void ActivateFunc()
        {
            foreach (Collider c in cs)
            {
                if(c.isTrigger)
                c.enabled = true;
            }
            io.enabled = true;
            rend.enabled = true;
            rigb.velocity = storeVelocity;
            rigb.isKinematic = false;
        }

        protected override void DeactiveFunc()
        {
            
        }

        protected override void DeactivateFunc()
        {
            if (!freezevelocity)
            {
                storeVelocity = rigb.velocity;
                foreach (Collider c in cs)
                {
                    if(c.isTrigger)
                    c.enabled = false;
                }
                io.enabled = false;
                rend.enabled = false;
                rigb.velocity = Vector3.zero;
                rigb.isKinematic = true;
            }
        }
        Quaternion angurot;
        protected override void InitFunc()
        {   rigb = GetComponent<Rigidbody>();
        io = GetComponent<Ion>();
        rend = GetComponent<Renderer>();
        cs = GetComponents<Collider>();
        angurot = Quaternion.Euler(0, 90, 0);
        }

        protected override void UpdateFunc()
        {
            storeVelocity = angurot * (transform.position - Centerpiece);
            rigb.velocity = storeVelocity;
        }

        void OnTriggerEnter(Collider c)
        {
            if (!rigb)
                rigb = GetComponent<Rigidbody>();
            if (rigb) 
            rigb.AddForce((transform.position - c.transform.position)*0.75f*rigb.velocity.magnitude);

        }
        
    }

