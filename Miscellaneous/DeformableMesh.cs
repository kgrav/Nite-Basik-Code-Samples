using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Key terms:
//Flex Point - a point selected pseudo-randomly (based on inter-set distance), 
//part of a set that serves as an entry point to whole-mesh searches.
//
//Root Platonic - the sum of all Vertices found in one of the Mesh's Vertex Spaces.
//
//Vertex Space - a single point in space that is occupied by one or more triangle vertices.
//
//At the start, this class bakes all vertices occupying each vertex space into consolidated
//'DMeshVert' Root Platonic structures, which function as vertices would in a modeling application. 
//Since all vertices at a point are stored in a single index, connections between these vertex 
//groups can be obtained from the triangle data.  This creates a lattice structure, over which
//the mesh may be deformed without breakage.
//
//Unlike Unity's built-in ClosestTriangle method, this does not require a Mesh Collider (although this is desired for
//the most high-quality visual effect), and can be determined from normal collisions (not just raycasts).  Additionally, it returns
//all vertices occupying the same space, not just the vertices of one triangle.
//
//The 'Flex Point Density' variable controls the relative amount of Flex Points in the mesh.
//A very low number will result in inaccurate real-time calculation, but very fast baking.
//A higher number will result in faster real-time calculation, but slower baking.
//Too high though, and the advantage of using Flex Points to begin with (to reduce # of 
//vertex comparisons per scan) starts to wear off.

[AddComponentMenu("Nite-Basik/Mechanical Constructs/Deformable Mesh")]
    public class DeformableMesh : MonoBehaviour
    {

        int[] DVTable;
        int[] DVHoms;
        int dvtp = 0, dvsp = 0;
        
        public float FlexPointDensity;
        public float MaxDisplacement;
    public float DisplacementFalloff;
        public float malleability;
        MeshCollider mX;
        public Vector3[] verts;
        Vector3[] originalVerts;
        List<int> FlexPoints;
        AudioSource dent;
        public AudioClip harvey;
        //FIt does not matter which vertices are chosen as flex points.
        //It only matters that the distance between them is sufficiently large.
        //These points create an abbreviated vertexc search list from which to enter
        //the array.
        int[] tris;
        MeshFilter reder;
        Mesh buff;
        float flexDist;
        List<DMeshVert> DVerts; //These vertices store their connections to other vertices, and the triangles they lie on.  These
        //are purely pointer structures; they store only the indexes of vertices.  Locations are stored in 'verts'
        //Since we're just deforming the mesh rather than degenerating/breaking it, we only need to compute
        //these values once, at startup.
        DMeshTri[] DTris;


        

        public virtual void ApplyForce(Vector3 dir, Vector3 pos, float amnt)
        {
            int q = EstimateVertex(pos);
            DirectDeform(q, dir, amnt);
            buff.vertices = verts;
            safety = 0;
            reder.mesh = buff;
            
            dent.PlayOneShot(harvey);
        }
        int safety = 0;
        protected virtual void DirectDeform(int pt, Vector3 dir, float forc)
        {
            safety++;
            if (safety > 100 || forc < 0.1f)
                return;
            Vector3 v = verts[DVerts[pt].VertIndex];
            int[] cSet = DVerts[pt].ConnectedTo;
            int cMax = DVerts[pt].maxC;
            Vector3 d = transform.InverseTransformDirection((transform.TransformPoint(v)).normalized);
            Vector3 rdir = transform.InverseTransformDirection(dir);
            int[] set = DVerts[pt].GetSet();

            bool spinFriends = false;
            float magstor = 0;
            foreach (int qq in set)
            {
                verts[qq] += rdir * forc;

                if ((verts[qq] - originalVerts[qq]).magnitude > MaxDisplacement) //if any vertices are more displaced from their starting position than the maximum displacement, also displace connected vertices in the same direction
                {
                    spinFriends = true; magstor = (verts[qq] - originalVerts[qq]).magnitude;
                }
            }
            if (spinFriends)
            {
                for (int i = 0; i < cMax; ++i)
                {
                    float mag1 = (verts[cSet[i]] - originalVerts[cSet[i]]).magnitude; //only displace a vertex if it is of a lower total displacement (prevents bottomless recursion,
                    //without this check, it would affect the vertex that called it...forever)
                    if (mag1 < magstor)
                    {
                        DirectDeform(cSet[i], dir, forc * DisplacementFalloff);
                    }
                }
            }
        }

        

        int EstimateVertex(Vector3 point)
        {
            Vector3 lpt = transform.InverseTransformPoint(point);
            float min = float.PositiveInfinity;
            int closestflex = 0;
            int secondclosestflex = 0;
            int closestpoint = -1;
            //First find the closest Flex Point.
            for (int i = 0; i < FlexPoints.Count; ++i)
            {
                int aver = FlexPoints[i];
                Debug.DrawLine(transform.TransformPoint(lpt), transform.TransformPoint(verts[aver]), Color.yellow, 20.0f);
                Vector3 vega = verts[aver] - lpt;
                float f1 = vega.magnitude;
                if (f1 < min)
                {
                    print(min);
                    min = f1;
                    closestflex = i;
                }
            }
            Debug.DrawRay(transform.TransformPoint(verts[FlexPoints[closestflex]]), transform.TransformDirection(verts[(FlexPoints[closestflex])]) * 5, Color.cyan, 3.0f);
            int wpt = AbsoluteVertex(FlexPoints[closestflex]);
            //After finding the closest Flex Point
            int iterations = 0;
            while (closestpoint == -1)
            {
                iterations++;
                if (iterations > 100)
                {
                    print("overload!");
                    closestpoint = wpt;
                }
                int ii = DVerts[wpt].maxC;
                int[] ji = DVerts[wpt].ConnectedTo;
                print(ii);
                bool foundCloserPoint = false;
                int lmin = DVerts[wpt].VertIndex;

                float fmin = Vector3.Distance(lpt, verts[lmin]);
                for (int i = 0; i < ii; ++i)
                {
                    int jpt = DVerts[ji[i]].VertIndex;
                    float t = Vector3.Distance(lpt, verts[jpt]);

                    Debug.DrawLine(transform.TransformPoint(verts[jpt]), transform.TransformPoint(verts[lmin]), Color.green, 3.0f);
                    if (t < fmin)
                    {
                        foundCloserPoint = true;
                        fmin = t;
                        wpt = ji[i];
                    }
                }
                
                if (!foundCloserPoint)
                    closestpoint = wpt;
            }
            //Debug.DrawLine(transform.position + transform.localRotation*verts[closestpoint], transform.position + transform.localRotation*verts[closestpoint] * 4, Color.red, 5.0f);
            return closestpoint;
        }
        void Start()
        {
            mX = GetComponent<MeshCollider>();
            dent = GetComponent < AudioSource > ();
            flexDist = GetComponent<Renderer>().bounds.size.magnitude/FlexPointDensity;
            reder = GetComponent<MeshFilter>();
            buff = reder.mesh;
            DTris = new DMeshTri[buff.triangles.Length/3 + 1];
            verts = buff.vertices;
            originalVerts = buff.vertices;
            tris = buff.triangles;
            FlexPoints = new List<int>();
            DVerts = new List<DMeshVert>();
            DVTable = new int[buff.vertexCount];
            DVHoms = new int[buff.vertexCount];
            int dvp = 0;
            //At the start, cache the mesh's Flex Points and Edges.
            //This saves a lot of CPU usage & RAM in the long run,
            //since each deformation affects the same set of pre-initialized data,
            //although pre-level load times may be lenghtened as a result.
            //
            //The way Unity loads meshes is to load each vertext once for each triangle,
            //rather than having triangles share vertices.
            //Because of this, every Vertex sharing a Point in space will be
            //Added to the same structure.
            for (int i = 0; i < verts.Length; ++i)
            {
                bool addDV = true;
                for (int j = 0; j < dvtp; ++j)
                {
                    //If this Vertex occupies the same space as a previously traversed vertex,
                    //Add this vertex as a Homonym of the first Vertex occupying the space given.
                    if (verts[i].Equals(verts[DVerts[DVTable[j]].VertIndex]))
                    {
                        addDV = false;
                        DVHoms[i] = j;
                        DVerts[DVTable[j]].AddHom(i);
                        break;
                    }

                }
                if (addDV)
                {
                    //Otherwise, add this as a new Root Platonic for its space. (Homonyms will be added as they are found)
                    DVerts.Add(new DMeshVert(i, this));
                    DVTable[dvtp] = DVerts.Count - 1;
                    DVHoms[i] = DVerts.Count - 1;
                    dvtp++;
                }
                bool addflex = true;
                foreach (int j in FlexPoints)
                {
                    if (Vector3.Distance(verts[j], verts[i]) < flexDist)
                    {    addflex = false; break;}
                }
                if (addflex)
                {
                    FlexPoints.Add(i);
                    Debug.DrawLine( transform.TransformPoint(verts[i]), transform.TransformPoint(5 * verts[i]), Color.yellow, 10.0f);
                }
            }
            for (int i = 0; i < tris.Length; i += 3)
            {
                int[] currtri = new int[] { tris[i], tris[i + 1], tris[i + 2] };
                DTris[i/3] = new DMeshTri(i, currtri);
                //Send all triangles to the Root Platonic instance of the triangle vertex given (backref through the arrays)
                DVerts[AbsoluteVertex(currtri[0])].SendTri(currtri, i, DVHoms);
                DVerts[AbsoluteVertex(currtri[1])].SendTri(currtri, i, DVHoms);
                DVerts[AbsoluteVertex(currtri[2])].SendTri(currtri, i, DVHoms);
            }
        }
        //find the Root Platonic of a given vertex index.
        int AbsoluteVertex(int i)
        {
            return DVTable[DVHoms[i]];
        }
    }

