using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//key words: 
//Room -
//a partition of physical space within a Scene.
//a Scene is a larger map, wherein all static geometry
//will be loaded at once.  A Scene is split up into 
//several Rooms (connected zones), which control the AI,
//sound, and rendering of active objects contained
//within the zone.
//
//Less off-screen activity means more complex on-screen action,
//better performance, and less inter-scene loading.
//
//Loading uses a pattern similar to Context Bodies,
//Objects within a loading group will turn themselves off individually
//when their loading zone is turned off.
//Conversely, when a Zone comes back on, its comprising Game Objects will turn
//themselves back on automatically.
//This distributed approach decreases the memory overhead,
//amd prevents the need for large loops during gameplay.
//
//Components that check activity status do not turn off,
//rather they revert to an abbrebiated alternative update loop.
//The network of loaders, zones, and objects self-populates at runtime
//Objects will save their state locally,
//and turn off expensive components such as Colliders,
//Rigidbodies, Pathfinders, Renderers, Movers, and Lights.
//For AI Characters, they will check the state of their zone every frame,
//but will not execute any other logic while not loaded, aside from 
//extensions to the DeactiveFunction method.

//Elements such as Audio Masks, Mechanical Constructs, Particle Systems, Lights,
//Trigger Zones, and other room-level phenomena should be included within the 'AudioZone' and 'ZoneStruct' classes.
//AudioZones are integrated with the Audio Mask system (a much better solution than the old Audio Animatronics system)
//ZoneStructs will, by default, turn off colliders, particle systems, lights, and magnetic poles.
//They made be overridden, if more functionality is desired, and less functionality will not cause error.
    
//Zones are numbered manu7ally in-editor.  It is critical that each zone has a unique number, and
//it is desired that the string of numbers be a sequence.

[AddComponentMenu("Nite-Basik/Data/Top Level Loader Zone")]
public class LoaderZone : MonoBehaviour
    {
        static LoaderZone[] SceneZones;
        static bool init = false;
        static int current = -1;
        static int next = -1;
    //Display the buffered Loading Zone
        public static void DisplayNext()
        {
            if (next != current && next != -1)
            {
                SceneZones[next].Display();
                print("Displaying Zone " + next);
            }
        }
    //Confirm that the buffered loading zone has been entered
    //Turn off previous loading zone.
        public static void LockinZone()
        {
            if (next != -1)
            {
                print("In Zone " + next);
                if (current!=-1 && SceneZones[current].on && SceneZones[current].AutoDelouse)
                {
                    SceneZones[current].SwitchOff();
                }
                current = next;
                next = -1;
                SceneZones[current].LockIn();
            }
        }
    //De-Buffer the buffered loading zone.
        public static void NeverMind()
        {
            if (next != current && next != -1)
            {
                SceneZones[next].SwitchOff();
                next = -1;
            }
        }
    //Buffer Zone i
        public static void LoadZone(int i)
        {
            print("Loading Zone " + next);
            SceneZones[i].SwitchOn();
            next = i;
        }
        //Turn off Zone i
        public static void TurnOffZone(int i)
        {
            if (SceneZones[i].on)
            {
                print("turning off zone " + i);
                SceneZones[i].SwitchOff();
            }

        }



        public bool AutoDelouse;
        public int dzone;
        bool halfway = false;
        AudioZone azone;
        ZoneStruct fzone;
        public int zoneID;
        public int ZonesInScene;
        bool on;
        void SwitchOff()
        {
            on = false;
            AILoader.UnLoadZone(dzone);
            if(fzone)
            fzone.Deload();
        }

        void SwitchOn()
        {
            if (azone) 
            azone.Load();
            if(fzone)
            fzone.Load();
        }

        void LockIn()
        {
            print(azone);
            if(azone)
            azone.Play();
        }

        void Display()
        {
            on = true;
            if(fzone)
            fzone.Display();
           AILoader.LoadZone(dzone);

        }
        //This construct turns off and on the audio, AI, and physical forces of the loading zone
    //
        void Start()
        {
            on = false;
            if (!init)
            {
                SceneZones = new LoaderZone[ZonesInScene];
                init = true;
            }
            SceneZones[zoneID] = this;
            azone = GetComponent<AudioZone>();

            if (!azone)
                azone = GetComponentInChildren<AudioZone>();
            fzone = GetComponent<ZoneStruct>();
            if (!fzone)
                fzone = GetComponentInChildren<ZoneStruct>();
        }

    }

