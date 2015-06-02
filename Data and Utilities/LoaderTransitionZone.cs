using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Nite-Basik/Data/LTC Zone")]
    public class LoaderTransitionZone : MonoBehaviour
    {
        public int Correllary;
        //Correllary corresponds to the Anterior zone with respect to player location this transition zone represents.
        public int MainCharacterContext;
        LoaderTransitionConstruct parent;

        void Start()
        {
            parent = GetComponentInParent<LoaderTransitionConstruct>();
        }


        void OnTriggerEnter(Collider c)
        {
            if (ContextBody.GroupOf(c.transform) == MainCharacterContext && c.transform.parent == null)
            {
                parent.EnterSig(Correllary);
            }
        }

        void OnTriggerStay(Collider c)
        {
        }

        void OnTriggerExit(Collider c)
        {
            if (ContextBody.GroupOf(c.transform) == MainCharacterContext && c.transform.parent == null)
            {
                parent.ExitSig(Correllary);
            }
        }
    }

