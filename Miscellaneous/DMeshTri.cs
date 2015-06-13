using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class DMeshTri
    {
        int[] Vertices;
        int Index;
        public DMeshTri(int triind, int[] verts)
        {
            Index = triind;
            Vertices = verts;
        }
    }