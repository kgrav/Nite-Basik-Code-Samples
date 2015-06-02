using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Mover component for AI characters which locomote by translating across flat surfaces.
public enum LEGFLAGS { FORWARD, JUMP, STOP, BACKUP, ROTATE }
public enum SPDFLAGS { NONE, CAUTION, FULL }

[AddComponentMenu("Nite-Basik/Action Coordination/AI/AI Legs")]
public class AILegs : BaseMover
{
    public float TopSpeed;
    public float CautionSpeed;
    public bool flies;
    public bool properlyaligned;
    public bool sticktoground;
    public float RotationSpeed;
    public bool blanky;
    public Vector3 stopat;
    public float yspeed;
    public int ferror; // 1 if AI character is properly aligned (front of model is aligned with local Z+ axis),
    //-1 if character is reversely aligned (front of model is aligned with local Z- axis)
    //Setting to '0' will prevent all movement and checking.
    //If enemy model is not aligned with the Z axis,
    //open the model in the appropriate 3D modeling software,
    //and rotate the model and the armature to face the proper axis.
    //I do not recommend computing every single motion vector at a 90 degree angle.
    CharacterController motor;
    public Vector3 moveDir;
    Vector3 inputDir;
    Vector3 position;
    Vector3 prevPosition;
    Vector3 facing;
    Vector3 up;
    Vector3 impulsedir;
    bool impulse;
    float impulsepow;
    float dec;
    Vector3 impulsenorm;
    public float ImpulseFactor;
    public float stundir;
    LEGFLAGS b, bn;
    SPDFLAGS m, mn;
    bool newmotion;
    bool stopor;
    bool move;
    bool rotate;

    AICharacter holder;
    //Called from Melee weapon, move backwards and stun.
    public override void Reflect(Vector3 dir, float amnt)
    {
        HitWithAtk(dir, amnt);
        holder.Stun(stundir);
    }

    //Called from AI Character class, set impulse force and direction based on received attack.
    public override void HitWithAtk(Vector3 dir, float amnt)
    {
        impulse = true;
        impulsenorm = dir.normalized;
        impulsenorm.y = 0;
        dec = amnt / 5;
        impulsepow = dec;
    }
    void Start()
    {
        if (sticktoground)
        {
            sticktoground = false;
            Invoke("YLock", 1.0f);
        }
        holder = GetComponent<AICharacter>();
        FinAvoid();
        newmotion = false;
        stopor = false;
        moveDir = transform.forward*ferror;
        inputDir = Vector3.zero;
        position = transform.position;
        prevPosition = transform.position;
        motor = GetComponent<CharacterController>();
        up = transform.TransformDirection(Vector3.up);
        motor = GetComponent<CharacterController>();
        facing = transform.TransformDirection(Vector3.forward);
        b = LEGFLAGS.STOP;
        m = SPDFLAGS.NONE;
    }
    float av;
    Quaternion currentRotation;

    //Rotate motion vector 10 degrees left or right (whichever is furthest) away from point v.
    public void RotateToAvoid(Vector3 v)
    {
        Vector3 ddir = (v - transform.position).normalized;
        if (Vector3.Angle(Vector3.forward, ddir) < 15)
        {
            Quaternion quatjr = Quaternion.Euler(0, 15, 0);
            Quaternion quatsr = Quaternion.Euler(0, -15, 0);
            Vector3 normsiff = transform.forward;
            Vector3 j1 = quatjr * normsiff;
            Vector3 j2 = quatsr * normsiff;
            float j11 = Vector3.Angle(ddir, j1);
            float j21 = Vector3.Angle(ddir, j2);
            if (j11 < j21)
                currentRotation = Quaternion.Euler(0, -10, 0);
            else
                currentRotation = Quaternion.Euler(0, 10, 0);
            inputDir = currentRotation * inputDir;
            Debug.DrawLine(transform.position, inputDir * 10.0f, Color.cyan, 2.0f);
        }
    }
    void YLock()
    {
        sticktoground = true;
    }
    //Set goal direction,
    //this method also computes the least
    //rotation required to reach the direction.
    //(Vector3.Angle is not sufficient, as values returned are always positive,
    //when in practice, a negative rotation may represent a smaller rotation towards the goal)
    public void SetDirection(Vector3 v)
    {
        Debug.DrawRay(transform.position, v * 10.0f, Color.red, 2.0f);
        inputDir = v;
        if (blanky)
            inputDir.y = 0;
        Quaternion quatjr = Quaternion.Euler(0, 10, 0);
        Quaternion quatsr = Quaternion.Euler(0, -10, 0);
        Vector3 normsiff = transform.forward * ferror;
        Vector3 j1 = quatjr * normsiff;
        Vector3 j2 = quatsr * normsiff;
        newmotion = true;
        float j11 = Vector3.Angle(v, j1);
        float j21 = Vector3.Angle(v, j2);
        if (j11 < j21)
            currentRotation = Quaternion.Euler(0, -100 * Time.deltaTime, 0);
        else
            currentRotation = Quaternion.Euler(0, 100 * Time.deltaTime, 0);
    }
    public void Recompute()
    {
        float y1 = Vector3.Angle(transform.forward * ferror, inputDir) * 10 * Time.deltaTime;

    }
    public void SetGoal(Vector3 v)
    {
    }

    public void SetBehaviour(bool movetf, bool rotatetf, string dbm)
    {
        move = movetf;
        rotate = rotatetf;

    }

    public void SetSpeed(SPDFLAGS si)
    {
        mn = si;
    }

    public LEGFLAGS GetBehaviour()
    {
        return b;
    }

    public SPDFLAGS GetSpeed()
    {
        return m;
    }
    public bool STOP;
    void Update()
    {
        if (!STOP)
        {
            //Apply gravity FIRST
            if(!motor.isGrounded)
            motor.Move(new Vector3(0, -2, 0));
            float spd = TopSpeed;
            if (mn == SPDFLAGS.CAUTION)
                spd = CautionSpeed;
            moveDir = transform.forward*ferror;
            Debug.DrawRay(transform.position, inputDir * spd, Color.yellow, 2.0f);
            if (move)
            {//If moving, and not moving in approzimately the correct direction (14 degrees of discrepancy are given)
                //continue pre-computed angular path,
                //always move forward if received movesignal.
                if (Vector3.Angle(moveDir, inputDir) > 30)
                {
                    print("rottin'");
                    moveDir = currentRotation * moveDir;
                    transform.LookAt(transform.position - moveDir);
                }
                moveDir *= spd * Time.deltaTime;
            }
            if (impulse)
            {//computer impulse from attacks and other functions.
                if (impulsepow < 0.1f)
                    impulse = false;
                impulsepow *= DRAGFACTOR;
                moveDir += impulsenorm * ImpulseFactor * impulsepow * Time.deltaTime;
            }
            moveDir += avoid;
            avoid *= .5f;
            motor.Move(moveDir);
            if (sticktoground)
                transform.position = new Vector3(transform.position.x, ylock, transform.position.z);
        }
    }
    
    bool okbounce = true;
    float ylock;
    public float DRAGFACTOR;
    
    Vector3 avoid;
    void FinAvoid()
    {
        avoid = Vector3.zero;
    }
    void tbo()
    {
        motor.enabled = true;
    }
    void OnCollisionEnter(Collision c)
    {
        if (!c.transform.root.Equals(transform.root))
        {
            if (Vector3.Angle(c.contacts[0].normal, Vector3.up) > 60 && Vector3.Angle(c.contacts[0].normal, Vector3.up) < 120)
            {
                avoid = c.contacts[0].normal * 2;
                avoid.y = 0;
                Invoke("finavoid", .2f);
            }
            else if (c.transform.GetComponent<WorldObject>() && Vector3.Angle(c.contacts[0].normal, Vector3.up) < 10)
                ylock = transform.position.y;
        }
    }
    void OnControllerColliderHit(ControllerColliderHit j)
    {
        if(!j.transform.root.Equals(transform.root))
        Invoke("tbo", 0.3f);
    }
    void OnCollisionStay(Collision c)
    {
        //If we're on a flat or slightly inclined surface, set ylock to current position.
        if (Vector3.Angle(c.contacts[0].normal, Vector3.up) < 40)
        {
            if (c.gameObject.GetComponent<WorldObject>() != null)
            {  ylock = transform.position.y; }
        }
    }


}
