using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Load correllated Audio Mask when zone is pre-loaded, fade to mask when room is entered.
[AddComponentMenu("Nite-Basik/Data/Audio Fade Zone")]
    public class AudioZone : MonoBehaviour
    {
        public static int LoadedMask = -1;
        public int zoneKey;
        public int Mask;
        public void Load()
        {
            if (zoneKey != LoadedMask)
            {
                AudioSwitcher.Statref.LoadMask(Mask);
                LoadedMask = zoneKey;
            }
        }

        public void Play(){
            AudioSwitcher.Statref.SwitchToLoadedMask();
        }

        
    }

