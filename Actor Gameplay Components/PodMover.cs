using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Nite-Basik/Action Coordination/AI/Pod Mover")]
    public class PodMover : BaseMover
    {
        Rigidbody rig;
        public float Force;
        public int Bounce, Slide, HitFloor, HitWall, Recover;
        public Transform chiefRend;
        AudioSource snd;
        public float UpwardBounce;
        bool onground;
        public bool movefine;
        Animator anima;
        AICharacter holder;
        Vector3 xOff, yOff, zOff;
        public int SoundContext;
        float estimatedbouncelength = 0, estimatedslidelength = 0;
        Vector3 lastbounce, lastslide;
        void RandomSignal()
        {
        }

        public bool pause;

        public void Pause()
        {
            CancelInvoke("AddForce");
            rig.velocity = Vector3.zero;
            pause = true;
        }

        public void UnPause()
        {
            onground = true;
            pause = false;
        }

        public void Pause(float t)
        {
            pause = true;
            Invoke("UnPause", t);
        }

        public override bool Motion()
        {
            return !(pause || stunned || rig.velocity == Vector3.zero);
        }
        Quaternion currentRotation;
        public void SetMotion(Vector3 v)
        {
            goaldir = v;
            Vector3 dir = transform.forward;
            goaldir.y = 0;
            dir.y = 0;
            Quaternion quatjr = Quaternion.Euler(0, 15, 0);
            Quaternion quatsr = Quaternion.Euler(0, -15, 0);
            Vector3 normsiff = transform.forward;
            Vector3 j1 = quatjr * normsiff;
            Vector3 j2 = quatsr * normsiff;
            float j11 = Vector3.Angle(v, j1);
            float j21 = Vector3.Angle(v, j2); //Sets current rotation at 5o per motion, picks rotation direction
            //of the least angular displacement from current forward direction.
            if (j11 > j21)
                currentRotation = Quaternion.Euler(0, -30, 0);
            else
                currentRotation = Quaternion.Euler(0, 30, 0);


            movesig = currentRotation * dir;
        }

        public void RotateToAvoid(Vector3 point)
        {
            if (Vector3.Angle(Vector3.forward, goaldir) < 35)
            {
                Quaternion quatjr = Quaternion.Euler(0, 15, 0);
                Quaternion quatsr = Quaternion.Euler(0, -15, 0);
                Vector3 normsiff = transform.forward;
                Vector3 j1 = quatjr * normsiff;
                Vector3 j2 = quatsr * normsiff;
                float j11 = Vector3.Angle(point, j1);
                float j21 = Vector3.Angle(point, j2);
                if (j11 < j21)
                    currentRotation = Quaternion.Euler(0, -30, 0);
                else
                    currentRotation = Quaternion.Euler(0, 30, 0);

            }
        }

        public override void HitWithAtk(Vector3 dir, float amnt)
        {
            //Getting hit with an attack produces the same logic as colliding with a wall,
            //except for longer.
            stunned = true;
            Invoke("UnStun", 1.2f);
            //Animator functions are to be set per-character;
            //Here, the example character, William's, PodMover is the mover in question
            anima.SetInteger("sig", 0);
            anima.SetTrigger("CIOR");
            rig.AddForce(dir * amnt*100);
        }

        public override Vector3 LocalForward()
        {
            return transform.forward;
        }

        public void MoveFine(bool v)
        {
            movefine = v;
        }

        void Start()
        {
            holder = GetComponent<AICharacter>();
            snd = GetComponent<AudioSource>();
            rig = GetComponent<Rigidbody>();
            anima = GetComponent<Animator>();
            lastbounce = transform.position;
            lastslide = transform.position;
            Renderer rdr = chiefRend.GetComponent<Renderer>();
            xOff = new Vector3(rdr.bounds.size.x, 0, 0);
            yOff = new Vector3(0, rdr.bounds.size.y, 0);
            zOff = new Vector3(0, 0, rdr.bounds.size.z);
        }
        void WouldSlitherAgain()
        {
            canslide = true;
        }
        bool canslide = true;
        Vector3 rr, movesig, goaldir;
        bool stunned = false;
        //We don't need explicit jump logic here;
        //On the top-level controller script, maybe
        //put in logic to switch from fine (slithering) to coarse (bouncing)
        //when approaching an obstacle;
        //While bouncing, the podmover moves forwards and up simultaneously.
        public float ForceToDistanceRatio()
        {
            return estimatedbouncelength / Force;
        }
        bool target = false;
        void Update()
        {
            Debug.DrawRay(transform.position, goaldir * 10, Color.red, 2.0f);
            Debug.DrawRay(transform.position, movesig * 10, Color.green, 2.0f);
            if(!stunned && !pause){ //If not stunned or pause, execute motion
            if ((onground && !movefine) || (canslide && movefine)) //Additionally, check to make sure we're not in progress of another motion
            {
                rr = movesig; //local direction vector (rr), update 
                rr = new Vector3(rr.x, 0, rr.z);
                //Rotate to face Goal Direction if not already within one rotation of it.

                if (Vector3.Angle(transform.forward, goaldir) > 32)
                {
                    movesig = currentRotation * movesig;
                }
                //Check to make sure we're not about to bounce off of a ledge
                if (!target)
                {
                    //If we're about to bounce off of a ledge, and we are NOT targeting the prime context,
                    //immediately reverse move direction (the previous location must have been on the same level as the last valid ground contact,
                    //thsi is implicit due to the way this mover functions.)

                    RaycastHit rca;
                    Debug.DrawRay(transform.position + rr*15, Vector3.down*100.0f, Color.magenta, 2.0f);
                    print(groundContactPoint);
                    if (Physics.Raycast(new Ray(transform.position + rr * 15, Vector3.down), out rca, 100.0f))
                    {//Perform a Raycast hit straight down from a great distance away from the mover, at ground level.  If we made a hit,
                        //and the ground level found is lower than the current ground level, reverse.
                        print(rca.point.y);
                        if (groundContactPoint-rca.point.y > 1.0f)
                        {
                            print("insufficient space " + rca.point.y + ", " + groundContactPoint);
                            rr *= -1;
                        }
                    }
                    else
                    {//Alternatively, if no contact is made, that just means the ground is really just that low.  Also reverse move direction in this case.
                        //This prevents attached character from killing itself on its own,
                        //Bouncing against another pod mover may knock one off edge, but that happens so rarely that it just adds an extra layer of realism.
                        print("insufficient space " + rca.point.y + ", " + groundContactPoint);
                        rr *= -1;
                    }
                }
                
                if (!movefine)
                {
                    
                anima.SetTrigger("CoarseMove");
                    rr *= Force;
                    rr += Vector3.up * UpwardBounce;
                    onground = false;
                }
                else
                {
                    canslide = false;
                    onground = true;
                    anima.SetTrigger("FineeMove");
                    Invoke("WouldSlitherAgain", 0.9f);
                    rr *= Force * 0.75f;
                }
                Vector3 look = new Vector3(transform.position.x + rr.x, transform.position.y, transform.position.z + rr.z);
                transform.LookAt(look);
                Invoke("AddForce", 0.18f);
                
                canslide = false;    
            }
            }
        }
        float groundContactPoint;
        void AddForce()
        {
            if (!pause)
            {
                if (rr.y == 0)
                {
                    if (Vector3.Distance(transform.position, lastslide) > estimatedslidelength)
                    {
                        estimatedslidelength = Vector3.Distance(transform.position, lastslide);
                    }
                    lastslide = transform.position;
                    snd.PlayOneShot(SoundTable.GetSound(SoundContext, Slide));
                }
                else
                {
                    if (Vector3.Distance(transform.position, lastbounce) > estimatedbouncelength)
                    {
                        estimatedslidelength = Vector3.Distance(transform.position, lastbounce);
                    }
                    lastbounce = transform.position;
                    snd.PlayOneShot(SoundTable.GetSound(SoundContext, Bounce));
                }
                rig.AddForce(rr);
            }
        }
        public bool DoNotAnimate;
        void UnStun()
        {
            snd.PlayOneShot(SoundTable.GetSound(SoundContext, Recover));
            if(!DoNotAnimate)
            anima.SetInteger("sig", 2);
            Invoke("UnStunPhaseB", 0.6f);
        }
        void UnStunPhaseB()
        {
            stunned = false;
        }
        void OnCollisionEnter(Collision c)
        {
            if(ContextBody.Same(transform, c.transform) || c.transform.GetComponent<ProjectileWeapon>())
                return;
            rig.velocity = Vector3.zero;
                if (Vector3.Angle(Vector3.up, c.contacts[0].normal) < 30)
                {
                    if (c.transform.GetComponent<WorldObject>())
                    {
                        groundContactPoint = c.contacts[0].point.y;
                        holder.MoverSafe();
            
                        snd.PlayOneShot(SoundTable.GetSound(SoundContext, HitFloor));
                        onground = true;
                    }
                }
                else if(!pause)
                {
                    float leftwall = float.PositiveInfinity, rightwall = float.PositiveInfinity;
                    bool turnleft = false;
                    RaycastHit lhit, rhit;
                    if(Physics.Raycast(new Ray(transform.position + xOff, transform.right), out rhit, 30.0f))
                    {
                        rightwall = rhit.distance;
                    }
                    if(Physics.Raycast(new Ray(transform.position - xOff, -transform.right), out lhit, 30.0f))
                    {
                        leftwall = lhit.distance;
                    }
                    if(rightwall < leftwall)
                    {
                        turnleft = true;
                    }
                    CancelInvoke("RandomSignal");
                    Invoke("RandomSignal", 6);
                    if(leftwall < 5 && rightwall < 5)
                    {
                        movesig = -transform.forward;
                    }
                    else if(turnleft)
                    {
                        movesig = -transform.right - transform.forward * .5f; ;
                    }
                    else
                    {
                        movesig = transform.right-transform.forward*.5f;
                    }
                    if (c.transform.GetComponent<WorldObject>())
                    {
                        snd.PlayOneShot(SoundTable.GetSound(SoundContext, HitWall));
                        stunned = true;
                        Invoke("UnStun", 1.2f);
                        if (!DoNotAnimate)
                        {
                            anima.SetInteger("sig", 0);
                            anima.SetTrigger("CIOR");
                        }
                    }
                    rig.AddForce(c.contacts[0].normal*UpwardBounce);
                }
            
        }


    }

