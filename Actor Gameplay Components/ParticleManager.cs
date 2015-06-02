using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[AddComponentMenu("Nite-Basik/Action Coordination/Everyone/ParticleManager")]
    public class ParticleManager : MonoBehaviour
    {
        public int numofchildren;
        ParticleChild[] chillens;
        bool init = false;
        int activ = -1;
        public void RollCall(int system, ParticleChild child)
        {
            if (!init)
            {
                chillens = new ParticleChild[numofchildren];
                init = true;
            }

            chillens[system] = child;
        }


        public void Set(int i)
        {
            activ = i;
        }

        public void TurnOn()
        {
            if(activ != -1)
                chillens[activ].SwitchOn();
        }
        public void TurnOff(int i)
        {
            chillens[i].SwitchOff();
        }
        public void TurnOn(int i)
        {
            chillens[i].SwitchOn();
        }
        public void TurnOff()
        {
            if (activ != -1)
                chillens[activ].SwitchOff();
        }
    }