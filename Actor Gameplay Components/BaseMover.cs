using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Base class for semi-automated motion functionality.
    public class BaseMover : MonoBehaviour
    {
        public virtual void Recoil(Vector3 dir, float amnt)
        {
        }

        public virtual void Reflect(Vector3 dir, float amnt)
        {
        }

        public virtual void HitWithAtk(Vector3 dir, float amnt)
        {
        }

        public virtual void PAUSE()
        {
        }

        public virtual Vector3 LocalForward()
        {
            return transform.forward;
        }


        public virtual void UNPAUSE()
        {
        }

        public virtual bool OnGround()
        {
            return false;
        }

        public virtual bool Motion()
        {
            return false;
        }
    
        
    }