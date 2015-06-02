using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[AddComponentMenu("Nite-Basik/Action Coordination/Trail Child")]
    public class TrailChild : ParticleChild
    {
        public GameObject trail;
        public Transform[] anchors;

        public override void StartFunction()
        {
            base.StartFunction();
        }
        public override void SwitchOn()
        {
            if (!on)
            {
                for (int i = 0; i < anchors.Length; ++i)
                {
                    GameObject j = (GameObject)Instantiate<GameObject>(trail);
                    j.transform.position = anchors[i].position;
                    j.transform.SetParent(anchors[i]);
                    anchors[i] = j.transform;

                }
                on = true;
            }
        }

        public override void SwitchOff()
        {
            if (on)
            {
                for (int i = 0; i < anchors.Length; ++i)
                {
                    Transform q = anchors[i].parent;
                    anchors[i].parent = null;
                    Destroy(anchors[i].gameObject);
                    anchors[i] = q;
                }
                on = false;
            }
        }
    }

