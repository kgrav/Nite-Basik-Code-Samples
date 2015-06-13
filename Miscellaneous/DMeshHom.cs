using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Homonym of a Deformable Mesh Vertex.
//'Homonyms' are copies of vertices that occupy the same
//space at a different index than the first found point
//at that space.  "Domain" is the halfway vector between 
//the vertex's two edge connections, and represents the 
//general 'direction' followed by this particular homonym.

//This is used to check which vertices to separate if splitting along a plane.
//To split along a plane, check each hom's angle relative to thesplit angle rotated 90 degrees 
//around the point's normal axis.
//If the angle between them is > 90, it is on one side of the cut, and if the angle is < 90,
//it is on the other side.  Just picture it in your head, it makes perfect sense.

    public struct DMeshHom
    {
        public Vector3 domain;
        public int vIndex, e1index, e2index;

        public DMeshHom(Vector3 p1, Vector3 p2, Vector3 h, int vi, int e1, int e2)
        {
            domain = ((p1 -h) + (p2-h)).normalized;
            vIndex = vi;
            e1index = e1;
            e2index = e2;
        }

    }

