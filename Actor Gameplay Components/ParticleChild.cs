using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[AddComponentMenu("Lady Stardust/Action Coordination/Everyone/Particle Child")]
    public class ParticleChild : MonoBehaviour
    {
        public ParticleSystem parts;
        public int callno;
        public bool on;
        public bool AlsoSound;
        public ParticleSystem alt;
        void Start()
        {
            StartFunction();
        }

        void Update()
        {
            UpdateFunction();
        }
        public virtual void StartFunction()
        {
            parts = GetComponent<ParticleSystem>();
            GetComponentInParent<ParticleManager>().RollCall(callno, this);
            parts.enableEmission = false;
            if (on)
            {
                parts.enableEmission = true;
            }
        }
        public virtual void UpdateFunction()
        {
            if (on)
            {
                parts.enableEmission = true;
            }
            else
            {
                parts.enableEmission = false;
            }
        }
        public virtual void SwitchOn()
        {
            on = true;
            if (AlsoSound)
            {
                GetComponent<AudioSource>().volume = 1;
            }
        }

        public virtual void SwitchOff()
        {
            on = false;
            if (AlsoSound)
            {
                GetComponent<AudioSource>().volume = 0;
            }
        }
    }
