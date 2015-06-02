using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class LIZDrillDash : Ability
{
    static LIZDrillDash statref;
    //Player Character special move:
    //If player is targeting an enemy/object,
    //Homing rockets will be activated,
    //otherwise, flight rockets will be activated.
    //Puts particles, animation, sfx, and appropriate logic all in one place.
    Transform source;
    Transform target;
    Cabinet c;
    DirectlyControlledMover m;
    Animator n;
    ParticleManager par;
    CombatInterface swrd;
    bool home;
    int stat;
    int particles;
        float manause;
        float dmana, hmana;
    string ss = "";
    float ord, oret;
    bool orip = false;
    public LIZDrillDash(Transform liz, int st, int pa, int snd)
    {
        statref = this;
        ord = 0.4f;
        oret = Time.time;
        
        this.source = liz;
        c = liz.GetComponent<Cabinet>();
        m = liz.GetComponent<DirectlyControlledMover>();
        n = liz.GetComponent<Animator>();
        par = liz.GetComponent<ParticleManager>();
        swrd = liz.GetComponent<CombatInterface>();
        stat = st;
        particles = pa;
        manause = 2.0f;
        dmana = 5.0f;
        hmana = 10.0f;
    }

    public string getstr()
    {
        string r = ss;
        ss = "";
        return r;
    }

    void RocketSpinOn()
    {
        statref = this;
        FASTACTIONCAMERA.DBM("Rocket Spin Init", "LizDrillDash");
        n.SetInteger("sig", 20);
        n.SetTrigger("AirDash");
        n.SetInteger("sig", 20);
        swrd.OBE();
        swrd.SetSpecialState(0);
        m.ROCKETS(1);
        par.TurnOn(particles);
        manause = dmana;
    }

    public override void AttackMod()
    {
        m.modifyflightspeed(3.5f, 0.8f);
    }

    public override void JumpMod()
    {
        m.modifyflightspeed(-0.5f, 0.5f);
    }

    Vector3[] corkscrew;



    void HomingSpinOn()
    {
        statref = this;
        FASTACTIONCAMERA.DBM("Homing Dash Init", "LizDrillDash");
        n.SetInteger("sig", -1);
        n.SetTrigger("AirDash");
        swrd.OBE();
        swrd.SetSpecialState(0);
        m.ROCKETS(1);
        par.TurnOn(particles);
        manause = hmana;
    }
    public override bool CanUseNow()
    {
        if (source.GetComponent<VitalBody>().QueryCon(stat, manause) && !orip)
            return true;
        else
            return false;
    }

    public override void ActivateEffect(Transform s, Transform t)
    {
        statref = this;
        source = s;
        target = t;
        if (t == null)
        {
            RocketSpinOn();
        }
        else
        {
            HomingSpinOn();
        }
    }
    public override void DeactivateEffect(Transform s, Transform t)
    {
        statref = this;
        m.UnRocket();
        swrd.WeaponCollisionEvent();
        par.TurnOff(particles);
    }
    public override void RuntimeEffect(Transform s, Transform t)
    {
        statref = this;
        source = s ;
        float time = Time.deltaTime;
        if (m.GetRockets() == -1)
        {
            TurnOff();

        }
        if(!Liz.dontusemana)
        s.GetComponent<VitalBody>().UseCon(stat, manause * time);
        if (swrd.spsta == -1)
            TurnOff();
        if (!s.GetComponent<VitalBody>().QueryCon(stat, manause))
        {
            TurnOff();
        }
    }
}

