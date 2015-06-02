using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
public enum HMDB {IDLEROT, IDLEFOR, APPRGOAL, ATKPRIME, BARAVD, FRNDAVD }
/*Basic enemy, approaches player and uses Melee attacks,
 * Moves along ground and does not jump,
 * If attacked, will call to friends for help.
 * If approach player from behind, will perform a strong attack.
 * If see wwall, correcting motion is the highest priority.
 * 
 * Will wander randomly if player isn't around.
 * These guys are guard dogs.
 * */
[AddComponentMenu("Nite-Basik/Header Scripts/Homunculus")]
[RequireComponent(typeof(AnimatorTranceiver))]
[RequireComponent(typeof(AIVision))]
[RequireComponent(typeof(AILegs))]
public class Homunculus : AICharacter
{

    public static int Homunculi = 0;
    public static int Courage = 1;
    public int soundContext;
    public HMDB DEBUGFLAG;
    public Transform LookingAt;
    public GameObject prefablink;
    //Death Mask: Destroyed Object Signal to leave upon death.
    public GameObject deathMask;
    public float atkanimlength; //Absolute Cool down time for attack.
    public float MeanAttackDelay; // mean cool down time for attack
    public float StopDistance; //distance where it's ok to stop and attack a target
    public float Loudness; //radius of alarm cast
    public Vector3[] directions; 
    public Vector2 atkrandomrange; //Random number used for attack timing will be >x and <y
    
    public bool georange;
    public int noticesnd, diesnd, atksnd, boredsnd; //Cabinet opcodes
    static bool init = false; //static variables initialized
    static GameObject homuncutemp;
    float rangle = 0; //Random angle (for rotation)
    RandomFlag announce; //Will the creature make a sound effect before it attacks?
    public int context, primecontext, barriercontext, MaxBarrierDistance, AtkDistance, fmod; //Contextx and distances.
    bool alerted; //is this Homunculus alert (targeting the player)?
   //Component reference variables:
    Animator m;
    AILegs legs;
    AIVision eyes;
    CombatInterface cb; Renderer gfx;
    Collider[] hitboxes;
    AudioSource vce;
    int contextOf;
    bool targetPrime;
    bool bubs = false;
    bool inattack = false;
    int idlestate;
    Vector3 goal;
    bool idle, attacking, follow, blockmotion, pause;
    bool attack = false;
    public int state = -1;
    public string nextatk = "ATKARM";
    protected override void InitFunc()
    {
        frame = 0;
        contextOf = ContextBody.GroupOf(transform); //cache own Context to improve comparison performance
        Homunculi++;
        localCourage = Courage;
        m = GetComponent<Animator>();
        legs = GetComponent<AILegs>();
        eyes = GetComponent<AIVision>();
        cb = GetComponent<CombatInterface>();
        vce = GetComponent<AudioSource>();
        gfx = GetComponent<Renderer>();
        List<Collider> tcl = new List<Collider>();
        foreach (Collider f in GetComponentsInChildren<Collider>())
        { tcl.Add(f); }
        foreach (Collider g in GetComponents<Collider>())
            tcl.Add(g);
        hitboxes = tcl.ToArray();
        if (!gfx)
            gfx = GetComponentInChildren<Renderer>();
        announce = new RandomFlag(10.0f, .30f);
        

        RandomSignal();
    }
    int currgoal;
    protected override void DeactivateFunc()
    {
        targetPrime = false;
        state = 0;
        eyes.enabled = false;
        legs.STOP = true;
        legs.enabled = false;
        gfx.enabled = false;
        foreach (Collider c in hitboxes)
        {
            c.enabled = false;
        }
        pause = true;
    }

    protected override void ActivateFunc()
    {
        state = 0;
        eyes.enabled = true;
        legs.STOP = false;
        legs.enabled = true;
        gfx.enabled = true;
        foreach (Collider q in hitboxes)
        {
            q.enabled = true;
        }
        pause = false;
    }
    void UnTargetPrime()
    {
        targetPrime = false;
        if (state == 2)
            state = 0;
    }
    Transform prime;
    //Prime Context refers to Player Character Context (0 by default)
    //states:
    //RandomSignal serves as the state checking function for the homunculus
    //Decides states and actions which can be considered Passive 
    //(i.e. not dependent on immediate sensory input), and ensures
    //that the AI never breaks kayfabe.
    //0 - Idle forward/Rotate on random signal
    //If see wall (anytime) -> GOTO 1
    //If see prime (anytime)-> GOTO 2
    //1 - Avoid wall/Reset check on random signal
    //   Substate 0 IF NOT corrected yet
    //   Substate 1 IF correction has been set
    //   Only leave state 1 when no wall exists within distance
    //2 - See prime/Choose behaviour on next signal
    //   Substate 0 if APPROACHING (distance > attack distance)
    //   Substate 1 if ATTACKING
    //   Substate 2 if REPOSITIONING (high times hit/successful attacks ratio)
    //   On random signal: check ray (all) from position to known prime position,
    //   If a wall lies between prime & homunculus > return to idle
    //   else keep going
    //3 - See other Homunculus/Avoid on random signal
    //4 - preparing attack.
    //5 - Executing attack, return to Target state on random signal if weapon is inactive.
    int substate = -1; //Inter-phase state 
    int s1mem = 0; //store previous state (so that the appropriate state may be returned to following an avoid state)
    Vector3 rmem;
    void RandomSignal()
    {
        nextatk = "ATKARM";
        float randomsignal = UnityEngine.Random.Range(4.0f, 10.0f);
        if(!pause){
        switch (state)
        {
            case 0:
                rangle = UnityEngine.Random.Range(-1.0f, 1.0f);
                legs.SetDirection(Quaternion.Euler(new Vector3(0, 30 * rangle, 0)) * (transform.forward * fmod));
                rmem = Quaternion.Euler(new Vector3(0, 30 * rangle, 0)) * (transform.forward * fmod);
                break;
            case 1:
                //If we're still in front of the wall on random signal, re-adjust.
                //Otherwise, return to previous state.
                if (substate == 1){
                    RaycastHit rhhh;
                    if (Physics.Raycast(new Ray(transform.position + fmod * eyes.eyesize * transform.forward, fmod * transform.forward), out rhhh, MaxBarrierDistance + 1.0f))
                    {
                        if (rhhh.transform.GetComponent<WorldObject>())
                        {
                            substate = 0;
                        }
                        else
                        {
                            state = 0;
                        }
                    }
                    else
                    {
                        state = s1mem;
                    }

                }
                else randomsignal = 0.5f;//check back a little sooner than usual;
                break;
            case 2:
               //If targeting the player, on random signal, check to see if there's too many
                //objects between Homunculus and player.
                RaycastHit[] r = Physics.RaycastAll(new Ray(transform.position, (LookingAt.position - transform.position).normalized), Vector3.Distance(LookingAt.position, transform.position));
                bool foundp = true;    int friendsbetweenus = 0;
                foreach(RaycastHit rch in r)
                    {
                    friendsbetweenus++;
                    if(ContextBody.GroupOf(rch.transform) == -2 || friendsbetweenus > 5)
                        foundp = false;
                    }
                if(!foundp){
                    state = 0;
                vce.PlayOneShot(SoundTable.GetSound(soundContext, boredsnd));}
                else //If we did find the player, and we've approached them from behind, attack with a headbutt (stronger than arm spiked). 
                {
                    if(Vector3.Angle(transform.forward*fmod, -LookingAt.transform.forward) < 20)
                        nextatk = "ATKHEAD";
                }
                break;
            case 3: //If in avoid Homunculus state and saw a Homunculus, return to avoid signal
                
                if(substate == 1 && Vector3.Angle(transform.forward*fmod, (hommem.position-transform.position).normalized) < 20)
                    substate = 0;
                else
                    state = s1mem;
                break;
            case 4:
                randomsignal = .5f;
                break;
            case 5:
                if (!cb.busy)
                {
                    state = 2;
                    attack = false;
                }
                else
                    randomsignal = 0.5f;
                break;
        }}
        Invoke("RandomSignal", randomsignal);
    }
    Transform hommem;
    public void Touchdown()
    {
        poshits++; 
    }
    int frame;
    protected override void UpdateFunc()
    {
        bool move = false;
        bool rotate = false;
        bool sawPrime = false;
        bool sawBarrier = false;
        bool sawHomunculus = false;
        int distancequadrant = -1;
        
        Transform saw;
        Vector3 tomover = fmod*transform.forward;
        float dist;
        //On update, check vision.  
        //The object in the primary field of view,
        //as well as the distance of that object,
        //will come into play when we're computing the next state.
        if (XRay.ValidHit(eyes.CenterRay()))
        {
            saw = eyes.CenterRay().transform;
            dist = eyes.CenterRay().distance;
            int ctx = ContextBody.GroupOf(saw);
            if (ctx == contextOf)
            {
                sawHomunculus = true;
                hommem = saw;
            }
            if (ctx == primecontext)
            {
                sawPrime = true;
                prime = saw;
            }
            if (ctx == barriercontext)
                sawBarrier = true;
            if (dist < StopDistance)
                distancequadrant = 0;
            else if (dist < AtkDistance)
                distancequadrant = 1;
            else if (dist < MaxBarrierDistance)
                distancequadrant = 2;
            else distancequadrant = 3;
            
        }
        //If not avoiding scenery (state 1) or targeting player (state 2), and
        //saw a sufficiently close piece of scenery,
        //Move to avoid scenery.
        if (state != 1 && state != 2  &&(sawBarrier && distancequadrant < 3))
        {
            s1mem = state;
            state = 1;
            substate = 0;
        }
        //If not avoiding or targeting,
        //and saw the player character (Prime Context), 
        //target the player character.  
        //Note - "announce" is a random boolean
        //which, in this class, has a 1/3 chance of returning true.
        if (state != 2 && sawPrime)
        {
            if (announce.Sample())
                SoundAlarm();
            Alarm();
        }
        //If we're not already avoiding it, and we saw a sufficiently close
        //Homunculus, move to avoid Homunculus state.
        if (state != 3 && sawHomunculus && distancequadrant < 2)
        {
            s1mem = state;
            state = 3;
            substate = 0;
        }

        //Once we've decided on state (or a previous state took precedence over
        //possible new states), act on current state.

        //Avoid states are exited (or re-entered) on the random signal.
        if (state == 1)
        {
            //If in state 1, decrease movement speed to anticipate sharp turn.
            legs.SetSpeed(SPDFLAGS.CAUTION);
            move = true;
            if (substate == 0)
            {
                //Substate 0 if we haven't corrected for the wall.
                tomover = Quaternion.Euler(0, 100, 0) * tomover;
                rmem = tomover;
                substate = 1;
            }
            else
            {
                //Otherwise, write corrected value to mover signal.
                tomover = rmem;
            }
        }
        else if (state == 0)
        {
            //If we're in the Idle state,
            //Move at full speed forward.
            //(directional adjustments made in Random signal
            //and stored to rmem)
            legs.SetSpeed(SPDFLAGS.FULL);
            if (!tomover.Equals(rmem))
                tomover = rmem; 
            move = true;
        }
        else if (state == 3)
        {
            //If we;re avoiding a fellow Homunculus,
            //rotate slightly in order to avoid if in substate 0.
            //(this state will always start in subs 0, and
            //will reset to subs. 0 if additional correction is needed on random signal.
            legs.SetSpeed(SPDFLAGS.CAUTION);
            if (substate == 0)
            {
                legs.RotateToAvoid(hommem.position);
                substate = 1;
            }


        }
        else if (state == 2)
        {
            //If we're targeting the player, always face.
            tomover = prime.position - transform.position;
            legs.SetSpeed(SPDFLAGS.FULL);
            if (Vector3.Distance(transform.position, prime.position) > AtkDistance)
            {
                //If we're not in attack range,
                //approach the target
                move = true;
                legs.SetSpeed(SPDFLAGS.FULL);
                print(Vector3.Distance(transform.position, prime.position));
                Vector3 ui = (transform.position- prime.position).normalized;
                ui = AtkDistance *   ui + prime.position;
                tomover = (transform.position - ui).normalized;
            }
            else if (!attack && Vector3.Distance(transform.position, prime.position) < AtkDistance)
            {
                //Otherwise, initiate attack
                attack = true;
                state = 4;
                Invoke("StartAttack", 0.1f);
            }
        }
        else if (state == 5 || state == 4)
        {
            //If executing or anticipating an attack, do not move.
            move = false;

            if (state == 5)
                transform.Rotate(Vector3.up, 25 * Time.deltaTime, Space.World);
        }
        if (move)
        {
            m.SetFloat("fmod", 1);

        }
        else
        {
            m.SetFloat("fmod", 0);
        }
        if (move)
        {
            legs.STOP = false;
            legs.SetBehaviour(move, true, "udl");
            legs.SetDirection(tomover);
        }
        else
        {
            
            legs.STOP = true;
            Vector3 primeee = transform.forward;
            if(prime)
            primeee = prime.position - transform.position;
            primeee.y = 0;
            primeee = transform.position-primeee.normalized;
            if (targetPrime)
                transform.LookAt(primeee);
        }
        frame++;
    }
    bool dead = false;
    public override void Untangle()
    {
        //On death, start animation and deactivate.
        if (!dead)
        {
            dead = true;
            m.SetInteger("sig", 1);
            m.SetTrigger("OR");
            Invoke("Preburn", 0.2f);
            SetActive(false);
        }
    }

    void StartAttack()
    {//Make a sound and initiate attack,
        vce.PlayOneShot(SoundTable.GetSound(soundContext, atksnd));
        Invoke("AtkV1", 1.2f);
    }
    
    void AtkV1()
    {//Jump to attack state,
        //start animation,
        //and turn on weapons.
        state = 5;
        m.SetTrigger(nextatk);
        if(nextatk.Equals("ATKHEAD"))
            Invoke("EndAtk", 1.0f);
        else
            Invoke("AtkV2", .7f);
        cb.OBE();
    }
    void AtkV2()
    {
        m.SetTrigger(nextatk);
        cb.OBE();
        Invoke("EndAtk", 1.0f);
    }

    public override void ReceiveAttack(Vector3 attackdir, float force)
    {
        //Cut off attack, target player,
        //1/3 chance of alerting nearby AI Characters.
        neghits++;
        CancelInvoke("StartAttack");
        legs.enabled = true;
        legs.HitWithAtk(attackdir, force);
        attack = false;
        TimesHitSinceLastAtk++;
        if (announce.Sample())
            SoundAlarm();
        Alarm();
    }

    //Alert nearby Homunculus Instances (William doesn't care if a Homunculus gets hurt.)
    public void SoundAlarm()
    {
        Collider[] buds = Physics.OverlapSphere(transform.position, Loudness);
        for (int i = 0; i < buds.Length; ++i)
        {
            Homunculus monky = buds[i].GetComponent<Homunculus>();
           
            if (monky && !ContextBody.Same(monky.transform, transform))
                monky.Alarm();
        }
    }
    void NoViolence()
    {
        attack = false;
    }
    int TimesHitSinceLastAtk = 0;
    //Target player on alarm.
    //TO-DO:
    //Add targeted alarms (dependent on the object/creature who sounded it)
    protected override void Alarm()
    {
        vce.PlayOneShot(SoundTable.GetSound(soundContext, noticesnd));
        LookingAt = ContextBody.FirstInGroup(primecontext);
        prime = LookingAt;
        targetPrime = true;
        state = 2;
    }
    int localCourage;
    //If scared, run away.
    private void Scare()
    {
        localCourage--;
        if (localCourage < 0)
        {
            legs.SetDirection(transform.forward);
            legs.SetSpeed(SPDFLAGS.FULL);
            legs.SetBehaviour(true, true, "FLEE");
            CancelInvoke("RandomSignal");
            Invoke("RandomSignal", 4.0f);
            localCourage = Courage;
            state = 0;
        }
    }
    void EndAtk()
    {
        attack = false;

    }
    
    int turn = 0;
    //Pre-death function.
    void Preburn()
    {
        GameObject dma = (GameObject)GameObject.Instantiate(deathMask, transform.position + Vector3.down, Quaternion.identity);
        dma.GetComponent<DestroyedObjectSignal>().DESTROY();
         if ((Infrared.PlayerTarget != null) && (Infrared.PlayerTarget.transform.name.Equals(transform.name)))
            Infrared.PlayerTarget = null;
        legs.STOP = true;
        vce.PlayOneShot(SoundTable.GetSound(soundContext,diesnd));
        Invoke("Burn", 3);
    }
    //Death function.
            void Burn()
            {
                --Homunculi;
                Destroy(gameObject);
            }
    
}

