using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[AddComponentMenu("Lady Stardust/Data/Liz Loader")]
    public class LizLoader : MonoBehaviour
    {
        static LizLoader Current;
        public static Liz staticref;
        public Vector3 InitPoint;

        void Start()
        {
            Current = this;
            if (staticref != null)
            {
                staticref.SetCheckPoint(InitPoint);
                staticref.Respawn();
            }
        }

        public static void Respawn()
        {
            staticref.Respawn();
        }
    }

