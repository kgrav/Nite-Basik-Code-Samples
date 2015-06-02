using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//
//If a standard Action Coordination Component is attached to
//a GameObject with an AI Character (or deriving class), it will automatically
//call these methods automatically.  The functionality of called methods
//can be defined within the AI character's specific script.
//This holds common functionality for combat, motion, and state.
//If an action component is attached to a directly controlled character, or a world object,
//the AI Character methods will not be called
//
//AI Header Scripts derive from this class.  Header scripts express functionality
//specific to the design of the Character they describe, and coordinate the 
//actions of the Action Coordination components which communicate through the methods
//contained below.
//
//This creates a sort of standardized internal "language" for AI characters to think
//and act within.
public class AICharacter : MonoBehaviour
    {
        //Static method which calls "alarm" function for all AI Characters in a sphere.
        public static void SoundAlarm(Vector3 position, float propagation)
        {
            Collider[] cs = Physics.OverlapSphere(position, propagation);
            foreach (Collider c in cs)
            {
                AICharacter ai = c.GetComponent<AICharacter>();
                if (ai)
                    ai.Alarm();
            }
        }
        public int LoadingGroup;
        Droogs zone;
        ContextBody cbx;

        public bool animated;
        public float initDelay;
        public bool active;
        protected int poshits;
        protected int neghits;
        void Start()
        {
            //On scene start, roll call to the AI Loader and call the initialization function..
            AILoader.totalDroogs++;
            cbx = GetComponent<ContextBody>();
            if (initDelay > 0)
            {
                Invoke("InitFunc", initDelay);
            }
            else
            {
                InitFunc();
            }
        }
        //Calls from mover when motion has stopped, and it is safe to perform an
    //action which requires the character to stand still.  This is only necessary for
    //character which use physics-based motion, such as William.  Conversely,
    //Character-Controller based motion, such as that used by the Homunculus,
    //can be stopped more easily through the header script.
        public virtual void MoverSafe()
        {
        }
        //Calls from combat interface when weapon can be used (after being used).
        public virtual void OnWeaponCooldown()
        {
        }
        
        public virtual void Distract(Vector3 location)
        {
            
        }

        //Calls when directly damaged, not associated with a physical attack
        public virtual void XORDamage(float dmg)
        {
        }
        //Calls when damaged by a physical attack or event
        public virtual void ReceiveAttack(Vector3 attackdir, float force)
        {
        }
        //Calls on and during collision event, returns transform
        public virtual void CollisionEvent(Transform source)
        {
        }
        //Calls from Weapon on a successful weapon hit
        public virtual void SuccessfulWeaponHit(Transform target)
        {
        }
        //Calls from Weapon when an attack is executed, but either fails or times out
        public virtual void FailedWeaponHit()
        {
        }
        //Same, but returns a source
        public virtual void FailedWeaponHit(Transform source)
        {
        }
        //Set the AI Loading zone of this particular character.
        public void SetZone(Droogs dr)
        {

            zone = dr;
        }
        

        //Handle explicit on/off statements inside of Header script (override 'ActivateFunction' and 'DeactivateFunction' *
        //Do not switch off the AI Character.  If not active, it will perform its Deactive
        //Function, which is by default a dummy method.  However, it will sense whether or not it
        //is activated every frame, although this is a simple boolean question
        //relating to an easily retrieved place in memory.
        //Leave at least one non-trigger collider active at all times,
        //for purposes of spatial searching.
        //
        //*Remember, DeactivateFunction calls once when Character is unloaded,
        //while DeactiveFunction calls every frame while not loaded.
        void Update()
        {
            if(zone)
            SetActive(zone.IsActive());
            ConstantFunc();
            if (active)
            {
                UpdateFunc();
            }
            else
            {
                DeactiveFunc();
            }
        }
        //block activity for forhowlong seconds
        public void Stun(float forhowlong)
        {
            active = false;
            Invoke("OffStun", forhowlong);
        }
        //automatically calls after stun period
        protected void OffStun()
        {
            active = true;
        }

        public void Kill()
        {

        }
        //Call to alert AI Character
        protected virtual void Alarm()
        {
        }
        //Calls every frame regardless of activity
        protected virtual void ConstantFunc()
        {
        }
        //Calls when activated
        protected virtual void ActivateFunc()
        {
        }
        //Calls when deactivated
        protected virtual void DeactivateFunc()
        {
        }
        //Calls while not active
        protected virtual void DeactiveFunc()
        {
        }
        //Calls within Start()
        protected virtual void InitFunc()
        {
        }
        //Calls within Update() if active.
        protected virtual void UpdateFunc()
        {
        }
        //Destroy, calls from Mortality when health <= 0
        public virtual void Untangle()
        {
        }

        //Detailed OnCollisionStay information from a collider not contained within the same transform as the AI Header script.
        public virtual void WhileCollision(Collision c)
        {
        }
        //Detailed OnCollisionEnter information
        public virtual void OnCollision(Collision c)
        {
        }
        //
        protected virtual void Atk()
        {
        }
        //Set true for Active behaviour, set false for Inactive behaviour
        public void SetActive(bool x)
        {

            cbx.Onit = x;
            if (active && !x)
            { DeactivateFunc(); 
            }
            else if (!active && x)
            {

                ActivateFunc();
            }
            active = x; 
        }
    }

