using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
//Base class for switches and the like, 
//override 'Activate' for more functionality.
//Rarely used.
    public class UseableWorldObject : WorldObject
    {
        public UMASK prohibited;

        public override bool OnInteract(Transform t)
        {
            Activate();
            return true;
        }

        public virtual  void Activate()
        { }
    }

