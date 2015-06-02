using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

    public class LIZLeafSpin : Ability
    {
        Transform localholder;
        DirectlyControlledMover sourcemover;
        Mortality mourt;
        Cabinet snd;
        //Player Special Move - Leaf Spin
        //Early game special move that provides limited invincibility, and locks y position
        //Useful for getting across gaps that are too far or oddly-arranged to jump,
        //or gaps which have low ceilings/a damaging entity preventing traversal
        //by other means (other than flight anyway)

        VitalBody soulpower;
        int stat;
        int particles, xparticles;
        float manause = 0.4f;
        bool inAtk;
        float timein, length;
        public LIZLeafSpin(Transform liz, int st, int pa, int ak)
        {
            inAtk = false;
            localholder = liz;
            stat = st;
            particles = pa;
            animkey = ak;
            length = 0.8f;
            xparticles = 3;
            snd = liz.GetComponent<Cabinet>();
            xmodsound = 9;
            jmodsound = 10;
        }

        public override void AttackMod()
        {
            if (!inAtk)
            {
                inAtk = true;
                timein = Time.time;
            }
        }

        public override bool CanUseNow()
        {
            soulpower = localholder.GetComponent<VitalBody>();
            DEBUGsounds.DBS(7);
            return !Locked && Unlocked && soulpower.QueryCon(stat, manause); 
        }

        public override void ActivateEffect(Transform s, Transform t)
        {
            localholder = s;
            soulpower = s.GetComponent<VitalBody>();
            sourcemover = s.GetComponent<DirectlyControlledMover>();
            mourt = s.GetComponent<Mortality>();

            s.GetComponent<ParticleManager>().TurnOff();
            s.GetComponent<ParticleManager>().Set(particles);
            s.GetComponent<ParticleManager>().TurnOn();
            if(soulpower.QueryCon(stat, manause))
            {
                sourcemover.SetSpState(0);
                mourt.SetInvul(true);
                mourt.SetReflect(true);
                s.GetComponent<ParticleManager>().TurnOn(particles);
            }

        }

        public override void DeactivateEffect(Transform s, Transform t)
        {
            soulpower = s.GetComponent<VitalBody>();
            sourcemover = s.GetComponent<DirectlyControlledMover>();
            mourt = s.GetComponent<Mortality>();
            sourcemover.SetSpState(-1);
            mourt.SetInvul(false);
            mourt.SetReflect(false);
            s.GetComponent<ParticleManager>().TurnOff(xparticles);
            s.GetComponent<ParticleManager>().TurnOff(particles);
        }

        public override void RuntimeEffect(Transform s, Transform t)
        {
            soulpower = s.GetComponent<VitalBody>();
            sourcemover = s.GetComponent<DirectlyControlledMover>();
            mourt = s.GetComponent<Mortality>();
            s.GetComponent<ParticleManager>().Set(particles);
            if (inAtk)
            {
                if (Time.time - timein > length)
                {
                    inAtk = false;
                    s.GetComponent<ParticleManager>().TurnOff(xparticles);
                    s.GetComponent<ParticleManager>().TurnOn(particles);
                }
            }
            if (!Liz.dontusemana)
            {
                if (sourcemover.Motion())
                    soulpower.UseCon(stat, manause);
                else
                    soulpower.UseCon(stat, (manause * 2) * Time.deltaTime);
            }
            if (!soulpower.QueryCon(stat, manause))
            {
                TurnOff();
            }
        }

    }

