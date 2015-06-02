using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
//Base class for equipable weapons
    public class Weapon : MonoBehaviour
    {
        public int WeaponIndex;
        public virtual void SetActivity(MWFLAGS e)
        {
        }
        
        public virtual void OBE()
        {
        }
        public virtual void PersistentEffect()
        {
        }
    }

