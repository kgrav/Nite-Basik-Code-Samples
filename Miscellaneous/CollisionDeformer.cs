using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Attach to a Deformable Mesh to deform on collision.
[RequireComponent(typeof(DeformableMesh))]
[RequireComponent(typeof(Collider))]
public class CollisionDeformer : MonoBehaviour
{
    DeformableMesh me;
    void Start()
    {
        me = GetComponent<DeformableMesh>();
    }

    void OnCollisionEnter(Collision c)
    {
        me.ApplyForce(c.relativeVelocity.normalized, c.contacts[0].point,c.relativeVelocity.magnitude*.02f);
        
    }
}

