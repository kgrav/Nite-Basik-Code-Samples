using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Nite-Basik/Action Coordination/Everyone/Hit Zone")]
//damage receiver that returns damage to Mortality in parent
    //Attach to the bone you want to define as a hit zone, and the logic will move
    //with the animation.
    public class HitZone : MonoBehaviour
    {
        public float DmgMultiplier;
        public bool ParentSide;
    public bool protectd;
    public bool Hard;
    public bool ignoreintegrity;
    bool localdead;
    public float maxintegrity;
    public Transform WhosTheDaddy;
    float integrity;
    public Transform[] protects;
        Mortality con;

        void Start()
        {
            localdead = false;
            WhosTheDaddy = transform.root;
            integrity = maxintegrity;
            if (ParentSide)
            {
                con = GetComponent<Mortality>();
            }
            else
            {
                con = WhosTheDaddy.GetComponent<Mortality>();
            }
        }

        void Update()
        {
            reflecting = con.reflect;
            if (localdead && protects.Length > 0)
            {
                foreach (Transform t in protects)
                {
                    t.GetComponent<HitZone>().protectd = false;
                }
            }
            else if (protects.Length > 0)
            {

                foreach (Transform t in protects)
                {
                    t.GetComponent<HitZone>().protectd = true;
                }
            }
        }

        public void Restore(float f)
        {
            integrity += f;
            if (integrity > maxintegrity)
                integrity = maxintegrity;
        }
        public bool reflecting;
    void RemoveEffect()
    {
        Destroy(GetComponentInChildren<DangerFluid>().gameObject);
    }
        public void DangerFluid(DangerFluid v)
        {
            if (v.contagious && !GetComponentInChildren<DangerFluid>())
            {
                bool spread = true;
                int[] resist = con.ElementalImmunities;
                foreach (int q in resist)
                {
                    if (q == v.type)
                    {
                        spread = false;
                    }
                }
                if (spread)
                {
                    GameObject pchild = (GameObject)GameObject.Instantiate(v.prefab);
                    pchild.transform.parent = transform;
                    pchild.transform.localPosition = Vector3.zero;
                    Invoke("RemoveEffect", v.SELength);
                }
            }
            con.DangerFluid(v);
        }
        public Vector3 BluntOn(Vector3 impulse, float force, bool nulldmg, Transform source)
        {
            if (!con.invulor)
            {
                if (!ignoreintegrity)
                {
                    integrity -= force * DmgMultiplier;
                    if (integrity < 0 && !ignoreintegrity)
                    {
                        localdead = true;
                    }
                }
                if (!localdead && !protectd)
                {
                    if (!WhosTheDaddy.GetComponent<AICharacter>())
                    {
                        con.GetComponent<BaseMover>().HitWithAtk(impulse, force);
                        con.GetComponent<AnimatorTranceiver>().SpecialTrig("OR");
                    }
                        if (nulldmg)
                            return con.BluntOn(impulse, 0, source);
                        return con.BluntOn(impulse, force * DmgMultiplier, source);
                    
                }
                else if (con.reflect)
                {
                    return -impulse * force;
                }
                    return -impulse;
            }
            else
                return Vector3.zero;
        }

        public void BluntOff()
        {
            con.BluntOff();
        }

        public void Shoot(Vector3 at, Vector3 impulse, float force, bool nulldmg)
        {
        if (!con.invulor)
        {
            integrity -= force * DmgMultiplier;
            if (integrity < 0 && !ignoreintegrity)
            {
                localdead = true;
            }
            if (!protectd)
            {
                    if(!nulldmg)
                    con.DirectDamage(force * DmgMultiplier);
                    con.Shoot(impulse, at, force);
                    con.PlaySound();
                                    
            }
        }
        }
    }

