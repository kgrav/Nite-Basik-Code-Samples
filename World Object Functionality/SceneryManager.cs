using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
[AddComponentMenu("Nite-Basik/Mechanical Constructs/Scenery Manager")]
public class SceneryManager : MonoBehaviour
{
    public TransforMatrix[] posted;
    public int location;
    bool init = false;
    public Transform FocalObject;
    
    public Vector2[] layers;
    public DynamicScenery[][,] lays;
    int groupsize, currsize;
    void Start()
    {
        groupsize = GetComponentsInChildren<DynamicScenery>().Length;
        currsize = 0;
    }
    void Init()
    {

        lays = new DynamicScenery[layers.Length][,];
        posted = new TransforMatrix[layers.Length];

        for (int i = 0; i < layers.Length; ++i)
        {
            posted[i] = new TransforMatrix(this, i, 1);
            int xx = (int)layers[i].x;
            int yy = (int)layers[i].y;
            lays[i] = new DynamicScenery[xx, yy];


        }
        init = true;
    }
    public void PostMatrix(int layer, TransforMatrix m)
    {
        foreach (Vector2 v in m.bits)
            print(layer + ": " + v + ">" + m.reverse[(int)v.x, (int)v.y]);
        posted[layer] = m;
    }

    void Update()
    {
        
    }

    public Transform GetPiece(int l, int x, int y)
    {
        return lays[l][x, y].transform;
    }

    public void RollCall(int layer, int x, int y, DynamicScenery i)
    {
        if (!init)
        {
            Init();
        }
        lays[layer][x, y] = i;
    }

    public void DisplaceLayer(Vector2[] tiles, int lay, Transformation t)
    {
        foreach (Vector2 v in tiles)
        {
            if (lays[lay][(int)v.x, (int)v.y] != null)
                lays[lay][(int)v.x, (int)v.y].SetTransformation(t);
        }
    }
    bool DDU = false;
    public bool MFR = false;

    public void Place(Vector3 v)
    {
        transform.position = v;
        MFR = false;
    }
    int dduptra, dduptrb;
    public void ResetLayer(int layer)
    {
        for (int i = 0; i < lays[layer].Length; ++i)
        {
            for (int j = 0; j < (int)layers[layer].x; ++j)
            {
                lays[layer][i, j].SoftReset();
            }
        }
    }

    public void HARDResetLayer(int layer)
    {
        foreach (DynamicScenery d in lays[layer])
        {
            d.HardReset();
        }
    }
    public void Deactivate()
    {
        DDU = true;
        dduptra = 0;
        dduptrb = 0;
        MFR = true;
    }


}