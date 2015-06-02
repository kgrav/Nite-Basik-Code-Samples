using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
//Items which have a physical representation
//On interact, they will either go straight to the inventory,
//or be picked up and held by the manipulator.
[AddComponentMenu("Nite Basik/Physical Objects/Item")]
    public class Item : PhysicalWorldObject
    {
        public ItemInfo IR;
        //Item Info class holds information and methods unique to the item.
        //The specifics of what each individual item does, and how it behaves,
        //are stored in a class which derives from it (for ex. Rocks use the Rock class,
        //which derives from ItemInfo.)
        //This allows items to have very specific functionality, but a standardized set of
        //interactions and behaviours.
        public string nameit;
        public int typeid;
        public int instanceid;
        public bool CollectOnStumble;
        public bool ManualPickup;
        public bool Holdable;
        public bool heavy;
        Transform tran;
        bool thrown;
        bool held;
        AudioSource source;
        public AudioClip clip;
        Collider collid;
        Ion ion;
        int heldbycontext; //group ID of the object's current holder

        Vector3 globalpos;

        protected override void itemstart()
        {
            SetupData();
            offset = GetComponent<Renderer>().bounds.size;
            ion = GetComponent<Ion>();
            collid = GetComponent<Collider>();
            rig = GetComponent<Rigidbody>();
            IR = ItemInfo.GetFromKey(typeid);
            source = GetComponent<AudioSource>();
            tran = GetComponent<Transform>();
            inertia = Vector3.zero;
        }
        public override void TearItDown()
        {
            GameObject.Destroy(gameObject);
        }

        public override bool OnInteract(Transform t)
        {
            InspectBody ins = GetComponent<InspectBody>();
            if (ManualPickup && Holdable)
            {
                if (clip)
                {
                    source.PlayOneShot(clip);
                }
                if (ins)
                    ins.OnInteract(t);
                heldbycontext=ContextBody.GroupOf(t);
                GetComponent<Transform>().SetParent(t); collid.enabled = false;
                rig.isKinematic = true;
                rig.useGravity = false;
                if(ion)
                ion.enabled = false;
                return false;
            }
            else if (ManualPickup)
            {
                collid.enabled = false;
                rig.isKinematic = true;
                rig.useGravity = false;
                if(ion)
                ion.enabled = false;
                if (clip)
                {
                    source.PlayOneShot(clip);
                }
                if (ins)
                    ins.OnInteract(t);
                IR.Pickup(t);
                return true;
            }
            else
                return false;
        }

        public void ManualRelease()
        {
            collid.enabled = true;
            rig.isKinematic = false;
            if (!ion) //if this item does not respond to magnetic fields, turn on default gravity.
                rig.useGravity = true;
            else
                ion.enabled = false;
            GetComponent<Transform>().SetParent(null);
            Pop();
        }

        void InitCollide()
        {
            collid.enabled = true;

        }
        public void Throw(Vector3 dir)
        {
            transform.SetParent(null);
            print("Thrown");
            rig.isKinematic = false;
            if (!ion)
                rig.useGravity = true;
            else
                ion.enabled = true;
            thrown = true;rig.AddForce(dir*(800/mass)); 
            IR.Throw(transform, dir);
            Invoke("InitCollide", 0.3f); //Collider should remain off for a short time following the throw.
        }
        //if this item is collectible upon collision,
        //and is not colliding with the Manipulator Object currently
        //holding it, it will be added to the inventory.
    //
    //Otherwise, if thrown and not colliding with the thrower, call the item's collision event.
    //If colliding with a non-physical world object, cancel the throw state,
        protected override void CollisionEvent(Collision c)
        {
            GetComponent<AudioSource>().PlayOneShot(clip);
            if (!(ContextBody.GroupOf(c.transform) == heldbycontext))
            {

                if (!thrown)
                {
                    Inventory i = c.gameObject.GetComponent<Inventory>();
                    if ((CollectOnStumble) && i != null)
                    {

                        IR.Pickup(c.transform);
                        i.Pickup(this);
                        TearItDown();
                    }
                }
                else
                {
                    if (c.transform.GetComponent<WorldObject>() != null)
                    {
                        if(!c.transform.GetComponent<WorldObject>().physical)
                        thrown = false;
                    }
                    IR.HitFromThrow(transform, c.transform, c);
                }
            }
        }
        protected override void CollisionEvent(ControllerColliderHit c)
        {
            GetComponent<AudioSource>().PlayOneShot(clip);
            if (!(ContextBody.GroupOf(c.transform) == heldbycontext))
            {
                print("COLLISION");
                if (!thrown)
                {
                    Inventory i = c.gameObject.GetComponent<Inventory>();
                    if ((CollectOnStumble) && i != null)
                    {

                        IR.Pickup(c.transform);
                        i.Pickup(this);
                        TearItDown();
                    }
                }
                else
                {
                    thrown = false;
                    //IR.HitFromThrow(transform, c.transform);
                }
            }
        }        
    }

