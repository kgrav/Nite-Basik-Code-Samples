using UnityEngine;
using System;
using System.Collections.Generic;

[AddComponentMenu("Nite-Basik/Action Coordination/Everyone/Hand")]
    public class Fist : MonoBehaviour
    {
        public Vector3 HandOffset;
        public float throwpower;
    
        void Start()
        {
        }


        void Update()
        {
        }

        public void OnTriggerEnter(Collider c)
        {

        }

        Transform hold;

        public void ThrowIt(float power, int sound)
        {
            if (hold)
            {
               ProjectileWeapon w = hold.GetComponent<ProjectileWeapon>();
                if (w)
                {
                    w.Throw();
                } hold.SetParent(null);
                hold.GetComponent<Rigidbody>().AddForce((transform.root.forward + 0.5f*Vector3.up) * throwpower);
                
            }
        }

        public void DropIt()
        {
            if (hold)
            {
                hold.SetParent(null);
                hold.GetComponent<Rigidbody>().AddForce(ProjectileInterface.arc);
                hold.GetComponent<ProjectileWeapon>().Activate();
            }
        }

        public void HoldThis(Transform objkt)
        {
            hold = objkt;
            objkt.parent = transform;
            Vector3 abv = objkt.GetComponent<Renderer>().bounds.size;
            objkt.localPosition = Vector3.zero;
        }


    
}
