using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Nite-Basik/Camera/Target Helper")]
    public class TargetHelper : MonoBehaviour
    {

        //Set THIS as the camera's look point - 
        //moves independently from player, smoothly following its transform,
        //rather than mirroring its location.
        //set 'whos' as the TOP LEVEL (Parent == null) Transform for the player character game object.
        //accomodates perspective while targeting, jumping, and dodging,
        //mainly smooths out camera jerks that would be otherwise present if
        //the camera looked directly at the reasonably fast-moving character.

    //Requires a Directly Controlled Mover in the follow object,
    //This mover is highly versatile, and should suffice as a template for
    //a character in an Adventure, Action, Platforming, RPG, or similarly third person game.
        bool on = false;
        public Transform whos;
       public  Vector3 position;
        public Vector3 goal;
        public Vector3 direction;
        DirectlyControlledMover watch;
        public float speed;
        void Start()
        {

            position = transform.position - whos.position;
            goal = position;
            watch = whos.GetComponent<DirectlyControlledMover>();
            direction = whos.transform.forward * (-1);
        }
        bool RETURNING = false;
        void Update()
        {
            if (!on)
            {
                if (watch.Motion())
                {
                    goal = whos.position - 2 * whos.transform.forward + Vector3.up * 2;
                }
                else
                {
                    goal = whos.position + whos.TransformDirection(position.normalized) + Vector3.up * 2;
                }
            }
            else
            {
                goal = whos.position + whos.forward * (-1)*(0.3f)*Vector3.Distance(TargetArrow.PlayersArrow.transform.position, whos.position);
            }
            direction = (goal - transform.position).normalized;
            float dista = Vector3.Distance(goal, transform.position);
            float smn = speed;
            RaycastHit rch;
            if(dista > 1)
            {
                transform.Translate(direction*(dista)*smn*Time.deltaTime);
            }
        }
        public bool colli = false;
        public void OnTriggerEnter(Collider c)
        {
            if (!ContextBody.IsActor(c.transform)                                                    )
            {
                if(Camera.main.GetComponent<FASTACTIONCAMERA>())
                Camera.main.GetComponent<FASTACTIONCAMERA>().movewith = whos;
                colli = true;
            }

        }

        public void OnTriggerStay(Collider c)
        {
            if (!ContextBody.IsActor(c.transform))
            {
                if (Camera.main.GetComponent<FASTACTIONCAMERA>())
                    Camera.main.GetComponent<FASTACTIONCAMERA>().movewith = whos;
                colli = true;
            }
        }
        
        public void OnTriggerExit(Collider c)
        {
            if (!ContextBody.IsActor(c.transform))
            {
                if (Camera.main.GetComponent<FASTACTIONCAMERA>())
                    Camera.main.GetComponent<FASTACTIONCAMERA>().movewith = this.transform;
                colli = false;
            }
        }
        public void SetActivity(bool b)
        {
            on = b;
        }
    }

