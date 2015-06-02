using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//A fully-featured movement script, intended for a character with flight and jumping
//capabilities,the ability to perform a shprt dash in a given direction, move with respect
//to a targeted object, and be affected by physical forces. 
//
[AddComponentMenu("Nite-Basik/Action Coordination/MAIN ONLY/Directly Controlled Mover")]
[RequireComponent(typeof(CharacterController))]
public class DirectlyControlledMover : BaseMover
{
    CharacterController motor;
    Transform trans, focus, movewith;
    TargetArrow arrow;
    Vector3 motion, input, forwarddir, floornorm;
    public Vector3 prevtrans;
    Transformation lockon;
    public Vector3 offset;
    public float vmod, landspeed;
    public float jumpspeed, jumpdrag, jmod;
    /*
     jumpspeed: the initial speed at which the character leaves the ground
     * jumpdrag: the rate at which the character accellerates towards the ground.
     * jmod: the rate at which the initial jump speed deteriorates.
     
     */
    public float Fturnv, Fpitchv;
    float jsi, jhi, vmem;
    public bool analog;
    public float analogerror;
    public int context;
    public float accelmod;
    public float flightspeed;
    float spdmod;
    public bool freezespdmod;
    bool lockinput;
    int unjumpbuffer = 0;
    int rockets = 0;
    /*-1/0 - no rockets.
     * 1 - begin rockets
     * 2 - Freeform rockets (no target)
     * 3 - Homing Rockets (has target)
     * */
    Vector3 Trajectory;
    public int jumpstate;
    /*-1: On Ground
     * 0: Begin Jump
     * 1: Rise
     * 2: Begin Falling
     * 3: Dubs tap
     * 4: Dubs Success
     * 5: Dubs rise
     * 6: Dubs fall
     * */
    int spstate;
    /*-1: none
     * 0: Leaf Spin OR (Lock y)
     */

    float zeropitchref; //Euler angle (x axis) that represents pitch parallel to the ground.
    public bool isdodging = false; //Are we in the progress of a Dodge Flash?
    public float dodgemod = 1.0f; //How wide is the dodge flash? (dynamic, sets relative to target)
    public float dodgeangle = 90;
    public float dodgerad;
    public float dodgespeed;
    public Vector3 dodgeReference;
    public Vector3 displace;
    public Vector3 deuler;
    public Quaternion Quat;
    public int dodgedir;
    public float DRAGFACTOR;
    public float factor, factori;


    float floor, vdisplace;

    bool ground; //Are we on the ground?
    bool inited; //Have we set up important variables?
    bool col; //Have we collided recently?
    bool OR; //Do we override movement in some way?
    Vector3 impulsedir; //What direction was the force applied to the player in

    float xang, yang;
    public int GetRockets()
    {
        return rockets;
    }
    //returns whether or not the directly controlled character is moving.
    public override bool Motion()
    {
        return motion.magnitude > 0.1f;
    }
    //Advanced isGrounded function; Unity's stock one is very unreliable.
    void CheckFloor()
    {

        motor = GetComponent<CharacterController>();
        RaycastHit rc;
        //if at the beginning of a jump, immeidately return false.
        if (jumpstate == 0 || jumpstate == 1 || jumpstate == 4)
        {  ground = false; return; }
        if(motor.isGrounded)//On the off-chance that Unity's ground check worked, cut the method off before the fine checks.
        {
            ground = true;
            jumpstate = -1;
            return;
        }//If not, perform a raycast to double-check/.  If the player is logically on the ground,
        //but physically not on the ground, the player will be transferred to jump state 2 (falling)
        if (Physics.Raycast(new Ray(transform.position + new Vector3(0, -offset.y, 0), Vector3.down), out rc, 0.5f))
        {
            if (jumpstate == 3)//If we hit the ground while the character was in Jump State 3 (briefly entered by tapping 'a' while in Jump State 2),
            {//proceed to Jump State 4 (a second, higher jump - think Mario 64)
                jumpstate = 4;
                ground = false;
                return;
            }
            if (rc.collider.GetComponent<MovingPlatform>() != null)
            {
                movewith = rc.transform;
                jumpstate = -1;
                floornorm = rc.normal;
                return;
            }
            if (rc.collider.GetComponent<WorldObject>() != null)
            {

                ground = true;
                floor = rc.collider.transform.position.y;
                floornorm = rc.normal;
                jumpstate = -1;
                vdisplace = 0;
                return;
            }
        }
            ground = false;
            vdisplace = transform.position.y - floor;
            if (jumpstate == -1)
                jumpstate = 2;
    }
    float ymem;
    //Used to set Y Lock Override (Spin move in the example)
    public void SetSpState(int i)
    {
        if (i == 0)
            ymem = transform.position.y;
        spstate = i;
    }
    //Set the player's Target
    public void ZTarget(Transform t)
    {
        focus = t;
        TargetArrow.PlayersArrow.turnedon = true;
    }

    //Release the player's target.
    public void ReleaseTarget()
    {
        ROCKETS(-1);
        focus = null;
        TargetArrow.PlayersArrow.turnedon = false;
    }

    //Ensure that the player is facing the right direction.
    void SetZ()
    {
        //If overridden or no target, face the previous move direction
        if (focus == null || freezespdmod)
        {
            transform.LookAt(transform.position - prevtrans, Vector3.up);

        }
        else if (avoid != Vector3.zero)//If avoiding, smooth look direction
        {
            transform.LookAt(transform.position - look, Vector3.up);
        }
        else //Otherwise, look at focus
        {
            
                        transform.LookAt(transform.position - new Vector3(focus.position.x - transform.position.x, 0, focus.position.z - transform.position.z), Vector3.up);
        }
    }
    public override Vector3 LocalForward() 
    {
        return -transform.forward;
    }

    public bool IsORed()
    {
        return lockinput;
    }

    public override void Recoil(Vector3 dir, float amnt)
    {
        SetInputLock(prevtrans * amnt, 0.2f);
    }
    bool impulse;
    float impulsepow;
    float dec;
    Vector3 impulsenorm;
    //Calling this method only applies force to the character controller-based object.
    //To do damage to a character, go through the Mortality script.
    //This is so this method can also be used for springboards, bumpers, and other
    //mechanical constructs.
    public override void HitWithAtk(Vector3 dir, float amnt)
    {
        isdodging = false;
        impulse = true;
        impulsenorm = dir.normalized;
        dec = amnt / 10;
        impulsepow = dec;
    }
    public void Dodge(int dir)
    {
        if(focus != null && !isdodging){
        isdodging = true;
        dodgedir = dir;
        factor = 90 / 0.5f;
        dodgeReference = focus.position;
        GetComponent<Animator>().SetTrigger("GroundDash");
        dodgemod = 0.5f;
        dodgeangle = 90*dodgedir;
        }
    }
    public void ResolveDodge()
    {
        if (col || dodgemod < 0 || factor < 0.1)
        {
            isdodging = false;
            print("Ground Dash Canceled " + col + " , " + dodgemod + " , " + factor);
            GetComponent<Animator>().SetTrigger("GroundDash");
            return;
        }
        
        deuler = new Vector3(0, dodgeangle/10, 0);
        Quat = Quaternion.Euler(deuler);
        displace = dodgeReference - transform.position;
        Vector3 newDisplace = displace - transform.position;
        motor.Move(newDisplace);
        dodgemod -= Time.deltaTime;
        factori = dodgemod * factor;
        dodgeangle = factori * Time.deltaTime;
    }
    public void ROCKETS(int rockstate)
    {
        rockets = rockstate;
        if (rockstate == -1)//If we're turning Rockets off, make sure we turn them all the way off
            UnRocket();
    }
    //Lunges the player forward at a constant speed, for a specified time (t)
    public void ForwardDash(float t)
    {
        SetInputLock(prevtrans, t);
        LockAccel(2.5f, t, true);
    }
    //Like forward dash, but this may be performed at any speed in any direction.
    public void AimJolt(float t, Vector3 dir, float spd)
    {
        transform.LookAt(transform.position + spd * dir);
        SetInputLock(prevtrans, t);
        LockAccel(spd, t, true);
    }

    public override bool OnGround()//For external decision making; if not flying, jumping, or falling, then
        //the character is logically on the ground.
    {
        return (rockets == -1) && (jumpstate == -1);
    }
    CharacterController mov;
    void Start()
    {
        mov = GetComponent<CharacterController>();
        Invoke("FinAvoid", 0.1f);
        lockon = null;
        focus = null;
        spdmod = 1;
        freezespdmod = false;
        jumpstate = -1;
        rockets = -1;
        zeropitchref = transform.eulerAngles.x;
        spstate = -1;
        CheckFloor();
        vmem = transform.position.y;
        input = Vector3.zero;
        forwarddir = vmod * transform.forward;
        movewith = null;
        jsi = 0;
        jhi = 0;
        Trajectory = Vector3.zero;
        impulsedir = Vector3.zero;
        inited = true;
    }
    public override void PAUSE()
    {
        pause = true;
    }
    public override void UNPAUSE()
    {
        pause = false;
    }

    public int GetJS()
    {
        return jumpstate;
    }
    //if not jumping or falling, execute a jump.
    public void JumpTap()
    {
        if (jumpstate == -1)
        {
            jumpstate = 0;
            unjumpbuffer = 0;
        }
        else if (jumpstate == 2 || jumpstate == 1)
        {
            unjumpbuffer = 0;
            jumpstate = 3;
            Invoke("TurnOffJumpTap", 0.5f);
        }
    }
    //Cut off a jump in progress (switch early to fall state)
    public void ReleaseJump()
    {
            if (jumpstate == 1)
                jumpstate = 2;
            else if (jumpstate == 5)
                jumpstate = 6;
    }
    float flooroffs;
    public void TurnOffJumpTap()//Turns off Second Jump, a couple tenths of a second after pressing Jump
    {
        if (jumpstate == 3)
            jumpstate = 2;
    }
    float osmod = 1;
    public void AdjustRate(float o)//Adjust movement rate externally.
    {
        osmod = o;
    }
    public void ResetRate()//reset movement rate
    {
        osmod = 1;
    }
    public void LockAccel(float f, bool q) //Lock all accelleration
    {
        spdmod = f;
        freezespdmod = true;
    }
    public void LockAccel(float f, float t, bool q) //Lock all accelleration for an amount of time.
    {
        spdmod = f;
        freezespdmod = true;
        CancelInvoke("UnlockAccel");
        Invoke("UnlockAccel", t);
    }

    void UnlockInput()
    {
        lockinput = false;
    }

    public void SetInputLock(bool b)
    {
        lockinput = b;

    }

    public void SetInputLock(Vector3 v, float howlong)
    {
        lockinput = true;
        input = v;
        CancelInvoke("UnlockInput");
        Invoke("UnlockInput", howlong);
    }
    Vector3 look;
    public void SetInput(Vector3 v, float f)
    {
        if (!lockinput)
        {
            look = v.normalized;
            //if avoiding a moving object
            /*if (avoid!= Vector3.zero)
            {
                //bind the movement so that the object cannot be physically collided with
                avoid.y = transform.position.y;
                float avx = avoid.x - transform.position.x;
                float avz = avoid.z - transform.position.z;
                float ix = v.x-avx;
                float iz = v.z - avz;
                float ixf = Mathf.Clamp(v.x, Math.Min(input.x, avx), Math.Max(input.x, avx));
                float ixz = Mathf.Clamp(v.z, Math.Min(input.z, avz), Math.Max(input.z, avz));
                Vector3 am = (transform.position - avoid).normalized;
                v -= am;
            }*/
            input.x = v.normalized.x * landspeed * f;
            input.z = v.normalized.z * landspeed * f;

            if (v.Equals(Vector3.zero) || f < 0.1f)
            {
                input = Vector3.zero;
            }
            else if (rockets != 2)
            {
                prevtrans = input;
            }
        }
    }

    public void ORMotion(Transformation t)
    {
        if(!lockinput)
        {lockinput = true;
        lockon = t;
        OR = true;
        }
    }

    public void UnlockAccel()
    {
        freezespdmod = false;
    }
    Vector3 avoid;
    void FinAvoid()
    {
        avoid = Vector3.zero;
    }
    void OnCollisionEnter(Collision c)
    {
        if (ContextBody.GroupOf(c.transform) != context)
        {
            if (Vector3.Angle(c.contacts[0].normal, Vector3.up) > 20)
            {
                WorldObject w = c.transform.GetComponent<WorldObject>();
                if (w)
                {
                    if (w.floor)
                    {
                        flooroffs = transform.position.y - c.contacts[0].point.y + 0.4f;
                        ymem = c.contacts[0].point.y + flooroffs;
                    }//Bump the Y memory if we're spinning while moving up a ramp.
                }
                col = true;
            }
            else if (Vector3.Angle(c.contacts[0].normal, Vector3.up) > 60 && Vector3.Angle(c.contacts[0].normal, Vector3.up) < 120)
            {
                avoid = c.contacts[0].normal*10;
                Invoke("FinAvoid", .2f);
            }
            if (jumpstate == 3 && c.gameObject.GetComponent<WorldObject>() != null)
            { jumpstate = 0; SetInputLock(c.contacts[0].normal, 1); }
            else if (rockets == 3)//If we're target-flying, exit flight on collision.
            {
                //UnRocket();
            }
            else if (rockets == 2)//If we're free-flyiung, Bounce off of the offending object.
            {
                Trajectory = Vector3.Reflect(Trajectory, c.contacts[0].normal);
            }
        }
    }
    void OnControllerColliderHit(ControllerColliderHit c)
    {
        
            if (Vector3.Angle(c.normal, Vector3.up) > 60 && Vector3.Angle(c.normal, Vector3.up) < 120)
            {
                avoid = c.normal*10;
                Invoke("FinAvoid", .2f);
            }
    }
    float tmod, deltatmod;
    public void modifyflightspeed(float acceleration, float decelleration) //Accelleration is the value to modify
        //flight speed by, decelleration is the value at which the accelleration reaches zero.
    {
        tmod = acceleration;
        deltatmod = decelleration;
    }
    void OnCollisionStay(Collision c)
    {
        WorldObject w = c.transform.GetComponent<WorldObject>();
        if (w)
        {
            if (w.floor)
            {
                float isvertical = Vector3.Angle(c.contacts[0].normal, Vector3.up);
                if (isvertical < 45)
                {
                    float moveangle = Vector3.Angle(-c.contacts[0].normal, motion.normalized);
                    ymem += (float)Math.Sin(moveangle);
                    print(ymem);
                }
            }
        }
        else if (!w)
        {
            if (avoid != Vector3.zero)
            {
                CancelInvoke("FinAvoid");
                Invoke("FinAvoid", 0.3f);
            }
        }
    }
    bool contextmotion = false;
    bool pause = false;
    void Update()
    {
        yang = transform.eulerAngles.x;
        xang = transform.eulerAngles.y;
        //If in a cutscene or similar contextual animation/motion,
        //return early.
        if (contextmotion)
            return;
        if (isdodging)
        {
            ResolveDodge();
        }
        else if (!pause && rockets == -1) //Ground motion Update loop
        {
            motor = GetComponent<CharacterController>();
            if (jumpstate == 2 || jumpstate == 6 || jumpstate == -1 || jumpstate == 3)
            {
                CheckFloor();
            }
            else
            {
                vdisplace = transform.position.y - floor;
                ground = false;
            }
            SetZ();
            if(!freezespdmod)
            {
                UnlockAccel();
            }
            if (!freezespdmod && !(input.Equals(Vector3.zero)))
                spdmod += accelmod;
            if (!freezespdmod &&(spdmod > 1))
                spdmod = 1;
            if (OR)
            {
                if (lockon.Exec(transform))
                    lockon = null;
                if (lockon == null)
                    OR = false;
                return;
            }
            else
            {
                motion = input * spdmod*osmod;
            }
            float dltt = Time.deltaTime;
            switch (jumpstate)
            {
                case -1:
                    motion.y = 0.0f;
                    break;
                case 0:
                    movewith = null;
                    motion.y = jumpspeed;
                    jsi = motion.y;
                    jumpstate = 1;
                    break;
                case 1:
                    jsi -= jmod*dltt;
                    motion.y = jsi;
                    if (jsi < 0.1f)
                    {
                        jumpstate = 2;
                    }
                    break;
                case 2:
                    if (jsi > 0)
                        jsi = 0;
                    jsi -= jumpdrag*dltt;
                    motion.y = jsi;

                    break;
                case 3:
                    
                    jsi -= jumpdrag*dltt;
                    motion.y = jsi;
                    break;
                case 4:
                    jsi = 1.25f * jumpspeed;
                    motion.y = jsi;
                    jumpstate = 5;
                    Animator tdot = GetComponent<Animator>();
                    if (tdot != null)
                    {
                        tdot.SetTrigger("Land");
                        tdot.SetTrigger("Jump");
                    }
                    break;
                case 5:
                    jsi -= jmod * dltt;
                    motion.y = jsi;
                    if (jsi < 0.1f)
                        jumpstate = 6;
                    break;
                case 6:
                    jumpstate = 2;
                    if (jsi > 0)
                        jsi = 0;
                    jsi -= jumpdrag*dltt;
                    motion.y = jumpdrag;
                    break;
            }
            switch (spstate)
            {
                case -1:
                    break;
                case 0:
                    motion.y = 0.0f;
                    transform.position = new Vector3(transform.position.x, ymem, transform.position.z);
                    motion *= 1.3f;
                    break;
            }
            if (movewith != null)
            {
                motion += movewith.GetComponent<MovingPlatform>().diff;

            }
            GetComponent<CharacterController>().Move(motion * Time.deltaTime);
        }
        else if (!pause && rockets != -1) //flight motion update loop
        {
            
            SetZ();
            prevtrans = Trajectory.normalized;
            if (rockets == 1)
            {
                if (focus == null)
                {
                    rockets = 2;
                    Trajectory = (-transform.forward)*flightspeed;
                    Trajectory.y = 0;

                }
                else if (focus != null)
                {
                    rockets = 3;
                    Trajectory = (focus.position-transform.position).normalized;
                    Trajectory *= flightspeed * 1.5f;
                }
            }
            else if (rockets == 2)
            {
                //For Rocket Dash, set the Camera mode.
                Camera.main.GetComponent<FASTACTIONCAMERA>().SetMode(1);
                    //If we're rocket dashing, e want input straight from the left stick (unmodified by the header, independent of camera rotation):
                float rlsx= Input.GetAxis("U");
                float rlsy = Input.GetAxis("V");

                float pickera = Math.Abs(rlsx);
                float pickerb = Math.Abs(rlsy);
                bool pitched = false;
                Trajectory = Trajectory.normalized;
                if (pickera > pickerb)
                {
                    Quaternion eulerot = Quaternion.Euler(new Vector3(0, Fturnv*Time.deltaTime*rlsx, 0));
                    Trajectory = eulerot * Trajectory;
                    
                }
                else if (pickera < pickerb)
                {
                    Vector3 right = Quaternion.Euler(0, 90, 0) * (new Vector3(Trajectory.x, 0, Trajectory.z));
                    Vector3 left = -right;
                    float pitchcj = transform.eulerAngles.x;
                    pitchcj += rlsy*Fpitchv*Time.deltaTime;
                    Quaternion Traject = Quaternion.AngleAxis(rlsy * Fpitchv * Time.deltaTime, right);
                    Trajectory = Traject * Trajectory;
                    pitched = true;
                }

                if (!pitched)
                {
                    Vector3 corrected = Trajectory;
                    float currpitch = transform.eulerAngles.x;
                    if (currpitch < zeropitchref - 0.2f)
                        corrected = Quaternion.Euler(5 * Time.deltaTime, 0, 0) * Trajectory;
                    else if (currpitch > zeropitchref + 0.2f)
                        corrected = Quaternion.Euler(-5 * Time.deltaTime, 0, 0) * Trajectory;
                    Trajectory = corrected;
                }

                Trajectory *= flightspeed;
                if (focus) //If we have a target, switch to homing dash on next frame.
                {
                    rockets = 3;
                    tmod = deltatmod * tmod;
                    if (tmod > 2 * flightspeed || tmod < 0.1f)
                        tmod = 0;
                    float tacc = 1+tmod;
                    Camera.main.GetComponent<FASTACTIONCAMERA>().SetMode(0);
                    Trajectory = tacc*(focus.position - transform.position).normalized;
                }
            }
            else if (rockets == 3)
            {
                if (focus == null)
                {
                    print("ROCKETS 3 UR");
                    UnRocket();
                }
                else
                {
                    Trajectory = (focus.position - transform.position).normalized;
                    Trajectory *= flightspeed;
                }
            }
            transform.LookAt(transform.position - Trajectory.normalized);
            mov.Move(Trajectory*Time.deltaTime);
        }
        if (impulse) //External forces update loop.
        {
            if (impulsepow < 0.1f)
                impulse = false;
            impulsepow *= DRAGFACTOR;
            mov.Move(impulsenorm *10* impulsepow*Time.deltaTime);
        }
        if (avoid != Vector3.zero)
        {
            mov.Move(avoid * Time.deltaTime);
            avoid *= .5f;
        }
    }
    bool pitchhasinput = false;
    public void UnRocket()
    {
        Camera.main.GetComponent<FASTACTIONCAMERA>().SetMode(0);
        rockets = -1;
        HitWithAtk(-Trajectory, 5.0f);
        prevtrans.y = 0;

    }
    
}

