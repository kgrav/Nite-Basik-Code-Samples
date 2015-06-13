using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class DMeshVert
    {
        
        public int VertIndex;
        public DMeshHom[] SurfaceNormals;
        public int[] Homonyms;
        public int[] ConnectedTo;
        public int[] partOfTris;
        public int maxC, maxT, maxH, maxN;
        public Color dbc;
        public int maxDeltaH;
        DeformableMesh parentRef;


        public DMeshVert(int ind, DeformableMesh pr)
        {
            parentRef = pr;
            maxC = 0;
            maxT = 0;
            maxH = 0;
            maxN = 0;
            SurfaceNormals = new DMeshHom[32];
            ConnectedTo = new int[32];
            Homonyms = new int[64];
            partOfTris = new int[32];
            VertIndex = ind;
        }

        public int[] GetSet()
        {
            List<int> r = new List<int>();
            r.Add(VertIndex);
            for (int i = 0; i < maxH; ++i)
            {
                r.Add(Homonyms[i]);
            }
            return r.ToArray();
        }

        public void AddHom(int x)
        {
            bool add = maxH < 64;
            for (int i = 0; i < maxH; ++i)
            {
                if (Homonyms[i] == x)
                    add = false;
            }
            if (add)
            {
                Homonyms[maxH] = x;
                maxH++;
            }
        }

        public void AddNormals(int[] tri, int mind, int exc)
        {
            bool add = (maxN < 32);
            int which;
            int o1 = -1, o2 = -1;
                for (int q = 0; q < maxN; ++q)
                {
                    if (SurfaceNormals[q].vIndex == tri[exc])
                    {
                        add = false;
                    }
                }
            if(add){
                which = mind;
                for (int i = 0; i < tri.Length; ++i)
                {
                    if (i != exc && o1 == -1)
                        o1 = tri[i];
                    else if (i != exc && o2 == -1)
                        o2 = tri[i];
                }
                SurfaceNormals[maxN] = new DMeshHom(parentRef.verts[o1], parentRef.verts[o2], parentRef.verts[VertIndex], mind, o1, o2);
            }
        }

        public void SendConnection(int connection)
        {
            bool add = (maxC < 32);
            for (int i = 0; i < maxC; ++i)
            {
                if (ConnectedTo[i] == connection)
                    add = false;
            }
            if (add)
            {
                ConnectedTo[maxC] = connection;
                maxC++;
            }
        }

        public void SendTri(int[] tri, int triindex, int[] homtable)
        {
            if (triindex < 16)
            {
                partOfTris[maxT] = triindex;
                maxT++;
            }
            
            for (int y = 0; y < tri.Length; ++y)
            {
                int z = homtable[tri[y]];
                if (z != VertIndex)
                {

                    SendConnection(z);

                }
                else if(z == VertIndex)
                {
                    AddNormals(tri, tri[y], y);
                }
            }
        }
    }
