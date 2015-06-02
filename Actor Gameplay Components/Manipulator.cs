using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
public enum UMASK { PLYR, DROG, DEUS, ALL, NONE }
[RequireComponent(typeof(Inventory))]
[AddComponentMenu("Nite-Basik/Action Coordination/Main Only/Manipulator")]
    public class Manipulator : MonoBehaviour
    {
        Transform manip, hand;
        public WorldObject inview;
        Item apple;

        public UMASK mask;
        public float CheckFrameRate;
        public float castrad;
        public int framessincecheck;
        public int liftfactor;
        public int dmod;
        public bool Lizs;
        public Transform pointerprobe;
        WorldInteractButton w;
        public bool holding = false;

        void Start()
        {
            CheckView();
            w = pointerprobe.GetComponent<WorldInteractButton>();
        }

        public Item getHeld()
        {
            return apple;
        }

        void GetRefs()
        {
            framessincecheck = 0;
            hand = GetComponentInChildren<Fist>().transform;
            manip = GetComponent<Transform>();
        }

    //Check a sphere for Items and World Objects.  Closest object will be considered 'current'
        void CheckView()
        {
            if (!holding)
            {
                float adist = float.PositiveInfinity;
                if (apple != null)
                    adist = Vector3.Distance(transform.position, apple.transform.position);
                float sdist = float.PositiveInfinity;
                if (inview != null)
                    sdist = Vector3.Distance(transform.position, inview.transform.position);

                Collider[] items = Physics.OverlapSphere(transform.position, castrad);
                for (int i = 0; i < items.Length; ++i)
                {
                    Item m = items[i].GetComponent<Item>();
                    WorldObject uw = items[i].GetComponent<WorldObject>();
                    InspectBody s = items[i].GetComponent<InspectBody>();
                    float distal = Vector3.Distance(transform.position, items[i].transform.position);
                    if (m)
                    {
                        if (distal < adist)
                        {
                            adist = Vector3.Distance(m.transform.position, transform.position);
                            apple = m;
                        }
                    }
                    if (uw && uw.interactive)
                    {
                        if (distal < sdist)
                        {

                            inview = items[i].GetComponent<WorldObject>();
                            sdist = Vector3.Distance(inview.transform.position, transform.position);
                        }
                    }
                }
            }
            Invoke("CheckView", CheckFrameRate);

        }



        public void SendObject(WorldObject w)
        {
           //Dummied out, kept for compiling's sake.
        }

        void Update() //On update, set the Interact icon to hover over 'current' interaction
            //object.  Additionally, clear respective buffer if distance from either the found
            //item, or the found interaction is sufficiently large.3
        {
            GetRefs();
            if (!holding)
            {
                if (apple)
                {
                    w.Position(apple);
                    w.SetOn(true);
                }
                else if (inview)
                {
                    w.Position(inview);
                    w.SetOn(true);
                }
                else
                {
                    w.SetOn(false);
                }
            }
                else
                {
                    w.SetOn(false);
                }

            if (apple != null)
            {
                if (Vector3.Distance(transform.position, apple.transform.position) > castrad*1.5f)
                {
                    apple = null;
                }
            }

            if(inview != null)
            {
                if (Vector3.Distance(transform.position, inview.transform.position) > 20)
                {
                    inview = null;
                }
            }

        }
        bool hzt = false;
        void trigifyHitZones(float howlong)
        {
            if (!hzt)
            {
                HitZone[] h = GetComponentsInChildren<HitZone>();
                foreach (HitZone q in h)
                {
                    q.GetComponent<Collider>().isTrigger = false;
                }
                hzt = true;
                Invoke("untrighitzones", howlong);
            }
        }

        void untrighitzones()
        {
            HitZone[] h = GetComponentsInChildren<HitZone>();
            foreach (HitZone q in h)
            {
                q.GetComponent<Collider>().isTrigger = false;
            }
            hzt = false;
        }
        

        public void ThrowButton()
        {
            trigifyHitZones(0.8f);
            if (apple != null)
            {
                    apple.Throw(dmod * transform.forward + Vector3.up);
                
            }
            holding = false;
            apple = null;
        }
        //Returns animation subsignal (item placement)
        public int interactionButton()
        {
            if (holding && apple != null) //if holding an item, drop the item
            {
                if (apple.IR.collectible) //or, if the item is collectible/permanent, place item into inventory on interact button.
                {
                    GetComponent<Inventory>().Pickup(apple);
                    apple.TearItDown(); //destroy the instance and clear the buffer.
                    apple = null;
                    holding = false;
                }
                else
                {
                    apple.ManualRelease();
                }
                    holding = false;
                
                return -1;
            }
            else if (apple != null) //if we've seen an item,
            {
                print(apple);
                if (apple.OnInteract(hand)) //and the item is only collectible, add item to inventory and destroy physical instance.
                {
                    GetComponent<Inventory>().Pickup(apple);
                    apple.TearItDown();
                }
                else //otherwise, pick up the object.
                {
                    holding = true;
                    apple.transform.localPosition = hand.GetComponent<Fist>().HandOffset - new Vector3(apple.offset.y, 0, 0);
                }
                return apple.height;
            }
            else if (!holding && inview != null) //otherwise, if we're not holding an item and we can see
                //an interactive world object, interact with that object.
            {
                print("Manip used " + inview);
                inview.OnInteract(manip);
                return inview.height;
            }
            else //return 'height' of object (spatial information -
                //0 - eye-level,
                //1 - small object on ground,
                //2 - heavy object on ground.
                
                return -1;
        }

    }

