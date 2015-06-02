using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//"Droog" is shorthand for "Instance-Specific AI Controlled Enemy",
//Manages entities which extend the AI character script,
//by their scene-level context.
//Derived from the Welsh word "drwg", meaning "bad," or "evil."
//For an unambiguous term to refer to non-boss, non-player, and non-story-critical characters.

[AddComponentMenu("Nite-Basik/Data/Droogs Zone")]
    public class Droogs : MonoBehaviour
    {


        //Table of contexts in zone
        public int[,] group;

        //Which context groups are active in this group, and which can be skipped?
        public bool[] active;

        //What is the maximum index of each context group
        public int[] maxes;
        public int droogsInScene;
        //GameObject[] copier;
        public Vector3 Center;
        public float MaxDistance;
        public float OffScreenY;
        public int ZoneID;
        public int activeCount;
        int population;
        bool ison;
        bool isReady;
        void Start()
        {
            ExtensionInit();
            AILoader.RollCall(this, droogsInScene);
            ison = false;
            group = new int[droogsInScene, 200];
            active = new bool[droogsInScene];
            maxes = new int[droogsInScene];
            for (int i = 0; i < droogsInScene; ++i)
            {
                active[i] = false;
                maxes[i] = 0;
            }
            //copier = new GameObject[droogsInScene];
            Collider[] cs = Physics.OverlapSphere(Center, MaxDistance);
            population = 0;
            activeCount = 0;
            foreach (Collider c in cs)
            {
                int context = ContextBody.GroupOf(c.transform);
                if (context > 0)
                {
                    int ind = ContextBody.IndexOf(c.transform);
                    AICharacter a = c.GetComponent<AICharacter>();
                    if(a){
                    if (!active[context])
                    {
                        //if this is the first AI Character of this type 
                        //copier[context] = (GameObject)GameObject.Instantiate(c.gameObject, new Vector3(0, OffScreenY, 0), Quaternion.identity);
                        //copier[context].GetComponent<Renderer>().enabled = false;
                        //copier[context].GetComponent<Rigidbody>().isKinematic = true;
                        //copier[context].GetComponent<Rigidbody>().useGravity = false;
                        //copier[context].GetComponent<AICharacter>().SetActive(false);
                        active[context] = true;
                    }
                    a.SetZone(this);
                    population++;
                    a.SetActive(false);
                    }
                }
            }
        }

        public void TurnOn()
        {
            ison = true;
            OnSwitch();
        }

        protected virtual void OnSwitch()
        {

        }

        public void TurnOff()
        {
            ison = false;
            OffSwitch();
        }

        protected virtual void OffSwitch()
        {
        }

        public bool IsActive()
        {
            return ison;
        }
        protected virtual void ExtensionInit()
        {
            //Spawn all droogs in overrides of this method, at Scene start.
            //Allocating every droog in an area of the castle
            //Has a bigger space/memory overhead, 
            //but is easier on the CPU (i.e. some loading, but no performance lags, within reason,
            //try to control the amount of droogs within a single room,
            //as each object within a Loading Group will be active at the same time.
        }
    }

