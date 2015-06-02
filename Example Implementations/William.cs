using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//An AI Character who hops and slithers
//around, throw bombs, and taunts the player.
//
//William represents an experiment in programming a 'personality'
//For an AI Character.
//He will taunt the player if he scores a direct hit
//with his bombs,
//and if he bounces into a wall, he'll shout in pain, fall over and
//right himself. 

//William Utilizes Three major components,
//Coordinated here in this Header script.
//The PodMover supplies his movement (bouncing in a direction, rotating to
//face the goal direction incrementally on each ground hit.
//The Projectile Interface spawns and throws Bombs, and handles
//all events triggered to or from the bombs (some of which call back to AI Character methods).
//the AI Vision Component checks for objects and characters in the field of view
//
//This script coordinates one-shot animations, sound effects, and action across all locally-held
//components.
[RequireComponent(typeof(AIVision))]
[RequireComponent(typeof(ProjectileInterface))]

[AddComponentMenu("Nite-Basik/Header Scripts/William")]
public class William : AICharacter
    {
    public int tgtContext, soundContext;
    public GameObject onDeath;
    public float idealdist, alrightdist, baddist;
    AIVision eyes;
    Animator anim;
    AudioSource snd;
    PodMover pods;
    ProjectileInterface pr;
    int state;
    int ctxg, ctxi = -1;
    public int TauntSound, AlertSound, GulpSound,SpitSound,LaunchSound, HurtSound, DeathSound;
    //Action flags, to coordinate state:
    bool preparingatk = false, execatk = false, taunt = false, target = false, pause = false, tauntq = false;
    //Random signal is called once every few seconds (randomly assigned from a state-dependent bounded range)
    //And is used for additional checks, actions, and transitions that would not work within the Update loop,
    //because it is critical that they only be called once at a time.
    void RandomSignal()
    {
        float ran = UnityEngine.Random.Range(4.0f, 10.0f);
        if (!target)
        {
            //If not targeting, turn to the closest free direction on random signal.
            Vector3 movesig;
            float leftwall = float.PositiveInfinity, rightwall = float.PositiveInfinity;
            bool turnleft = false;
            RaycastHit lhit, rhit;
            if (Physics.Raycast(new Ray(transform.position + transform.right * 1.5f, transform.right), out rhit, 30.0f))
            {
                rightwall = rhit.distance;
            }
            if (Physics.Raycast(new Ray(transform.position - transform.right * 1.5f, -transform.right), out lhit, 30.0f))
            {
                leftwall = lhit.distance;
            }
            if (rightwall < leftwall)
            {
                turnleft = true;
            }
            if (leftwall < 5 && rightwall < 5)
            {
                movesig = -transform.forward;
            }
            else if (turnleft)
            {
                movesig = -transform.right;
            }
            else
            {
                movesig = transform.right;
            }
            pods.SetMotion(movesig);
        }
        else
        { //Otherwise, set a goal location close to the player.
            float currd = Vector3.Distance(transform.position, targ.position);
            if (currd > baddist)
            {
                target = false;
                targ = null;
            }
            else if (currd > idealdist)
            {
                rmove = true;
                Invoke("resetrmove", 1.2f);
                Vector3 currdir = new Vector3(targ.position.x, transform.position.y, targ.position.z) - transform.position;
                currdir = currdir.normalized;
                Vector3 gotwo = Quaternion.Euler(0, 30, 0)*currdir;
                gotwo *= idealdist;
                currdir = gotwo - transform.position;
                ran = UnityEngine.Random.Range(2.0f, 4.0f);
                pods.SetMotion(currdir);
            }
            else if (currd < idealdist)
            {
                rmove = true;
                Invoke("resetrmove", 1.2f);
                pods.RotateToAvoid(targ.position);
                ran = UnityEngine.Random.Range(2.0f, 5.0f);
            }
        }
        Invoke("RandomSignal", ran);
    }
    bool rmove = false;
    void resetrmove()
    {
        rmove = false;
    }
    protected override void InitFunc()
    {
        eyes = GetComponent<AIVision>();
        anim = GetComponent<Animator>();
        snd = GetComponent<AudioSource>();
        ctxg = GetComponent<ContextBody>().GroupID;
        pods = GetComponent<PodMover>();
        pr = GetComponent<ProjectileInterface>();
        RandomSignal();
    }
    //Calls from Mover when it is safe to cut off communications and begin an action
    public override void MoverSafe()
    {
        if (tauntq) //Either taunt, or initialize attack mechanism.
        {
            tauntq = false;
            Invoke("Taunt", 0.4f);
        }
        if (preparingatk && calledattack)
        {
            calledattack = false;
             Vector3 ld = LIZ.position - transform.position;
        ld.y = 0;
        ld = transform.position + ld.normalized;
        transform.LookAt(ld);
            Invoke("AtkV1", 1.5f);
        }
    }
    protected override void Alarm()
    {
        //Play alert sound and move to target state.
        snd.PlayOneShot(SoundTable.GetSound(soundContext, AlertSound));
        target = true;
        targ = ContextBody.FirstInGroup(tgtContext);
    }
    public override void OnWeaponCooldown()
    {
        
    }
    bool dead = false;
    public override void Untangle()
    {
        if (!dead)
        {
            //Death stage 0:
            //Cut off attacks, go to death animation, sound
            //death cry, initialize death mechanism.
            dead = true;
            CancelAttack();
            taunt = true;
            pods.enabled = false;
            anim.SetTrigger("Kill");
            snd.PlayOneShot(SoundTable.GetSound(soundContext, DeathSound));
            Invoke("DieV0", 2.0f);
        }
    }
    void DieV0()
    {
        //Death Stage 1 -
        //2 seconds after stage 0,
        //create corpse object
        GameObject dmask = (GameObject)GameObject.Instantiate(onDeath);
        dmask.transform.position = transform.position + Vector3.up;
        dmask.GetComponent<DestroyedObjectSignal>().DESTROY();
        Invoke("DieV1", 2);
    }
    void DieV1()
    {
        //Death Stage 2 -
        //destroy this instance of William.
        Destroy(transform.gameObject);
    }
    //Different segments of the Attack 'Envelope'
    bool calledattack;
    void AtkV0()
    {
        //STAGE 0 - stop mover (attack init is called from the "MoverSafe" function,
        //which calls whenever the PodMover has landed.
        pods.Pause();
    }
    void AtkV1()
    {
        //STAGE 1 - double check to make sure that mover is paused.
        //Look at player position.
        //Begin animation,
        //play Bomb Retrieval sound.
        //
        Vector3 ld = LIZ.position - transform.position;
        ld.y = 0;
        ld = transform.position + ld.normalized;
        transform.LookAt(ld);
        print("Stage 1");
        pods.Pause();
        Debug.DrawRay(transform.position, ld.normalized*10, Color.black, 4.0f);
        print(LIZ);
        anim.SetTrigger("Bomb");
        float rate = pods.ForceToDistanceRatio();
        if (targ)
        {
            float ds = Vector3.Distance(transform.position, targ.position);
            pr.SetThrowPower(rate * ds);
            snd.PlayOneShot(SoundTable.GetSound(soundContext, GulpSound));
            Invoke("AtkV2", 1.9f);
        }
        else
        {
            CancelAttack();
        }
        //AtkV2 is called after the approximate amount of time 
        //between this method call, and the part of the bomb retrieval animation
        //at which the bomb is actually spawned.
    }
    void AtkV2()
    {
        //STAGE 2 -
        //When called, signal Projectile Interface to spawn bomb,
        //and play bomb spawn sound.
        Vector3 ld = LIZ.position - transform.position;
        ld.y = 0;
        ld = transform.position + ld.normalized;
        snd.PlayOneShot(SoundTable.GetSound(soundContext, SpitSound));
        transform.LookAt(ld);
        print("stage 2");
        pr.Activate();
        execatk = true;
        preparingatk = false;
        Invoke("AtkV3", 1.2f);
    }
    void AtkV3()
    {
        //STAGE 3 -
        //Launch spawned bomb, return to idle in .5 seconds.
        Vector3 ld = LIZ.position - transform.position;
        ld.y = 0;
        ld = transform.position + ld.normalized;
        transform.LookAt(ld);
        print("stage 3");
        anim.SetTrigger("Throw");
        pr.Activate();
        Invoke("AtkV4", 0.5f);

    }

    void AtkV4()
    {
        //Stage 4 -
        //return to idle, keep target.
        Vector3 ld = LIZ.position - transform.position;
        ld.y = 0;
        ld = transform.position + ld.normalized;
        transform.LookAt(ld);
        print("stage 4");
        execatk = false;
        pods.UnPause();
        
    }
    //On cancel, stop all attack functions, reset all flags,
    //and drop projectile.
    void CancelAttack()
    {
        CancelInvoke("AtkV0");
        CancelInvoke("AtkV1");
        CancelInvoke("AtkV2");
        CancelInvoke("AtkV3");
        CancelInvoke("AtkV4");
        calledattack = false;
        preparingatk = false;
        execatk = false;
        pods.UnPause();
        pr.Override();
    }
    //If receive an attack, Cancel attack and apply force to mover.
    public override void ReceiveAttack(Vector3 attackdir, float force)
    {
        print("Got Hit");
        pods.HitWithAtk(attackdir, force);
        CancelAttack();
        LIZ = Liz.StaticLiz.transform;
        Alarm();
    }
    //If successfully hit a target, Taunt on ground hit.
    public override void SuccessfulWeaponHit(Transform target)
    {
        if (ContextBody.GroupOf(target) == tgtContext)
        {
            pods.Pause();
            CancelAttack();
            tauntq = true;

        }
    }
    public override void FailedWeaponHit()
    {

    }
    public override void FailedWeaponHit(Transform source)
    {
    }
    Transform targ, avd, LIZ;
    protected override void UpdateFunc()
    {
        if (ctxi == -1)
        {
            ctxi = ContextBody.IndexOf(transform);
        }
        //If targeting the player character.
        if (target)
        {
            //If not in an action, or a cooldown period,
            if (!preparingatk && !execatk && !calledattack && !taunt &&
                pr.CanSpawn())
            {
                //Enforce target and initialize attack
                targ = LIZ;
                preparingatk = true;
                calledattack = true;
                float randomt = UnityEngine.Random.Range(4.0f, 6.0f);
                print("attack in " + randomt);
                Invoke("AtkV0", randomt);
            }
            if (!targ)
            {
                //If target is lost, leave target state.
                target = false;
                print("No Targ");
            }
            else if (!rmove)
            { pods.SetMotion((targ.position - transform.position).normalized); }
            //If not in a random override move, approach target.
        }
        else if (!(preparingatk || execatk || taunt)) //If not targeting, and not in an action,
        {
            bool sawfriend = false;
            bool sawenemy = false;
            //Check fields of vision for AI Characters and Player Character,
            //William does not need to avoid walls,
            //He just bounces off of them.
            Vector3 wherefr = Vector3.zero;
            RaycastHit rt = eyes.CenterRay();
            if (XRay.ValidHit(rt)) {
                int x = ContextBody.GroupOf(rt.transform);
                if (x == tgtContext)
                {    sawenemy = true;
                LIZ = rt.transform;
                    targ = rt.transform;
                }
                else if (rt.transform.GetComponent<AICharacter>() && !ContextBody.Same(ctxg, ctxi, rt.transform))
                {   if(!avd || !ContextBody.Same(avd, rt.transform)){ 
                    sawfriend = true;
                    wherefr = rt.transform.position;
                    avd = rt.transform;}
                }
            }
            RaycastHit[] ps = eyes.Periphery();
            foreach (RaycastHit r in ps)
            {
                if (XRay.ValidHit(r))
                {
                    int z = ContextBody.GroupOf(rt.transform);
                    if (z == tgtContext)
                    {
                        targ = r.transform;
                        sawenemy = true;
                    }
                }
            }
            if (sawfriend)
            {
                pods.RotateToAvoid(wherefr);
            }
            if (sawenemy)
            {//If we saw the target, play Alert sound and set Target state.
                snd.PlayOneShot(SoundTable.GetSound(soundContext, AlertSound));
                target = true;
            }
            
        }
        
    }
    void Taunt()
    {
        //Taunt: stop everything and laugh at whatever we just injured.
        //On the second invocation, turn off and resume normal functionality.
        if (!taunt && !execatk && !preparingatk)
        {
            CancelAttack();
            pods.Pause();
            Vector3 look = new Vector3(targ.position.x, transform.position.y, targ.position.z);
            transform.LookAt(look);
            snd.loop = true;
            snd.PlayOneShot(SoundTable.GetSound(soundContext, TauntSound));
            anim.SetBool("Taunt", true);
            taunt = true;
            Invoke("Taunt",1.2f);
        }
        else if (taunt && !dead)
        {
            snd.loop = false;
            anim.SetBool("Taunt", false);
            taunt = false;
            preparingatk = false;
            execatk = false;
            tauntq = false;
            pods.UnPause();
        }
    }
    
    }

