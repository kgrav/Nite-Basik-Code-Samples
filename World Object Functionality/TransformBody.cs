using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[AddComponentMenu("Nite-Basik/Mechanical Constructs/Transformation Receiver")]
    public class TransformBody : MonoBehaviour
    {
        Transformation curr;
        int phase;
        bool willinvert;
        void Start()
        {
            curr = null;
            phase = -1;
            willinvert = false;
        }

        void Update()
        {
            if (phase != -1)
            {
                switch (phase)
                {
                    case 0:
                        bool b = curr.Exec(transform);
                        if (b)
                            phase = 1;
                        break;
                    case 1:
                        if (willinvert)
                            phase = 2;
                        else
                        {
                            willinvert = false;
                            phase = -1;
                            curr = null;
                        }
                        break;
                    case 2:
                        bool c = curr.ExecInverse(transform);
                        if (c)
                        {
                            phase = -1;
                            curr = null;
                            willinvert = false;
                        }
                        break;
                }
            }
        }

        public void PushTransformation(Transformation t, bool invert)
        {
            willinvert = true;
            phase = 0;
            curr = t;

        }

    }

