using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
//Base class for world objects,
    public class WorldObject : MonoBehaviour
    {
    Collider c;
    Rigidbody r;
    Renderer q;
    Vector3 pos;
    int loadzone;
    protected void SetupData()
    {
        c = GetComponent<Collider>();
        r = GetComponent<Rigidbody>();
        q = GetComponent<Renderer>();
        pos = transform.position;
    }

    public void Load()
    {
        transform.position = pos;
        if (c)
            c.enabled = true;
        if (r)
            r.isKinematic = false;
        if (q)
            q.enabled = true;
    }

    public void UnLoad()
    {
        pos = transform.position;
        if (c)
            c.enabled = false;
        if (r)
            r.isKinematic = true;
        if (q)
            q.enabled = false;
    }

        public int height;
        public bool gravity;
        public bool hard;
        public Vector3 PhysicalOffset;
        public bool physical;
        public float BOffset;
        public bool invul;
        public bool interactive;
        public bool floor;
        public bool wall;
        public virtual void Strike(Vector3 dir, float force)
        {
            invul = true;
            Invoke("Unvul", 0.4f);
        }
        protected void Unvul()
        {
            invul = false;
        }
        //Calls when receives an interaction signal from player character.
        public virtual bool OnInteract(Transform t)
        {
            return false;
        }
        
        public virtual void TearItDown()
        {
        }
        protected virtual void CollisionEvent(Transform t)
        {
        }
        protected virtual void CollisionEvent()
        {
        }
        protected virtual void CollisionEvent(Collision c)
        {
        }

        protected virtual void CollisionEvent(ControllerColliderHit cch)
        {
        }

        void OnCollisionEnter(Collision c)
        {
            CollisionEvent(c);
        }

        void OnControllerColliderHit(ControllerColliderHit cch)
        {
            CollisionEvent(cch);
        }
    }

