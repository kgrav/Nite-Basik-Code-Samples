using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
//Manages Player Special Moves.
//Special Moves are unlocked, activated, deactivate, and 
//modified through this class.
//Puts the complex functionality of special abilities in one place.
    public class AbilityManager
    {
        Transform top;
        List<Ability> mag;
        int active;
        int animreturnkey;
        public int Active()
        {
            return active;
        }
        public AbilityManager(Transform t)
        {
            top = t;
            mag = new List<Ability>();
            animreturnkey = -1;
            active = -1;
        }

        public int AddAbility(Ability a)
        {
            mag.Add(a);
            return mag.IndexOf(a);
        }
        public void finishCooldown()
        {
        }
        public void Cooldown()
        {
        }
        public void JumpMod()
        {
            if (active != -1)
            {
                mag[active].JumpMod();
            }
        }
        public void AttackMod()
        {
            if (active != -1)
            {
                mag[active].AttackMod();
            }
        }
        public bool UseAbility()
        {
            if (active != -1)
            {
                mag[active].SetReg(top, top);
                if (!mag[active].CanUseNow())
                    return false;
                mag[active].Use();
                return true;
            }
            return false;
        }
        public bool Check(int i)        {
            return mag[i].CanUseNow();
        }
        public void StopAbility()
        {
            if (active != -1)
            {
                mag[active].TurnOff();
            }
        }

        public int GetAnimKey()
        {
            return animreturnkey;
        }

        public void RunTicker()
        {
            if (active != -1)
            {
                if (mag[active].Active())
                {
                    animreturnkey = mag[active].animkey;
                }
                else
                {
                    animreturnkey = -1;
                }
            }
            else
            {
                animreturnkey = -1;
            }
        }

        public void UnlockAbility(int abilikey)
        {
            mag[abilikey].Unlock();
        }
        public bool isPlaying()
        {
            return mag[active].Active();
        }
        public void StopNSwap(int i)
        {
            if (i != active)
                mag[active].TurnOff();
            active = i;
        }

        public void HotSwap(int i)
        {
            active = i;
        }
    }

