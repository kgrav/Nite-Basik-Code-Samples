using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
//Inspectable world ob+ject.
//A+utomatically detected by the Manip+ulator,
//or, if it is att+ached to an Ite+m,
//+it will be activated when the item is interacted with.

[AddComponentMenu("Nite-Basik/Physical Objects/Inspect Body")]
    public class InspectBody : WorldObject
    {
        public string file;
        public int length;
        string[] tbuffers;
        int[] transitions;
        int ptr = 0;

        public override bool OnInteract(Transform t)
        {
            print("Fired");
            interactfunc(t);
            GUIText.SetText(this);
            return true;
        }

        public string GetBuffer()
        {
            int i = ptr;
            ptr = transitions[ptr];
            if (i == -1)
            {
                ptr = 0;
                return "END";
            }
            return tbuffers[i];
        }
        void Start()
        {
            ReadFile();
            initfunc();
        }
        protected virtual void initfunc()
        {

        }
        protected virtual void interactfunc(Transform t)
        {

        }
        public void ReadFile()
        {
            StreamReader animfile = new StreamReader("Assets/" + file + ".smt");
            string ts = animfile.ReadLine();
            string[] splits = ts.Split(':');
            int phase = 0;
            int frame = -1;
            bool HashTag = false;
            bool end = false;
            while (ts != null){
                switch (phase)
                {
                    case 0:
                        int t1 = Convert.ToInt32(splits[0]);
                        int t2 = Convert.ToInt32(splits[1]);
                        tbuffers = new string[t1];
                        transitions = new int[t1];
                        phase = 1;
                        break;
                    case 1:
                        t1 = Convert.ToInt32(splits[0]);
                        tbuffers[t1] = "";
                        frame = t1;
                        phase = 2;
                        break;
                    case 2:
                        if (ts[0] == '>')
                        {
                            string num = ts[1] + "";
                            transitions[frame] = Convert.ToInt32(num);
                            phase = 1;
                        }
                        else if (ts[0] == '<')
                        {
                            transitions[frame] = -1;
                            phase = 4;
                        }
                        else
                        {
                            tbuffers[frame] += ts.Split('/')[0] + "\n";
                        }
                        break;
                }

                try
                {
                    ts = animfile.ReadLine();
                    splits = ts.Split(':');
                }
                catch (Exception  e)
                {
                    ts = null;
                }
            }
        }


    }

