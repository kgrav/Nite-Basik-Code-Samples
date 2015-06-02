using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
//Scenery which can be transformed in-game.
//Can be set directly, but a better method is to
//post a transformation matrix to the appropriate layer
//in the parent SceneryManager.
[AddComponentMenu("Nite-Basik/Physical Objects/Managed Scenery")]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class DynamicScenery : WorldObject
{
    public bool UTF;
    public int Type, GridX, GridY;
    public Vector3 Mot;
    Vector3 origin, defsize;
    Quaternion orot;
    Transformation tcurr;
    SceneryManager m;
    int phase = -1;

    public void SetTransformation(Transformation t)
    {
        tcurr = t;
        phase = 0;
    }

    public void SoftReset()
    {
        if (phase == 1)
            phase = 2;
        else
            phase = -1;
    }
    public void HardReset()
    {
        phase = -1;
        transform.position = origin;
        transform.localScale = defsize;
        transform.rotation = orot;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }

    void Start()
    {
        m = GetComponentInParent<SceneryManager>();
        m.RollCall(Type, GridX, GridY, this);
        origin = transform.position;
        defsize = transform.localScale;
        orot = transform.rotation;
    }

    void Update()
    { //In upddate, if not transforming, check for transformations, and execute them.
        if (m != null)
        {
            if (m.posted[Type].reverse[GridX, GridY] != -1)
            {
                if (m.posted[Type].reverse[GridX, GridY] == -2)
                {
                    SoftReset();
                }
                else
                {
                    if (phase != -1)
                    {
                        HardReset();
                    }

                    SetTransformation(m.posted[Type].correllary[m.posted[Type].reverse[GridX, GridY]]);
                }
                
                m.posted[Type].reverse[GridY, GridX] = -1;
            }
        }
        switch (phase)
        {
            case 0:
                bool b = tcurr.Exec(transform);
                if (b)
                    phase = 1;
                break;
            case 2:
                bool c = tcurr.ExecInverse(transform);
                if (c)
                    phase = -1;
                break;
        }
    }
}