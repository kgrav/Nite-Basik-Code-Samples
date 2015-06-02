using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[AddComponentMenu("Nite-Basik/Mechanical Constructs/Danger Zone")]
    public class DangerZone : MeleeWeapon
    {
        protected override void ExtUpdate()
        {
            SetActivity(MWFLAGS.ATK);
        }
    }
