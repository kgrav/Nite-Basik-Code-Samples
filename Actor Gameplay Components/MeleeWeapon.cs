using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum MWFLAGS { NONE, BLK, ATK }
//Melee weapon for player, AI, and world forces (such as Pistons)
[AddComponentMenu("Nite-Basik/Physical Objects/Melee Weapon")]
    [RequireComponent(typeof (Collider))]
[RequireComponent(typeof(Rigidbody))]
    public class MeleeWeapon : Weapon
    {
        public bool Sounds;
        public bool submesh;
        public bool constantImpulse;
        public bool nullDmg;
        public bool crit;
        public Transform weaponmesh;
        public int OnSound;
        public int PosContactSound;
        public int NegContactSound;
        public int triggerqueue = 0;
        public int CritSound;
        public int fmod;//multiplierfor direction.  0 for no impulse, -1 for reverse-aligned objects, 1 for correctly aligned objects
        public int context; // Ignores Hitzones whos context bodies have this group ID.
        Transform t;
        public Transform CIP;
        public Vector3 pos, prev, origin, impulse, localoffset;
        public float power;
        public float critpower;
        float currpower;
        public bool interruptable;
        public bool MASK_ALL;
        public float interruptcheck;
        public int SoundContext;
        Cabinet cab;
        public MWFLAGS activity;
        CombatInterface parent;
        AICharacter paren;
        Rigidbody rig;
        bool droogy = false;
        void Start()
        {
            rig = GetComponent<Rigidbody>();
            paren = transform.root.GetComponent<AICharacter>();
            if(paren)//check to see if we belong to an AI character
            {
                droogy = true;
            }
            t = GetComponent<Transform>();
            currpower = power;
            Transform l = transform;
            while (l.parent != null)
            {
                l = l.parent;
            }
            CIP = l;
            origin = t.localPosition;
            prev = t.localPosition;
            localoffset = Vector3.zero;
            pos = t.TransformPoint(Vector3.zero);
            activity = MWFLAGS.NONE;
            parent = GetComponentInParent<CombatInterface>();
            parent.RollCall(WeaponIndex, transform);
        }
        public void SpecialOn()
        {

            Invoke("SetMask", 0.4f);
        }
        void SetMask()
        {
            MASK_ALL = true;
            crit = true;
        }
        MWFLAGS pvac;

    //On button (logical or physical), play attack sound (weapon is turned on/off through combat interface,
    //however, weapons play from from their own individual audio source)
        public override void OBE()
        {
            if(Sounds)
            GetComponent<AudioSource>().PlayOneShot(SoundTable.GetSound(SoundContext, OnSound));
        }
        
        //Set activity; if turning off, also turn off special flag.
        public override void SetActivity(MWFLAGS f)
        {
            pvac = activity;
            if (f == MWFLAGS.NONE)
            {
                MASK_ALL = false;
                crit = false;
                rig.isKinematic = true;
            }
            else
            {
                rig.isKinematic = false;
            }
            activity = f;
        }
        protected virtual void ExtUpdate()
        {


        }

        public void OnCollisionEnter(Collision c)
        {
            float ptf = power;
            if (MASK_ALL || crit) //if using a special attack, do critical damage.
                ptf = critpower;
            if (activity == MWFLAGS.ATK)
            {

                if (!c.gameObject.Equals(parent.gameObject))
                {
                    HitZone rate = c.gameObject.GetComponent<HitZone>();
                    WorldObject wor = c.gameObject.GetComponent<WorldObject>();


                    PhysicalWorldObject w2 = c.gameObject.GetComponent<PhysicalWorldObject>();
                    if (rate && ContextBody.GroupOf(rate.transform) != context) //if collided with an external hitzone,
                    {
                        print("Hitzone Collision " + ContextBody.GroupOf(rate.transform) + ", " + context);
                        if (rate.Hard) //if that hitzone is armored, attack fails.
                        {
                            if (droogy)
                            {
                                paren.FailedWeaponHit(rate.transform);
                            }
                            if (interruptable)
                            {

                                GetComponent<AudioSource>().PlayOneShot(SoundTable.GetSound(SoundContext, NegContactSound));
                                parent.AtkFail();
                            }
                            if (ContextBody.GroupOf(c.transform) != context)
                                parent.WeaponCollisionEvent();
                        }
                        else if (rate.reflecting) //otherwise, if the hitzone is reflective, attack fails and character is knocked back.
                        {
                            GetComponent<AudioSource>().PlayOneShot(SoundTable.GetSound(SoundContext, NegContactSound));
                            if (droogy)
                            {
                                paren.FailedWeaponHit(rate.transform);
                            }
                            parent.GetComponent<BaseMover>().Reflect(-CIP.TransformDirection(impulse), ptf);

                        }//Otherwise, the attack goes through; do damage to enemy hit zone.
                            //Invul periods ensure that an enemy will not be hit multiple times by the same strike,
                            //Even if the strike passes through multiple hitzones.
                            //The first hitzone stricken will begin the damage processing, which immediately triggers
                            //the Mortality's invul period.
                        else
                        {

                            GetComponent<AudioSource>().PlayOneShot(SoundTable.GetSound(SoundContext,PosContactSound));
                            if (droogy)
                            {
                                paren.SuccessfulWeaponHit(rate.transform.root);
                            }
                            if (!constantImpulse)
                            {
                                print(rate.WhosTheDaddy);
                                rate.BluntOn(-c.contacts[0].normal, ptf, nullDmg, CIP);
                            }
                            else
                            {
                                rate.BluntOn(CIP.TransformDirection(impulse), ptf, nullDmg, CIP);
                            }


                        }
                    }//Same deal for world objects, but only collide with vanilla WO's if in a special attack
                        //otherwise, a regular attack should only affect Physical World Objects and deriving classes.
                    if (!w2 && (wor && MASK_ALL))
                    {
                        MASK_ALL = false;
                        print ("Struck " + c.transform);
                        if (wor.hard)
                        {
                            GetComponent<AudioSource>().PlayOneShot(SoundTable.GetSound(SoundContext, NegContactSound));
                            if (droogy)
                            {
                                paren.FailedWeaponHit(rate.transform.root);
                            }
                            if (interruptable)
                            {
                                parent.AtkFail();
                            }
                            if (ContextBody.GroupOf(c.transform) != context)
                                parent.WeaponCollisionEvent();
                        }
                        else if(wor.invul == false)
                        {
                            GetComponent<AudioSource>().PlayOneShot(SoundTable.GetSound(SoundContext, PosContactSound));
                            if (droogy)
                            {
                                paren.SuccessfulWeaponHit(rate.WhosTheDaddy);
                            }
                            if (!constantImpulse)
                            {
                                wor.Strike(-c.relativeVelocity, ptf);
                            }
                            else
                            {
                               
                                wor.Strike(transform.TransformDirection(impulse), ptf);
                            }
                            Debug.DrawLine(transform.position, transform.position - c.relativeVelocity * power * 2, Color.magenta, 3.0f);
                            
                        }
                    }
                    else if(w2)
                    {
                        if (wor.hard)
                        {
                            GetComponent<AudioSource>().PlayOneShot(SoundTable.GetSound(SoundContext, NegContactSound));
                            if (droogy)
                            {
                                paren.FailedWeaponHit(rate.transform);
                            }
                            if (interruptable)
                            {
                                parent.AtkFail();
                            }
                            if (ContextBody.GroupOf(c.transform) != context)
                                parent.WeaponCollisionEvent();
                        }
                        else if (wor.invul == false)
                        {
                            GetComponent<AudioSource>().PlayOneShot(SoundTable.GetSound(SoundContext, PosContactSound));
                            if (droogy)
                            {
                                paren.SuccessfulWeaponHit(rate.WhosTheDaddy);
                            }
                            if (!constantImpulse)
                            {
                                wor.Strike(-c.relativeVelocity, ptf);
                            }
                            else
                            {

                                wor.Strike(transform.TransformDirection(impulse), ptf);
                            }
                            Debug.DrawLine(transform.position, transform.position - c.relativeVelocity * power * 2, Color.magenta, 3.0f);
                        }
                        
                    }
                    
                }
            }
        }


        
    }

