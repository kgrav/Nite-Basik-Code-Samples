using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[AddComponentMenu("Nite-Basik/Mechanical Constructs/Ion (magnetic particle)")]
    public class Ion : MonoBehaviour
    {
    public int IonKey;



    void OnCollisionEnter(Collision c)
    {
        GetComponent<Rigidbody>().AddForce(c.contacts[0].normal * GetComponent<Rigidbody>().velocity.magnitude);
    }
    }
