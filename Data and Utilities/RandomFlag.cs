using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public struct RandomFlag
    {
        float size;
        float percent;

        public RandomFlag(float s, float p)
        {
            size = s;
            percent = p * s;
        }

        public bool Sample()
        {
            float g = UnityEngine.Random.Range(0.0f, size);
            return g < percent;
        }
    }

