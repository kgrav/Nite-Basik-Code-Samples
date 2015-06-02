using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//Grenades/Bombs/etc. ordinance.
[AddComponentMenu("Nite-Basik/Physical Objects/Projectile Weapon")]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
    public class ProjectileWeapon : MonoBehaviour
    {
        //link to a prefab which MUST contain a Destroyed Object Signal.  This is created when object is destroyed.
        public GameObject onDestroy;
        public float lifetime;
        public float buffertime;
        public float painvalue;
        public float IFEXrad;
        public float IFEXforce;
        float tin = 0;
        public bool destroyOnCollision;
        public bool destroyOnTimer;
        public bool WaitForTimer;
        public AudioClip explososund;
        public bool ExplodeOnDestroy; //Explode when timer runs out/collision with other object?
        public bool Tennis; 
        bool launched;
        int Context;
        Rigidbody rg;
        Collider q;
        void Start()
        {
            tin = 0;
            rg = GetComponent<Rigidbody>();
            q = GetComponent<Collider>();
            q.enabled = false;
            rg.isKinematic = true;
        }

        void Update()
        {
            tin += Time.deltaTime;
            
        }
        ProjectileInterface launchedfrom;
        public void SetSignalReceiver(ProjectileInterface c)
        {
            //Set the projectile interface to call back to on termination (as well as ensuring that 
            //the attached game object is immune to damage from this projectile)
            launchedfrom = c;
            Context = ContextBody.GroupOf(c.transform);
        }
        public void Throw()
        {
            rg.isKinematic = false;
            Invoke("Activate", buffertime);
        }
    //Calls after 'buffer time' to make sure that no collision events are called between
    //the moment when the object is thrown, and the moment when the object actually leaves
    //the source's collision zones.
        public void Activate()
        {
            launched = true;
            q.enabled = true;
            if (destroyOnTimer)
            {
                Invoke("WreckIt", lifetime);

            }
        }
        Transform collidedwith;
    //On collision, damage target, destroy if this projectile destroys on collision
        void OnCollisionEnter(Collision c)
        {
            if (launched)
            {
                collidedwith = c.transform;
                if (launchedfrom)
                {
                    launchedfrom.OnCollisionHit(c.transform);
                }
                MeleeWeapon mw = c.transform.GetComponent<MeleeWeapon>();
                HitZone hz = c.transform.GetComponent<HitZone>();
                WorldObject wz = c.transform.GetComponent<WorldObject>();
                if (Tennis && mw && mw.activity == MWFLAGS.ATK)
                {
                    rg.velocity = -2 * rg.velocity;
                    return;
                }
                if (hz)
                {
                    hz.BluntOn(rg.velocity.normalized, painvalue, false, transform);
                }
                if (wz)
                {
                    wz.Strike(rg.velocity.normalized, painvalue);
                }
                if (tin < buffertime && !WaitForTimer && destroyOnCollision)
                {
                    CancelInvoke("WreckIt");
                    WreckIt();
                }
                else if (tin > buffertime && destroyOnCollision)
                {
                    CancelInvoke("WreckIt");
                    WreckIt();
                }
            }

        }
    //On destroy, call back to projectile interface if one was declared, cause an explosion and spawn this projectile's
    //Destroyed Object Signal.-
        void WreckIt()
        {
            if (destroyOnTimer)
            {
                if (launchedfrom)
                {
                    launchedfrom.OnTimerOut();
                }
            }

            GameObject gw = (GameObject)GameObject.Instantiate(onDestroy, transform.position + 2*Vector3.up, Quaternion.identity);
            gw.GetComponent<DestroyedObjectSignal>().DESTROY();
            GetComponent<AudioSource>().PlayOneShot(explososund);
            if (ExplodeOnDestroy)
                new Explosion(launchedfrom.transform, transform.position, IFEXforce, IFEXrad, painvalue, (1.0f / 3.0f) * IFEXrad, false);
            Destroy(gameObject);
        }
    }

