using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class Rock : ItemInfo
    {
        static List<Rock> ROCKSINSCENE;
        static bool init = false;
        public static int RollCall(Rock r)
        {
            if (!init){
                ROCKSINSCENE = new List<Rock>();
                init = true;
            }
            ROCKSINSCENE.Add(r);
            return ROCKSINSCENE.IndexOf(r);
        }
        public Rock()
        {
            name = "ROCK";
            typeid = 3;
            instanceid = RollCall(this);
            collectible = false;
            floats = false;
            holdable = true;
            DESTROYonpickup = false;
            heavy = false;
            INVHeavy = false;
            DESTROYonuse = false;
            staticCollection = false;
        }

        public override void Throw(Transform instance, Vector3 dir)
        {
            instance.GetComponent<Rigidbody>().AddForce(dir*100 + Vector3.up*75);
        }

        public override void HitFromThrow(Transform instance, Transform target, Collision hitinfo)
        {
            Vector3 v = instance.GetComponent<Rigidbody>().velocity;
            float f = instance.GetComponent<Item>().mass;
            instance.GetComponent<Rigidbody>().AddForce(hitinfo.contacts[0].normal * hitinfo.relativeVelocity.magnitude);
            HitZone h = target.GetComponent<HitZone>();
            PhysicalWorldObject w = target.GetComponent<PhysicalWorldObject>();
            if (h != null)
            {
                h.BluntOn(v, f,false, instance);
            }
            else if (w != null)
            {
                w.Strike(v, f);
            }
        }
    }