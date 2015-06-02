using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

    public class Ability
    {
        protected bool Unlocked = false;
        protected bool Activated = false;
        protected bool Locked = true;
        public bool oneshot;

        protected int jmodsound;
        protected int xmodsound;
        protected int actsound, subactsound;
        public int animkey;
        Transform effect;
        Transform affect;
        bool ExternalManaPool;
        public int localmanamax;
        public float absolutecooldown;
        float currdown = 0.0f;
        public bool statdependant;
        public int statind;

        public void SetReg(Transform source, Transform target)
        {
            affect = target;
            effect = source;
        }

        public bool Active()
        {
            return Activated;
        }

        public bool Useable()
        {
            return Locked;
        }

        public bool IsUnlocked()
        {
            return Unlocked;
        }

        public void Unlock()
        {
            Unlocked = true;
            Locked = false;

        }

        public void TempLock()
        {
            Locked = true;
        }

        public void TempUnlock()
        {
            Locked = false;
        }
        public virtual void AttackMod()
        {
        }

        public virtual void JumpMod()
        {
        }

        public virtual bool CanUseNow()
        {
            return !Locked && Unlocked;
        }

        public bool CanUseNowXOR()
        {
            return CanUseNow() && !Activated;
        }

        public void Use()
        {
            bool b = Unlocked && !Locked;
            if (b)
            {
                if (!Activated)
                {
                    Activated = true;
                    ActivateEffect(affect, effect);
                }
                else if (oneshot)
                {
                    TurnOff();
                }
                RuntimeEffect(affect, effect);

            }
            else
            {
                TurnOff();
            }
        }

        public float TurnOff()
        {
            if (Activated)
            {
                DeactivateEffect(affect, effect);
            }
            Activated = false;
            return absolutecooldown;
        }




       

        public virtual void ActivateEffect(Transform s, Transform t)
        {
            //override...
        }

        public virtual void RuntimeEffect(Transform s, Transform t)
        {
            //override...
        }

        public virtual void DeactivateEffect(Transform s, Transform t)
        {
            //override...
        }
    }

