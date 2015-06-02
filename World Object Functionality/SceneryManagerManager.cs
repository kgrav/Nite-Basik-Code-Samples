using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[AddComponentMenu("Nite-Basik/Mechanical Constructs/Scenery Manager Manager")]
    class SceneryManagerManager : MonoBehaviour
    {
        public static SceneryManager[] DynamicRooms;
        static int rptr = 0;
        public int RoomsInScene;
        public Transform BackEnd, FrontEnd;
        Vector3 offset;
        Vector3 qend;

        void Start()
        {
            DynamicRooms = new SceneryManager[RoomsInScene];
            SceneryManager[] ss = GetComponentsInChildren<SceneryManager>();

            int lprev = -1;
            foreach (SceneryManager s in ss)
            {
                if (s.location > lprev)
                {
                    if (lprev != -1)
                    {
                        offset = s.transform.localPosition - qend;
                    }
                    lprev = s.location;
                    rptr = lprev;
                    qend = s.transform.localPosition;
                    
                }
                DynamicRooms[s.location] = s;
            }
            
        }

        void Update()
        {
        }



        public void SnakeRooms()
        {
            SceneryManager t = DynamicRooms[0];
            for (int i = 0; i < RoomsInScene  - 1; ++i)
            {
                DynamicRooms[i] = DynamicRooms[i + 1];
            }
            DynamicRooms[RoomsInScene - 1] = t;
            t.transform.localPosition = qend + offset;
            qend = t.transform.localPosition;
        }



    }
