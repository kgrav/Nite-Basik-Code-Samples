using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Similar to the GUI sound structure, this uses
//a revolving queue of Audio Sources to provide World Object sound effects.
//World Objects will call to this on collision events,
//And this will send one of its Audio Sources to the point
//Where the collision is made, ensuring proper positioning in 3D space,
//as well as adding an extra layer of character to the world,
//with a solution that is optimal for performance,
//as, like with the Sound Table, it allows for an indefinite number
//of audio-producing entities, while only loading one copy of the needed clips into memory (declared in a manifest),
//And maintaining a constant number of Audio Sources (rather than one for each world object!).
    public class WorldAudio : MonoBehaviour
    {

        static WorldAudioUnit[] UNITS;

        static int tabctx;
        static int count;
        static int uptr;
        static bool init = false;


        public static void PlaySound(Vector3 where, int which)
        {
            if (init)
            {
                UNITS[uptr].Call(where, SoundTable.GetSound(tabctx, which));
                uptr++;
                if (uptr >= count)
                    uptr = 0;
            }
        }

        public int SoundContext; //Table index used by world objects.
        public int MaxDensity; //the total number of Audio Sources used in scene.
        public GameObject prefab;

        void Start()
        {
            UNITS = new WorldAudioUnit[MaxDensity];
            count = MaxDensity;
            tabctx = SoundContext;
            uptr = 0;
            for (int i = 0; i < MaxDensity; ++i)
            {
                GameObject gq = (GameObject)GameObject.Instantiate(prefab);
                UNITS[i] = gq.GetComponent<WorldAudioUnit>();
            }
            init = true;
        }

    }

