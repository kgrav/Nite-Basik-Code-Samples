using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[AddComponentMenu("Nite-Basik/Physical Objects/Static Geometry")]
public class StaticWorldObject : WorldObject
{
    //Base level static world objects; use this, as it sets up data for the object.
    void Start()
    {
        SetupData();
    }
}

