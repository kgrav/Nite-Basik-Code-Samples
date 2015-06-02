using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Key Terms:
// Connection -
// if the player opens the door from one side of the scene, and passes through
//the threshold between the door's sensory loading zones,
// a "connection" is made.  On a connection, the next zone will be loaded from memory,
//while the current zone is dismissed.
//
//the Transition Zones are trigger zones present in the child
//objects of the transition construct.
//
//The Anterior Zone is the loading zone ("room") the player is currently in.
//The Exterior Zone is the loading zone laying beyond this transition.

[AddComponentMenu("Nite-Basik/Data/Loader Transition Construct")]
    public class LoaderTransitionConstruct : WorldObject
    {
        Door door;
        public int onind, offind, openedfrom;
        public int sig1, sig2;
        bool sig1on, sig2on;
        public int finalresult = -1;
        bool open;

        //Loading Zones represent the zones in front of the door, spanning all possible points the
        //door may be opened from on each repsective side of the room.
        //
        //Entrance into one of these zones cues the loader to load the exterior zone.
        //
        //"Open" the "Door" to the exterior zone. (Class Door may be overridden, in 
        //case of alternate functionality.

        //Opening the door cues the loader to display the exterior zone.
        //If a connection is not made within a few seconds,
        //the door will reset, and the exterior zone will be
        //unloaded.
        //
        //Conversely, if a connection is made, the anterior zone will be unloaded,
        //and the exterior zone will become the next anterior zone.


        public override bool OnInteract(Transform t)
        {
            door.StartMotion();
            openedfrom = offind;
            if (openedfrom == -1)
                openedfrom = 99;
            LoaderZone.DisplayNext();
            open = true;
            transferedZones = false;
            Invoke("ResetDoor", 5);
            return true;
        }
        bool reset = false;
        
        //Executes the Reset function if a connection has not been made.
        
        void ResetDoor()
        {
            if (!transferedZones)
            {
                door.isOpen = true;
                transferedZones = true;
                door.StartMotion();
                resetorload = -1;
                Refresh();
            }
        }
        bool on = false;
        int resetorload;
        //Calls when player enters a zone
        //correllary represents the zone the method was called from.
        public void EnterSig(int correllary)
        {
            //If a double signal, ignore.
            if (correllary == sig1 && sig1on)
            {
                return;
            }
            if (correllary == sig2 && sig2on)
            {
                return; 
            }
            //if not yet opened the door, prepare to load exterior transition zone.
            if (correllary == sig1 && openedfrom == -1)
            {

                sig1on = true;
                onind = sig2;
                resetorload = sig2;
                offind = sig1;
                finalresult = offind;
                if (sig2 != -1)
                {
                    LoaderZone.LoadZone(sig2);
                }
            }
            else if (correllary == sig2 && openedfrom == -1)
            {
                sig2on = true;
                onind = sig1;
                resetorload = sig1;
                offind = sig2;
                finalresult = offind;
                if (sig1 != -1)
                {
                    LoaderZone.LoadZone(sig1);
                }
            }
            else if (openedfrom != -1 && correllary == onind) //if opened the door and entering the exterior zone, confirm and unload the anterior zone.
            {
                door.isOpen = true;
                door.StartMotion();
                if (offind != -1)
                {
                    LoaderZone.TurnOffZone(offind);
                }
                CancelInvoke("Refresh");
                offind = -1;
                onind = -1;
                transferedZones = true;
                openedfrom = -1;
                finalresult = correllary;
                LoaderZone.LockinZone();
            }
        }
        
        bool transferedZones = false;
        void Start()
        {
            openedfrom = -1;
            door = GetComponent<Door>();
            sig1on = false;
            sig2on = false;
        }
        int torefresh = 0;
        void Refresh()
        {
            LoaderZone.NeverMind();
            if (torefresh == sig1)
            {
                sig1on = false;
            }
            else if (torefresh == sig2)
            {
                sig2on = false;
            }
        }
        //if zone was reset while player was standing in it, treat the Stay signal like an Entry signal one time.
        public void StaySig(int correllary)
        {
            if (correllary == sig1)
            {
                if (!sig1on)
                    EnterSig(correllary);
            }
            else if (correllary == sig2)
            {
                if (!sig2on)
                    EnterSig(correllary);
            }
        }
        void RefreshIfNoTransition()
        {

        }

        public void ExitSig(int correllary)
        {
            torefresh = correllary;
            Invoke("Refresh", 1.3f);
        }



    }