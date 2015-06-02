using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//Class for objects which may or may not 
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Nite-Basik/Physical Objects/Physics World Object")]
public class PhysicalWorldObject : WorldObject
{
    public int FastCollisionSound;
    public int MediumCollisionSound;
    public int SlowCollisionSound;
    public GameObject destroyPrefab;
    public bool Pushable;
    public bool IgnoreIntegrity;
    public bool Breakable;
    
    public bool SupportedFromAbove;
    public bool GravityForever;
    public bool supported;
    public Vector3 offset, inertia;
    public float drag, mass, resistance, maxinteg, corpsedelay, crushcheck, springiness;
    float integ;
    float currspeed;
    bool broken;
    bool supporting;
    bool ignorehits;
    bool usingphysics;
    bool falling;
    public bool canactassupport;
    public bool canbecrushed;
    public bool hurts;
    public bool lockpos;
    public bool fragile;
    static int destroycount = 0;
    Transform supports;
    protected Rigidbody rig;
    void TurnOffInvul()
    {
        ignorehits = true;
    }
    protected virtual void itemstart()
    {
    }
    float timein;
    void Start()
    {
        timein = Time.time;
        itemstart();
        SetupData();
        broken = false;
        rig = GetComponent<Rigidbody>();
        offset = GetComponent<Renderer>().bounds.size/2;
        supporting = false;
        lockpos = false;
        falling = GravityForever;
        integ = maxinteg;
        supportgroup = new List<Transform>();
        if (GravityForever)
        {
                GetComponent<Rigidbody>().useGravity = true;
        }
    }

    public void Bump(Vector3 b)
    {
            rig.velocity += b;
    }

    void Lock()
    {
        if (rig.velocity.magnitude < 0.1f)
            rig.isKinematic = true;
        else
            Invoke("Lock", .5f);
    }
    protected override void CollisionEvent(Collision c)
    {
        print(ContextBody.IsActor(c.transform));
        if (ContextBody.IsActor(c.transform) && rig.velocity.magnitude > 1)
        {
            HitZone alive = c.gameObject.GetComponent<HitZone>();
            if (alive)
            {
                Vector3 diff = alive.BluntOn(rig.velocity.normalized, mass, hurts, transform);
                inertia -= diff;

                //rig.AddForceAtPosition(c.contacts[0].normal * springiness, c.contacts[0].point);

            }
        }
        else
        {
            if (c.relativeVelocity.magnitude > 2.0f)
                WorldAudio.PlaySound(transform.position, FastCollisionSound);
            else
                WorldAudio.PlaySound(transform.position, SlowCollisionSound);
            if (c.gameObject.GetComponent<WorldObject>() && c.gameObject.GetComponent<WorldObject>().physical)
            {
                if (Vector3.Angle(c.contacts[0].normal, Vector3.up) < 15 || (SupportedFromAbove && Vector3.Angle(c.contacts[0].normal, Vector3.down) < 15))
                {
                    bool skip = false;
                    foreach (Transform t in supportgroup)
                    {
                        if (c.transform.name.Equals(t.name))
                            skip = true;
                    }
                    if (!skip)
                    {
                        print("AddingSupport to " + transform + ": " + c.transform.name);
                        supportgroup.Add(c.transform);
                        falling = false;
                        if (Time.time - timein > 2.0f)
                            Invoke("Lock", 1.5f);
                        else
                            rig.isKinematic = true;
                        supportCount++;
                        return;
                    }
                }
            }
            WorldObject notalive = c.gameObject.GetComponent<WorldObject>();
            //Make sure the collision is oriented  towards velocity before applying kinetic force.
            Vector3 cAnglea = -c.contacts[0].normal;
            Vector3 cAngleb = rig.velocity.normalized;
            Debug.DrawRay(c.contacts[0].point, cAnglea * 10.0f, Color.cyan, 4.0f);

            Debug.DrawRay(c.contacts[0].point, cAngleb * 10.0f, Color.green, 4.0f);
            float hitAngle = Vector3.Angle(cAnglea, cAngleb);
            float amod = (float)Math.Sin(hitAngle);
            print(hitAngle + ", " + amod);
            if (currspeed != 0)
            {

                if (notalive)
                {
                    notalive.Strike(rig.velocity.normalized, rig.velocity.magnitude);

                    rig.AddForceAtPosition(c.contacts[0].normal * springiness, c.contacts[0].point);

                }
            }
        }
    }
    public bool still;
    public float popcheck;
    bool popped;
    void Update()
    {
        inertia = rig.velocity;
        for (int i = 0; i < supportgroup.Count; ++i)
        {
            if (!supportgroup[i])
            {
                print(transform.name + ": Support Lost (" + supportgroup[i] + ")");
                supportCount--;
                supportgroup.Remove(supportgroup[i]);
            }
        }
        if (supportCount <= 0)
        {
            rig.isKinematic = false;
        }
        if (!supporting && !broken && !falling &&canactassupport)
        {
            RaycastHit s;
            if (Physics.Raycast(new Ray(transform.position + new Vector3(0, offset.y, 0), Vector3.up), out s, 0.2f))
            {
                if (s.collider.GetComponent<PhysicalWorldObject>() != null)
                {
                    print(transform + "Contact with" + s.collider.transform);
                    supporting = true;
                    supports = s.transform;
                }
            }
        }
    }
    public bool ignoreALLhits;
    public override void Strike(Vector3 dir, float force)
    {
        WorldAudio.PlaySound(transform.position, MediumCollisionSound);
        if (!ignorehits && !ignoreALLhits)
        {
            print(transform + " hit");
            if (fragile)
                Invoke("TearItDown", 0.4f);
            if (!falling)
                Pop();
            ignorehits = true;
            integ -= force;
            if (integ < 0 && !IgnoreIntegrity)
            {
                broken = true;
                if (Breakable)
                    TearItDown();
            }

            if (Pushable)
            {
                    GetComponent<Rigidbody>().isKinematic = false;
                    GetComponent<Rigidbody>().useGravity = true;

                if (supporting)
                    supports.GetComponent<PhysicalWorldObject>().Pop();

                inertia = dir * force*100;
                if (mass == 0)
                      mass = 1;
                inertia /= mass;
                rig.AddForce(inertia);
                


            }
            Invoke("TurnOffInvul", 0.3f);
        }
    }
    //Destroy object, create destroyed object parts
    public override void TearItDown()
    {
        if (supporting)
        {
            for (int i = 0; i < supports.GetComponent<PhysicalWorldObject>().supportgroup.Count; ++i)
            {
                if (supports.GetComponent<PhysicalWorldObject>().supportgroup[i].name.Equals(transform.name))
                {
                    print("Removing Support " + supports.GetComponent<PhysicalWorldObject>().supportgroup[i].name);
                    supports.GetComponent<PhysicalWorldObject>().supportgroup.RemoveAt(i);
                }
            }
            supports.GetComponent<PhysicalWorldObject>().Pop();
        }
        GameObject gq = (GameObject)GameObject.Instantiate(destroyPrefab);
        gq.transform.position = transform.position;
        gq.GetComponent<DestroyedObjectSignal>().DESTROY();
        GameObject.Destroy(gameObject);
    }

    List<Transform> supportgroup;
    void InvokePop()
    {
        Invoke("Pop", 0.8f);
    }
    public virtual void Glue(Vector3 pt, float parentoffs)
    {
        lockpos = true;
        falling = false;
        rig.useGravity = false;
    }
    void reset()
    {
        popped = false;
    }
    public int supportCount = 0;
    public virtual void Pop()
    {
        print(transform + " displaced");
            lockpos = false;
        supportCount--;
        popped = true;
            Invoke("Reset", popcheck);
            if (supporting)
            {
                supports.GetComponent<PhysicalWorldObject>().Pop();
            }
   }



}