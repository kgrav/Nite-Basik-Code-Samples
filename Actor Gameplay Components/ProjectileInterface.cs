using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//For enemies which throw physical projectile weapons, (distinct from Missiles, 
//which fire in a straight line and are chiefly kinematic)
//Such as bombs, rocks,
//throwing lnives, and shards of broken glass.
[AddComponentMenu("Nite-Basik/Action Coordination/AI/Projectile Interface")]
    public class ProjectileInterface : AICombatInterface
    {
        public static Vector3 arc; //Cached arc direction
        static bool arcinit = false; //Have we cached the arc direction?
       
        public float CoolDownTime; //Time between throwing a projectile, and being able to throw another projectile
        public GameObject projectile; // Prefab object for the projectile.
        public Vector3 projectilelocation; //Local offset for the projectile.
        public Vector3 launchforce; //Force at whhich to throw the projectile.
        Fist thrower; //Pointer to object in heirarchy which represents the point where the projectile is to be launched from.
        GameObject currproject; //Pointer to currently held projectile
        Animator anim; //
        AICharacter holder;
        int ctxg, ctxi;
        Rigidbody rg;
        bool hasprojectile;
        bool iscool;
        void Start()
        {
            rg = GetComponent<Rigidbody>();
            thrower = GetComponentInChildren<Fist>();
            holder = GetComponent<AICharacter>();
            ctxg = ContextBody.GroupOf(transform);
            ctxi = ContextBody.IndexOf(transform);
            if (!arcinit)
                arc = (new Vector3(0, 2, 3)).normalized; //Height of the arc should be 2/3s the distance.
            hasprojectile = false;
            iscool = true;
            
        }

        public bool HasProjectile()
        {
            return hasprojectile;
        }

        public bool CanSpawn()
        {
            return iscool;
        }

        void CoolDown()
        {
            iscool = true;
            holder.OnWeaponCooldown();
        }
    
    
    
    //On activation:
    //If not within a cooldown period,
    //and not holding a projectile, 
    //Spawn a projectile.
    //If holding a projectile,
    //throw the projectile.

        public override void Activate()
        {
            if (iscool)
            {
                iscool = false;
                Spawn();
            }
            else if (hasprojectile)
            {
                Launch();
            }
        }
        //Calls back to interface when a successful projectile hit has been made.
        public void OnCollisionHit(Transform target)
        {
            holder.SuccessfulWeaponHit(target);
        }
        //called if timer runs out before projectile is otherwise destroyed
        public void OnTimerOut()
        {
            holder.FailedWeaponHit();
        }
        float throwpower;
    //Set the force multiplier for the next projectile throw.
        public void SetThrowPower(float v)
        {
            throwpower = v;
        }
        //In case of emergency, drop projectile.
        public void Override()
        {
            if (hasprojectile)
            {
                hasprojectile = false;
                thrower.DropIt();
            }
        }
        //In case of being un-loaded while holding a projectile
        public void UnLoad()
        {
            if (hasprojectile)
            {
                hasprojectile = false;
                Destroy(currproject);
            }
        }

        //Private method for launching projectiles.  Will not call unless holding a projectile.
        void Launch()
        {
            hasprojectile = false;
            thrower.ThrowIt(throwpower, 1);
            Invoke("CoolDown", CoolDownTime);
        }
        //Private method for spawning projectiles.  Will not be called unless no projectile is held.
        void Spawn()
        {
            
            currproject = (GameObject)GameObject.Instantiate(projectile, transform.position + transform.TransformDirection(projectilelocation), Quaternion.identity);
            thrower.HoldThis(currproject.transform);
            currproject.GetComponent<ProjectileWeapon>().SetSignalReceiver(this);
            hasprojectile = true;
        }
        
    }

