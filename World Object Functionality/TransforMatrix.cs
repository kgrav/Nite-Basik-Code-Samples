using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public enum AXIS2D { X, Y, CROSS };
    public struct TransforMatrix
    {
        public static TransforMatrix Inverse(TransforMatrix a)
        {
            TransforMatrix r = a;
            for (int i = 0; i < r.cptr; ++i)
            {
                r.reverse[(int)r.bits[i].x, (int)r.bits[i].y] = -2;
            }
            return r;
        }

        public int size, h, w;
        public int count, cptr;
        public Vector2[] bits;
        public int[,] reverse;
        public Transformation[] correllary;

        public TransforMatrix(Vector2 s, Vector2[] bi, Transformation[] ci)
        {
            size = (int)(s.x*s.y);
            h = (int)s.x;
            w = (int)s.y;
            reverse = new int[(int)s.x, (int)s.y];
            correllary = ci;
            count = bi.Length;
            cptr = bi.Length;
            bits = bi;
            for (int i = 0; i < (int)s.x; ++i)
            {
                for(int j = 0; j < (int)s.y; ++j)
                {
                    reverse[i, j] = -1;
                }
            }
            for(int i = 0; i < bits.Length; ++i)
            {
                Vector2 v = bits[i];
                reverse[(int)v.x, (int)v.y] = i;
                
            }
        }

        public TransforMatrix(SceneryManager s, int layer, int n)
        {
            Vector2 v = s.layers[layer];
            count = n;
            cptr = 0;
            size = (int)(v.x * v.y);
            h = (int)v.x + 1;
            w = (int)v.y + 1;
            reverse = new int[h, w];
            correllary = new Transformation[n];
            
            bits = new Vector2[n];
            for (int i = 0; i < (int)v.x; ++i)
            {
                for (int j = 0; j < (int)v.y; ++j)
                {
                    reverse[i, j] = -1;
                    
                }
            }
        }
        public string printMat()
        {
            return h + ", " + w;
        }
        

        public string PushAxis(Transformation t, int i, AXIS2D ax)
        {
            if (ax == AXIS2D.CROSS || ax == AXIS2D.X)
            {
                for (int j = 0; j < w; ++j)
                {
                    if (reverse[i, j] != -1)
                    {
                        int hella = reverse[i, j];
                        correllary[hella] = t;
                    }
                    else
                    {
                        bits[cptr] = new Vector2(j, i);
                        reverse[i, j] = cptr;
                        correllary[cptr] = t;
                        ++cptr;
                    }
                }

            }

            if (ax == AXIS2D.CROSS || ax == AXIS2D.Y)
            {
                for (int j = 0; j < h; ++j)
                {
                    if (reverse[i, j] != -1)
                    {
                        int rad = reverse[i, j];
                        correllary[rad] = t;
                    }
                    else
                    {
                        bits[cptr] = new Vector2(i, j);
                        reverse[i, j] = cptr;
                        correllary[cptr] = t;
                        ++cptr;
                    }
                }
            }
            return h + ", " + w + ":" + i;
        }

        public string PushTransformation(Transformation t, int x, int y)
        {
            if(cptr < count)
            {
                bits[cptr] = new Vector2(x, y);
                correllary[cptr] = t;
                reverse[x, y] = cptr;
                cptr++;
            }
            return h + ", " + w + ":" + x + ", " + y;
        }
    }

