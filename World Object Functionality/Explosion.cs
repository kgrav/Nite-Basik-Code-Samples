using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class Explosion
    {
        //Create an force which instantaneously extends from a point in space
        //and affects a sphere around it.
        //Built for grenades, but can be used for anything that behaves that way.
        //pass true for 'nulldmg' for a physical force that does not
        //damage the vitality of  
        public Explosion(Transform caster, Vector3 position, float force, float radius, float pain, float splashstart, bool nulldmg)
        {
            Collider[] hits = Physics.OverlapSphere(position, radius);
            HitZone h;
            WorldObject w;
            foreach (Collider c in hits)
            {
                float dist = Vector3.Distance(position, c.transform.position);
                Vector3 dir = c.transform.position - position;
                h = c.transform.GetComponent<HitZone>();
                w = c.transform.GetComponent<WorldObject>();
                AICharacter ch = caster.GetComponent<AICharacter>();
                float forceout = force;
                if (ch &&!ContextBody.Same(caster, c.transform))
                {
                    ch.SuccessfulWeaponHit(c.transform);
                }
                if (dist > splashstart)
                {
                    forceout = force * ((dist - splashstart) / (radius - splashstart));
                }
                if (h && !ContextBody.Same(caster, c.transform))
                    h.BluntOn(dir, pain, nulldmg, caster);
                if (w)
                    w.Strike(dir, forceout);
            }
        }
    }

