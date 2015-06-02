using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public enum WEAPONTYPE { RANG, BLNT, BLDE, PROJ, MAGK }
[AddComponentMenu("Nite-Basik/Action Coordination/Everyone/Combat Interfacer")]
    public class CombatInterface : Weapon
    {
        public Vector3 impulseExpose;
        public Vector3 ImpulseError;

        public WEAPONTYPE TypeofWeapon;
        public int Count;
        public int ForMult;
        public bool queuing, ORing;
        bool animor;

        public int[] ATKONrange;
        public int[] ATKOFFsig;
        public float timer;
        public int[] order;
        bool wponinit = false;
        public Vector3[] SpecialStateNormals;
        public int[] SpecialStateCollisions;
        Transform[] equipment;

        void Start()
        {

        }
        public bool busy;
        //Upon initiating a queued or commanded action, On New Sig will call once.
        void OnNewSig()
        {

            foreach (Transform w in equipment)
            {

               w.GetComponent<Weapon>().OBE();
               w.GetComponent<Weapon>().SetActivity(MWFLAGS.ATK);
            }
            busy = true;
            if (spsta == -1)
            {
                Invoke("Deactivate", timer);
            }
        }
        //After the spcified delay, this CI leaves its busy state and deactivates child weapons
        void Deactivate()
        {
            busy = false;
            if (spsta == -1 || spsta == 2)
            {
                spsta = -1;
                foreach (Transform w in equipment)
                {

                    w.GetComponent<Weapon>().SetActivity(MWFLAGS.NONE);
                }
            }
            if (q)
            {
                q = false;
                OnNewSig();
            }
        }

        public void AtkFail()
        {
            AnimatorTranceiver m = GetComponent<AnimatorTranceiver>();
            BaseMover q = GetComponent<BaseMover>();
            if (m != null)
            {
                print("ATKF");
                m.SpecialTrig("CIOR");
            }
        }
        public int spsta;
        public void SetSpecialState(int i)
        {
            MeleeWeapon m = GetComponentInChildren<MeleeWeapon>();
            if (m != null && i != -1)
                m.MASK_ALL = true;
            else if (i == -1)
                m.MASK_ALL = false;
            imem = m.impulse;
            m.impulse = SpecialStateNormals[i];
            if (i == 2)
            {
                m.crit = true;
                m.MASK_ALL = false;
            }
            if (i == 4 || i == 2)
            {
                CancelInvoke("Deactivate");
                Invoke("Deactivate", 0.8f);
            }
            spsta = i;
        }
        public int XCell = 0;
        bool blocksynapse = false;
        public void XCelTrigger()
        {
            if (!blocksynapse)
            {
                XCell++;
                blocksynapse = true;
                Invoke("RevXCellTrigger", 0.5f);
            }
            
        }
        Vector3 imem;
        public void RevXCellTrigger()
        {
            XCell--;
        }
        public void WeaponCollisionEvent()
        {
            if (XCell > 0&&ContextBody.GroupOf(transform) != 0)
            {
                XCell--;
                blocksynapse = false;

                Infrared q = Infrared.PlayerTarget;
                if (q)
                {
                    if (q.next)
                        Infrared.PlayerTarget = q.next;
                }
            }
            if(spsta >= 0){
            AnimatorTranceiver m = GetComponent<AnimatorTranceiver>();
            DirectlyControlledMover q = GetComponent<DirectlyControlledMover>();
            equipment[0].GetComponent<MeleeWeapon>().SetActivity(MWFLAGS.NONE);
            GetComponent<Cabinet>().PlaySoundNOW(SpecialStateCollisions[spsta]);
            spsta = -1;
            Deactivate();
            if (m != null)
            {

                m.SpecialTrig("CIOR");
            }
            if(q != null){
                q.UnRocket();
            }
            }
        }
        void DeQ()
        {
            q = false;
        }
        public bool AnimOR()
        {
            bool f = animor;
            animor = false;
            return f;
        }

        public void SetAnimOverride()
        {
            animor = true;
        }

        public void RollCall(int i, Transform m)
        {
            if (!wponinit)
            {
                equipment = new Transform[Count];
                wponinit = true;
            }
            equipment[i] = m;
        }
        public void ANIMSIDETurnOn()
        {
            equipment[0].GetComponent<Weapon>().SetActivity(MWFLAGS.ATK);
            OBE();
        }
        int lastcode = -1;
        public void OBE()
        {
            impulseExpose = (transform.position + transform.TransformDirection(-Vector3.forward)).normalized;
            if (busy)
            {
                q = true;
                Invoke("DeQ", 0.2f);
            }
            else
            {
                OnNewSig();
            }
        }
        bool q = false;

        public void SendAnimSig(int fist, int code)
        {
            impulseExpose = (transform.position + transform.TransformDirection(-Vector3.forward)).normalized;
            MWFLAGS r;
            if (code > ATKONrange[0] && code < ATKONrange[1])
            {
                r = MWFLAGS.ATK;
                if (code != lastcode)
                    OnNewSig();
            }
            lastcode = code;
        }

    }

