using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[AddComponentMenu("Lady Stardust/Header Scripts/Liz")]
[RequireComponent(typeof(DirectlyControlledMover))]
[RequireComponent(typeof(Mortality))]
[RequireComponent(typeof(CombatInterface))]
[RequireComponent(typeof(Manipulator))]
[RequireComponent(typeof(ParticleManager))]
[RequireComponent(typeof(Inventory))]
public class Liz : MAIN_CHARACTER
{
    public static void AddMeat(float f, Vector3 pos)
    {
        StaticLiz.Meat += f;
        GameObject.Instantiate(StaticLiz.sprites[0], pos, Quaternion.identity);

    }
    public static Liz StaticLiz;
    public static TargetHelper StaticTHelp;
     public static bool dontusemana;
        public bool DEBUG_UNLOCKLEAFSPIN, DEBUG_UNLOCKDRILLDASH, DEBUG_UNLOCKSHATTERSHINE, DEBUG_BOTTOMLESSMANA;
        NumLang digikey;
        DirectlyControlledMover motor;
        Mortality body;
        Animator nerves;
        AudioSource voice;
        Manipulator manos;
        ControllerAdmin pad;
        ControllerAdmin secondary;
        CharacterController collid;
        CombatInterface sword;
        ContextBody metabody;
        VitalBody v;
        public GameObject[] sprites;
        AbilityManager m;
        public int SoundContext;
        public RectTransform AbilityProbe;
        bool SoulShine;
        int spin, fly;
        int shatter;
        int backst;
        Vector3 bodypos;
        int currability;
        int abin;
        public float MaxSoulPower;
        float SoulPower;
        char hb;
        char sb;
        int pb;
        public float stepoffs;
        float stepcurr = 2.0f;
        int footflip = 0;
        float fmod;
        Vector3 tomover, frompad, flatinput, origin;
        float Meat; //Meat is yout money, and your XP
        public int GetAbility()
        {
            return m.Active();
        }
        public override void SetCheckPoint(Vector3 pos)
        {
            DontDestroyOnLoad(transform.gameObject);
            LizLoader.staticref = this;
            DontDestroyOnLoad(LizLoader.staticref);
            origin = pos;
        }
        public override void Respawn()
        {
            transform.position = origin;
            body.DirectRestore(100.0f);
        }
        float stickmag = 0.0f;
        void Start()
        {
            StaticLiz = this;
            dontusemana = DEBUG_BOTTOMLESSMANA;
            if (LizLoader.staticref == null)
                SetCheckPoint(transform.position);
            m = new AbilityManager(GetComponent<Transform>());
            spin = m.AddAbility(new LIZLeafSpin(GetComponent<Transform>(), 1, 0, 5));
            fly = m.AddAbility(new LIZDrillDash(transform, 1, 2, 11));
            shatter = m.AddAbility(new LIZShatterShine(transform, 1, 3));
            m.HotSwap(spin);
            if (DEBUG_UNLOCKDRILLDASH)
                m.UnlockAbility(fly);
            if (DEBUG_UNLOCKLEAFSPIN)
                m.UnlockAbility(spin);
            if (DEBUG_UNLOCKSHATTERSHINE)
                m.UnlockAbility(shatter);
            interruptsig = false;
            AbilityProbe.GetComponent<UIAbilityProbe>().Init(new int[] { spin, fly });
            origin = transform.position;
            spinon = false;
            abilic = spin;
            abilin = fly;
            PrepareRefs();
            interruptsig = false;
            mtf = false;
            hb = '~';
            pb = -1;
            anims = -1;
            animsn = -1;
            minp = false;
            animsp = -1;
            fmod = 0.0f;
            hmem = GetComponent<CharacterController>().center;
            smem = GetComponent<CharacterController>().height;
            soundsig = -1;
            tomover = Vector3.zero;
            frompad = Vector3.zero;
            pad = new ControllerAdmin();
            digikey = new NumLang();
            GetComponent<ParticleManager>().TurnOff();
            pad.AddButton("X", 'x', false);
            pad.AddButton("B", 'b', false);
            pad.AddButton("A", 'a', false);
            pad.AddButton("Y", 'y', false);
            pad.AddButton("RB", 'r', true);
            pad.AddButton("STRT", 's', false);
            pad.AddAxis("U", "V", 'm', true, true, 0.2f);
            pad.AddAxis("UD", "VD", 'd', false, false, 0.1f);

            secondary = new ControllerAdmin();
            secondary.AddButton("LB", 'g', true);
            secondary.AddButton("A", 'a', true);
            secondary.AddButton("SLCT", 'o', false);

            digikey.AddSyn(2, 'x');
            digikey.AddSyn(3, 'b');
            digikey.AddSyn(6, 'y');
            digikey.AddSyn(7, 'a');
            digikey.AddSyn(-2, 'r');
            digikey.AddSyn(5, 'l');
            abin = -1;
            Jump = false;
            PrepareRefs();
        }

        bool holding;
        bool sworded = false, swordsig = false;
        bool MOVE_OVERRIDE = false;
        bool ANIM_OVERRIDE = false;
        bool ACTN_OVERRIDE = false;
        bool willcancel = false;

        bool STOP_EVERYTHING = false;

        bool Combat;
        bool ClimbOR;
        bool Interact;
        bool JumpOn;
        bool Slash;
        bool Pray;
        bool spinon;
        bool lockuntilrelease;
        bool HoldAbility;
        bool HoldPray;
        bool HoldLock;
        bool HoldJump;
        bool pressJump;
        bool TapThrow;
        bool pressAtk;
        bool pressB;
        bool HoldB;
        bool Jump;
        bool helmbrkr = false;
        bool DodgeL;
        bool DodgeR;
        bool skipau = false;
        bool onceperjump = false;
        bool ability;
        int anims, animsn, animsp, animsubn;
        int soundsig;
        bool interruptsig, mtf, minp;
        string dbl = "";
        int abilic, abilin;
        public override void UnlockAbility(int i)
        {
            m.UnlockAbility(i);
        }
        public void UnlockAbility()
        {
            print("Liz Unlocked Leaf Spin!");
            m.UnlockAbility(spin);
        }
        bool HOLDIT = false;
        int ctd = 0;
        public void ApplyGameLogic()
        {

            RaycastHit tgt;
            if (!TargetArrow.PlayersArrow.turnedon)
            {
                if (Physics.SphereCast(new Ray(transform.position - transform.forward * 2.1f, -transform.forward), 1, out tgt, 200.0f))
                {
                    if (tgt.transform.root.GetComponent<Infrared>() != null)
                        Infrared.PlayerTarget = tgt.transform.root.GetComponent<Infrared>();
                }
            }
            bool continu = false;
            holding = GetComponent<Manipulator>().holding;
            int jumpstaten = motor.GetJS();
            int rockstaten = motor.GetRockets();
            if (holding && TapThrow)
            {
                skipau = true;
                nerves.SetFloat("fmod", 0);
                nerves.SetInteger("sig",-1);
                nerves.SetTrigger("Throw");
                voice.PlayOneShot(SoundTable.GetSound(SoundContext, 12));
                GetComponent<Manipulator>().ThrowButton();
            }
            int swordsong = 0;
            int songsong = 0;
            if (spinon && !HoldAbility)
            {
                spinon = false;
                m.HotSwap(spin);
                m.StopAbility();
            }
            if (Combat && !HoldLock)
            {
                Combat = false;
                motor.ResetRate();
                motor.ReleaseTarget();
            }
            bodypos = GetComponentsInChildren<HitZone>()[0].transform.position;
            willcancel = false;
            swordsong = anims;
            skipau = false;
            if (sword.busy)
            {

                motor.UnlockAccel();
                willcancel = true;
                if (TapThrow)
                {
                    if (motor.OnGround())
                    {
                        nerves.SetTrigger("UnGrab");
                        motor.ForwardDash(0.5f);
                        sword.OBE();
                        skipau = true;
                    }
                }
            }
            if (rollindirty && pressAtk)
            {
                nerves.SetTrigger("strike");
                sword.OBE();
                sword.SetSpecialState(2);
                willcancel = true;
                skipau = true;
            }
            else if(pressAtk && (jumpstaten != -1 && jumpstaten != 1 && jumpstaten != 4))
            {
                helmbrkr = true;
                nerves.SetTrigger("HelmBreaker");
                sword.OBE();
                sword.SetSpecialState(1);
                willcancel = true;
                skipau = true;
            }
            if (currability == fly && (motor.GetRockets() == -1 && ability))
            {
                ability = false;
                caninit = false;
                m.StopAbility();
            }
            if (HoldAbility && caninit)
            {
                bool c = m.UseAbility();

                if (c)
                {
                    ability = true;

                }
            }
            else if (!m.Check(currability) || !HoldAbility || sword.spsta == -1)
            {

                m.StopAbility();
                ability = false;
            }
            if (ability && pressAtk)
            {
                m.AttackMod();
            }
            else if (ability && pressJump)
            {
                m.JumpMod();
            }
            if (HoldLock)
            {
                ctd = 0;
                if (!Combat && Infrared.PlayerTarget != null)
                    motor.ZTarget(Infrared.PlayerTarget.transform);
                Combat = true;
                motor.AdjustRate(0.5f);
            }
            if (motor.OnGround() && (pressAtk || (pressB && stickmag < 0.2)))
            {
                willcancel = true;
                continu = false;
            }

            if (!onceperjump && (motor.GetJS() == 4 || motor.GetJS() == 5))
            {
                onceperjump = true;
                voice.PlayOneShot(SoundTable.GetSound(SoundContext, 4));
            }
            if (!ability && !pressB)
            {
                if ((pressJump) && (motor.GetJS() == -1 || motor.GetJS() == 2))
                {
                    if (motor.GetJS() == -1)
                        voice.PlayOneShot(SoundTable.GetSound(SoundContext, 4));
                    motor.JumpTap();
                    Jump = true;
                    onceperjump = false;
                }
                else if (Jump && !HoldJump)
                {

                    motor.ReleaseJump();
                    Jump = false;
                }
            }

            if ((jumpstaten == -1 && !(HoldJump || pressJump)) || jumpstaten == 2 || jumpstaten == 6)
                Jump = false;
            tomover = frompad;
            if (willcancel)
            {
                tomover = Vector3.zero;
                stickmag = 0;

            }
            else if (!Jump)
            {
                if (stickmag > 0.1f && motor.OnGround() && !ability)
                {
                    stepcurr += Time.deltaTime;
                    if ((stepcurr >= 0.2 && !Combat) || (stepcurr > 0.4f && Combat) && !pressJump)
                    {
                        stepcurr = 0;
                        voice.PlayOneShot(SoundTable.GetSound(SoundContext, footflip));
                        if (footflip == 0)
                            footflip = 1;
                        else if (footflip == 1)
                            footflip = 0;
                    }
                }
            }
            motor.SetInput(tomover, stickmag);
        }

        void Update()
        {
            dbl = "";
            skipau = false;
            DoTheReading();
            if (!STOP_EVERYTHING)
            {
                ProcessControllerInput();
                ApplyGameLogic();
                if (!skipau)
                {
                    DecideAnimationState();
                    SetAnimationState();
                }
            }
        }

        public void SetAnimationState()
        {
            nerves.SetInteger("sig", animsn);
            nerves.SetInteger("subsig", animsubn);
            nerves.SetFloat("fmod", fmod);
            dbl += animsn + ", " + animsubn + ", " + fmod;
        }

        public void DecideAnimationState()
        {
            if (ability)
            {
                if (m.Active() == spin)
                {
                    animsn = 5;
                    fmod = 0;
                    return;
                }
                else if (m.Active() == fly)
                {

                    if (motor.GetRockets() == 2)
                        animsn = 20;
                    return;
                }
            }
            if ((motor.OnGround() || animsn == 2) || TapThrow)
            {
                animsn = -1; animsubn = -1;
            }
            if (!motor.OnGround())
            {
                fmod = 0;
            }
            else
            {
                fmod = stickmag;
            }
            if (motor.GetJS() == -1 && motor.GetRockets() == -1)
            {
                nerves.SetTrigger("Land");
                Jump = false;
                if (helmbrkr)
                {
                    helmbrkr = false;
                    sword.WeaponCollisionEvent();
                }
                if (Combat)
                {
                    animsubn = 2;
                }
                if (pressB && !motor.Motion())
                {
                    int i = manos.interactionButton();

                    animsubn = i;
                    animsn = 3;


                    fmod = 0;
                }
                else if (pressB && fmod > 0)
                {
                    nerves.SetTrigger("Roll");
                    motor.AimJolt(0.75f, frompad, 2.2f);
                    Roll(0.75f);
                }
                else if (pressJump)
                {
                    fmod = 0;
                    nerves.SetTrigger("Jump");
                    animsn = 7;
                }
                if (pressAtk)
                {
                    animsn = 2;
                    nerves.SetTrigger("strike");
                    fmod = 0;
                }
                else
                {
                    if (!Combat && !pressB && !TapThrow && !HoldAbility && !pressJump)
                    {
                        animsubn = -1;
                        animsn = -1;
                    }
                }

            }
            else
                if (Jump && motor.GetRockets() == -1)
                {
                    fmod = 0;
                    animsn = 7;
                    if (pressAtk)
                        nerves.SetTrigger("strike");
                }
                else if (pressAtk)
                {
                    nerves.SetTrigger("strike");
                    animsn = 2;
                    fmod = 0;
                }
                else if (spinon)
                {
                    animsn = 5;
                    animsubn = -1;
                    fmod = 0;
                    return;
                }
        }
        bool rollindirty;
        bool caninit = true;
        Vector3 hmem;
        float smem;
        float tmem;
        public void Roll(float t)
        {
            tmem = t;
            rollindirty = true;
            voice.PlayOneShot(SoundTable.GetSound(SoundContext,6));
            collid.height = 2;
            collid.center = hmem + Vector3.down * 1.6f;
            Invoke("UnRoll", t);
        }
        void UnHold()
        {
            HOLDIT = false;
        }
        public void UnRoll()
        {
                rollindirty = false;
                collid.height = smem;
                collid.center = hmem;
            
        }
        int atksig;
        bool lockab;
        public void ProcessControllerInput()
        {
            HoldLock = false;
            HoldPray = false;
            HoldAbility = false;
            dbl += "PAD: ";
            switch (hb)
            {
                case 'r':
                    HoldLock = true;
                    break;
            }
            pressJump = false;
            pressB = false;
            TapThrow = false;
            pressAtk = false;
            switch (pb)
            {
                case 6:
                    TapThrow = true;
                    break;
                case 7:
                    pressJump = true;
                    break;
                case 2:
                    HOLDIT = true;
                    pressAtk = true;
                    sword.OBE();
                    break;
                case 3:
                    pressB = true;
                    break;

            }
            DodgeL = false;
            DodgeR = false;
            HoldJump = false;
            bool labtf = lockab;
            bool fff = Physics.Raycast(new Ray(bodypos, Vector3.down), 0.5f) && !HoldAbility;
            switch (sb)
            {
                case 'l':
                    if (fff)
                        lockab = false;
                    DodgeL = true;
                    break;
                case 'r':
                    lockab = false;
                    if (fff)
                        DodgeR = true;
                    break;
                case 'g':
                    lockab = true;
                    if (labtf == false)
                    {
                        caninit = true;
                    }
                    HoldAbility = true;
                    break;
                case 'a':
                    lockab = false;
                    HoldJump = true;
                    break;
                default:
                    lockab = false;
                    break;
            }
        }

        void TurnOffAutoJump()
        {
            HoldJump = false;
        }


        public void Interrupt()
        {
            interruptsig = true;
        }
        void PrepareRefs()
        {
            motor = GetComponent<DirectlyControlledMover>();
            body = GetComponent<Mortality>();
            nerves = GetComponent<Animator>();
            voice = GetComponent<AudioSource>();
            sword = GetComponent<CombatInterface>();
            metabody = GetComponent<ContextBody>();
            manos = GetComponent<Manipulator>();
            collid = GetComponent<CharacterController>();
        }

        void DoTheReading()
        {
            pad.Frame();
            secondary.Frame();
            UStickReceipt u = pad.GetAxes()[0];
            frompad = u.v;
            stickmag = u.mag;
            flatinput = u.raw;
            if (u.m)
                fmod = u.v.magnitude;
            m.RunTicker();
            if (secondary.GetButton() == 'o')
            {
                if (currability == spin)
                {
                    currability = fly;
                }
                else if (currability == fly)
                {
                    currability = spin;
                }
                m.StopNSwap(currability);
                AbilityProbe.GetComponent<UIAbilityProbe>().Signal(currability);
            }
            sb = secondary.GetExtButton();
            abin = m.GetAnimKey();
            pb = digikey.GetSig(pad.GetButton());
            hb = pad.GetExtButton();
            animsp = anims;
        }

    /*QUICK LOCATIONS:
     * Controller Variables - CNTRLV
     * Attack Signals - ATKGL, ATKAS
     * Jump Signal - JMPGL, JMP
     *

    public static bool dontusemana;
    public bool DEBUG_UNLOCKLEAFSPIN, DEBUG_UNLOCKDRILLDASH, DEBUG_UNLOCKSHATTERSHINE, DEBUG_BOTTOMLESSMANA;
    NumLang digikey;
    DirectlyControlledMover motor;
    Mortality body;
    AnimatorTranceiver nerves;
    Cabinet voice;
    Manipulator manos;
    ControllerAdmin pad;
    ControllerAdmin secondary;
    CombatInterface sword;
    ContextBody metabody;
    VitalBody v;
    AbilityManager m;
    ClimbableWorldObject Ledge;
    public RectTransform AbilityProbe;
    bool SoulShine;
    int spin, fly;
    int shatter;
    int backst;
    Vector3 bodypos;
    int currability;
    int abin;
    public float MaxSoulPower;
    float SoulPower;
    char hb;
    char sb;
    int pb;
    public float stepoffs;
    float stepcurr = 2.0f;
    int footflip = 0;
    float fmod;
    Vector3 tomover, frompad, flatinput, origin;
    public int GetAbility()
    {
        return m.Active();
    }
    public void SetCheckPoint(Vector3 pos)
    {
        DontDestroyOnLoad(transform.gameObject);
        LizLoader.staticref = this;
        DontDestroyOnLoad(LizLoader.staticref);
        origin = pos;
    }
    public void Respawn()
    {
        transform.position = origin;
        body.DirectRestore(100.0f);
    }
    float stickmag = 0.0f;
    void Start()
    {
        dontusemana = DEBUG_BOTTOMLESSMANA;
        if (LizLoader.staticref == null)
            SetCheckPoint(transform.position);
        m = new AbilityManager(GetComponent<Transform>());
        spin = m.AddAbility(new LIZLeafSpin(GetComponent<Transform>(), 1, 0, 5));
        fly = m.AddAbility(new LIZDrillDash(transform, 1, 2, 11));
        shatter = m.AddAbility(new LIZShatterShine(transform, 1, 3));
        m.HotSwap(spin);
        if (DEBUG_UNLOCKDRILLDASH)
            m.UnlockAbility(fly);
        if (DEBUG_UNLOCKLEAFSPIN)
            m.UnlockAbility(spin);
        if (DEBUG_UNLOCKSHATTERSHINE)
            m.UnlockAbility(shatter);
        interruptsig = false;
        AbilityProbe.GetComponent<UIAbilityProbe>().Init(new int[] {spin, fly });
        origin = transform.position;
        spinon = false;
        abilic = spin;
        abilin = fly;
        PrepareRefs();
        interruptsig = false;
        mtf = false;
        hb = '~';
        pb = -1;
        anims = -1;
        animsn = -1;
        minp = false;
        animsp = -1;
        fmod = 0.0f;
        hmem = GetComponent<CharacterController>().center;
        smem = GetComponent<CharacterController>().height;
        soundsig = -1;
        tomover = Vector3.zero;
        frompad = Vector3.zero;
        pad = new ControllerAdmin();
        digikey = new NumLang();
        GetComponent<ParticleManager>().TurnOff();
        pad.AddButton("X", 'x', false);
        pad.AddButton("B", 'b', false);
        pad.AddButton("A", 'a', false);
        pad.AddButton("Y", 'y', false);
        pad.AddButton("RB", 'r', true);
        pad.AddButton("STRT", 's', false);
        pad.AddAxis("U", "V", 'm', true, true, 0.2f);
        pad.AddAxis("UD", "VD", 'd', false, false, 0.1f);

        secondary = new ControllerAdmin();
        secondary.AddButton("LB", 'g', true);
        secondary.AddButton("A", 'a', true);
        secondary.AddButton("SLCT", 'o', false);
        secondary.AddTrigger("LT", 'l', 0.5f);
        secondary.AddTrigger("RT", 'r', 0.5f);

        digikey.AddSyn(2, 'x');
        digikey.AddSyn(3, 'b');
        digikey.AddSyn(6, 'y');
        digikey.AddSyn(7, 'a');
        digikey.AddSyn(-2, 'r');
        digikey.AddSyn(5, 'l');
        abin = -1;
        nerves.AddLooker(-1, "TOP.NONE", 0);
        nerves.AddLooker(-2, "TOP.ORST", 0);
        nerves.AddLooker(-3, "TOP.JUMP", 0);
        nerves.AddLooker(28, "TOP.CMBT", 0);
        nerves.AddLooker(31, "TOP.ATK1", 0);
        nerves.AddLooker(32, "TOP.ATK2", 0);
        nerves.AddLooker(33, "TOP.ATK3", 0);
        nerves.AddLooker(32, "TOP.DLATK", 0);
        nerves.AddLooker(32, "TOP.DRATK", 0);
        nerves.AddLooker(1, "TOP.MOVEN", 0);
        nerves.AddLooker(1, "TOP.MOVEC", 0);
        nerves.AddLooker(10, "TOP.PUSH", 0);
        nerves.AddLooker(11, "TOP.PICK", 0);
        nerves.AddLooker(12, "TOP.MOVEH", 0);
        nerves.AddLooker(12, "TOP.HVY", 0);
        nerves.AddLooker(13, "TOP.HVYT", 0);
        nerves.AddLooker(6, "TOP.PRAY", 0);
        nerves.AddLooker(7, "TOP.SPIN", 0);
        nerves.AddLooker(8, "TOP.DASH", 0);
        nerves.AddLooker(20, "TOP.POLG", 0);
        nerves.AddLooker(20, "TOP.LDRG", 0);
        nerves.AddLooker(21, "TOP.POLC", 0);
        nerves.AddLooker(21, "TOP.LDRC", 0);
        nerves.AddLooker(25, "TOP.LDJ", 0);
        nerves.AddLooker(26, "TOP.LDGR", 0);
        nerves.AddLooker(26, "TOP.LDJL", 0);
        nerves.AddLooker(27, "TOP.LDJU", 0);
        nerves.AddLooker(40, "TOP.ROLL", 0);
        nerves.AddLooker(9, "TOP.SLOFF", 0);
        nerves.AddLooker(9, "TOP.SROFF", 0);
        nerves.AddLooker(7, "TOP.ForwardSpin", 0);
        nerves.AddLooker(30, "TOP.GreatStab", 0);
        nerves.AddLooker(31, "TOP.GreatStabFinisher", 0);
        nerves.AddLooker(30, "TOP.GreatStabReturn", 0);
        nerves.AddLooker(30, "TOP.AirDash", 0);
        Jump = false;
    }

    bool holding;
    bool sworded = false, swordsig = false;
    bool MOVE_OVERRIDE = false;
    bool ANIM_OVERRIDE = false;
    bool ACTN_OVERRIDE = false;
    bool willcancel = false;

    bool STOP_EVERYTHING = false;

    bool Combat;
    bool ClimbOR;
    bool Interact;
    bool JumpOn;
    bool Slash;
    bool Pray;
    bool spinon;
    bool lockuntilrelease;
    bool HoldAbility;
    bool HoldPray;
    bool HoldLock;
    bool HoldJump;
    bool pressJump;
    bool TapThrow;
    bool pressAtk;
    bool pressB;
    bool HoldB;
    bool Jump;
    bool DodgeL;
    bool DodgeR;
    bool skipau = false;
    bool onceperjump = false;
    bool ability;
    int anims, animsn, animsp, animsubn;
    int soundsig;
    bool interruptsig, mtf, minp;
    string dbl = "";
    int abilic, abilin;
    public void UnlockAbility(int i)
    {
        m.UnlockAbility(i);
    }
    public void UnlockAbility()
    {
        print("Liz Unlocked Leaf Spin!");
        m.UnlockAbility(spin);
    }
    bool HOLDIT = false;
    int ctd = 0;
    public void ApplyGameLogic()
    {

        RaycastHit tgt;
        if(!TargetArrow.PlayersArrow.turnedon){
        if (Physics.SphereCast(new Ray(transform.position -transform.forward*2.1f, -transform.forward), 1, out tgt, 200.0f))
        {
            if (tgt.transform.root.GetComponent<Infrared>() != null)
                Infrared.PlayerTarget = tgt.transform.root.GetComponent<Infrared>();
        }}
        bool continu = false;
        holding = GetComponent<Manipulator>().holding;
        if(holding && TapThrow)
        {
            skipau = true;
            nerves.SendF(0);
            nerves.Send(-1);
            nerves.SpecialTrig("Throw");
            GetComponent<Manipulator>().ThrowButton();
        }
        int swordsong = 0;
        int songsong = 0;
        if (spinon && !HoldAbility)
        {
            spinon = false;
            m.HotSwap(spin);
            m.StopAbility();
        }
        if (Combat && !HoldLock)
        {
                Combat = false;
                motor.ResetRate();
                motor.ReleaseTarget();
        }
        bodypos = GetComponentsInChildren<HitZone>()[0].transform.position;
        willcancel = false;
        swordsong = anims;
        skipau = false;
        if (HOLDIT && anims == 31)
            HOLDIT = false;
        if (sword.busy)
        {
            
            motor.UnlockAccel();
            HOLDIT = false;
            willcancel = true;
            if (TapThrow)
            {
                if (motor.OnGround())
                {
                    
                    nerves.SpecialTrig("UnGrab");
                    motor.ForwardDash(0.5f);
                    sword.OBE();
                    skipau = true;
                }
            }
        }
        if (!HOLDIT && rollindirty && pressAtk)
        {
            nerves.SpecialTrig("strike");
            sword.OBE();
            willcancel = true;
            skipau = true;
            HOLDIT = true;
        }
        if (currability == fly &&(motor.GetRockets() == -1 && ability))
        {
            ability = false;
            caninit = false;
            m.StopAbility();
        }
        if (HoldAbility && caninit)
        {
            bool c = m.UseAbility();

            if (c)
            {
                ability = true;

            }
        }
        else if(!m.Check(currability) || !HoldAbility || sword.spsta == -1)
        {

            m.StopAbility();
            ability = false;
        }
        if (ability && pressAtk)
        {
            m.AttackMod();
        }
        else if (ability && pressJump)
        {
            m.JumpMod();
        }
        if (HoldLock)
        {
            ctd = 0;
            if (!Combat && Infrared.PlayerTarget != null)
                motor.ZTarget(Infrared.PlayerTarget.transform);
            Combat = true;
            motor.AdjustRate(0.5f);
            if (DodgeL)
            {
                print("ddj");
                motor.Dodge(-1);
                skipau = true;
            }
            else if (DodgeR)
            {
                print("ddj");
                motor.Dodge(1);
                skipau = true;
            }
        }
        if (motor.OnGround() && (pressAtk || HoldPray || (pressB && stickmag < 0.2)))
        {
            willcancel = true;
            continu = false;
        }
        if (Combat && (DodgeL || DodgeR))
        {
            willcancel = true;

        }
        
        if (!onceperjump &&(motor.GetJS() == 4 || motor.GetJS() == 5))
        {
            onceperjump = true;
            voice.PlaySoundNOW(4);
        }
        if (!DodgeL && !DodgeR && !ability && !pressB)
        {
            if ((pressJump) && (motor.GetJS() == -1 || motor.GetJS() == 2))
            {
                if (motor.GetJS() == -1)
                    voice.PlaySoundNOW(4);
                motor.JumpTap();
                Jump = true;
                onceperjump = false;
            }
            else if (Jump && !HoldJump)
            {
                print("Released jump because jump not held");
                motor.ReleaseJump();
                Jump = false;
            }
            else
            {
                Jump = false;
            }
        }
        else
        {
            Jump = false;
        }

        if (willcancel)
        {
            tomover = Vector3.zero;
            stickmag = 0;

        }
        else if(!Jump)
        {
            tomover = frompad;
            if (stickmag > 0.1f && motor.OnGround() && !ability)
            {
                stepcurr += Time.deltaTime;
                if ((stepcurr >= 0.2 && !Combat) || (stepcurr > 0.4f && Combat) &&!pressJump)
                {
                    stepcurr = 0;
                    voice.PlaySoundNOW(footflip);
                    if (footflip == 0)
                        footflip = 1;
                    else if (footflip == 1)
                        footflip = 0;
                }
            }
        }
        motor.SetInput(tomover, stickmag);
    }

    void Update()
    {
        dbl = "";
        skipau = false;
        PrepareRefs();
        DoTheReading();
        if (!STOP_EVERYTHING)
        {
            ProcessControllerInput();
            ApplyGameLogic();
            if (!skipau)
            {
                DecideAnimationState();
                SetAnimationState();
            }
        }
    }

    public void SetAnimationState()
    {
        nerves.Send(animsn);
        nerves.SendSub(animsubn);
        nerves.SendF(fmod);
        dbl += animsn + ", " + animsubn + ", " + fmod;
    }

    public void DecideAnimationState()
    {
        if (ability)
        {
            if (m.Active() == spin)
            {
                animsn = 5;
                fmod = 0;
                return;
            }
            else if (m.Active() == fly)
            {

                if (motor.GetRockets() == 2)
                    animsn = 20;
                return;
            }
        }
        if ((motor.OnGround() || animsn == 2) || TapThrow)
        {
            animsn = -1; animsubn = -1;
        }
        if (!motor.OnGround())
        {
            fmod = 0;

        }
        else
        {
            fmod = stickmag;
        }
        if (motor.OnGround())
        {
            nerves.SpecialTrig("Land");
            Jump = false;
            if (Combat)
            {
                animsubn = 2;

            }
            if (pressB && !motor.Motion())
            {
                int i = manos.interactionButton();

                animsubn = i;
                animsn = 3;

                
                fmod = 0;
            }
            else if (pressB && fmod > 0)
            {
                nerves.SpecialTrig("Roll");
                motor.AimJolt(0.75f, frompad, 2.2f);
                Roll(0.75f);
            }
            else if (pressJump)
            {
                fmod = 0;

                animsn = 7;
                nerves.SpecialTrig("Jump");
            }
            if (pressAtk)
            {
                animsn = 2;
                nerves.SpecialTrig("strike");
                fmod = 0;
            }
            else
            {
                if (!Combat && !pressB && !TapThrow && !HoldAbility && !pressJump)
                {
                    animsubn = -1;
                    animsn = -1;
                }
            }

        }
        else 
            if (Jump && motor.GetRockets() == -1)
            {
                fmod = 0;
                animsn = 7;
                if (pressAtk)
                    nerves.SpecialTrig("strike");        
            }
            else if (pressAtk)
            {
                nerves.SpecialTrig("strike");
                animsn = 2;
                fmod = 0;
            }
            else if (spinon)
            {
                    animsn = 5;
                    animsubn = -1;
                    fmod = 0;
                    return;
            }
    }
    bool rollindirty;
    bool caninit = true;
    Vector3 hmem;
    float smem;
    public void Roll(float t)
    {
        rollindirty = true;
        voice.PlaySoundNOW(6);
        GetComponent<CharacterController>().height = 2;
        GetComponent<CharacterController>().center = hmem + Vector3.down * 1.2f;
        Invoke("UnRoll", t);
    }
    void UnHold()
    {
        HOLDIT = false;
    }
    public void UnRoll()
    {
        rollindirty = false;
        GetComponent<CharacterController>().height = smem;
        GetComponent<CharacterController>().center = hmem;
    }
    public void LedgeOR(ClimbableWorldObject c)
    {
        if (!motor.OnGround())
        {
            nerves.SpecialTrig("Grab");
            ClimbOR = true;
            Ledge = c;
            animsn = -1;
            m.HotSwap(spin);
            m.StopAbility();
            if (c.Width != 1)
            { 
                if (c.Width == 0)
                {
                    print("LEDGE GRAB");
                }
                else if (c.Width == 3)
                {
                    print("LADDER GRAB");
                }
            }
        }
    }
    int atksig;
    bool lockab;
    public void ProcessControllerInput()
    {
        HoldLock = false;
        HoldPray = false;
        HoldAbility = false;
        dbl += "PAD: ";
        switch (hb)
        {
            case 'r':
                HoldLock = true;
                break;
        }
        pressJump = false;
        pressB = false;
        TapThrow = false;
        pressAtk = false;
            switch (pb)
            {
                case 6:
                    TapThrow = true;
                    break;
                case 7:
                    pressJump = true;
                    break;
                case 2:
                    HOLDIT = true;
                    pressAtk = true;
                    GetComponent<CombatInterface>().OBE();
                    break;
                case 3:
                    pressB = true;
                    break;

            }
            DodgeL = false;
            DodgeR = false;
            HoldJump = false;
            bool labtf = lockab;
        bool fff = Physics.Raycast(new Ray(bodypos, Vector3.down), 0.5f) && !HoldAbility;
                switch (sb)
                {
                    case 'l':
                        if (fff)
                            lockab = false;
                        DodgeL = true;
                        break;
                    case 'r':
                        lockab = false;
                        if(fff)
                        DodgeR = true;
                        break;
                    case 'g':
                        lockab = true;
                        if (labtf == false)
                        {
                            caninit = true;
                        }
                        HoldAbility = true;
                        break; 
                    case 'a':
                        lockab = false;
                        HoldJump = true;
                        break;
                    default:
                        lockab = false;
                        break;
                }
        }

    void TurnOffAutoJump()
    {
        HoldJump = false;
    }


    public void Interrupt()
    {
        interruptsig = true;
    }
    void PrepareRefs()
    {
        motor = GetComponent<DirectlyControlledMover>();
        body = GetComponent<Mortality>();
        nerves = GetComponent<AnimatorTranceiver>();
        voice = GetComponent<Cabinet>();
        sword = GetComponent<CombatInterface>();
        metabody = GetComponent<ContextBody>();
        manos = GetComponent<Manipulator>();
    }
    
    void DoTheReading()
    {
        pad.Frame();
        secondary.Frame();
            UStickReceipt u = pad.GetAxes()[0];
            frompad = u.v;
            stickmag = u.mag;
            flatinput = u.raw;
            if (u.m)
                fmod = u.v.magnitude;
        m.RunTicker();
        if (secondary.GetButton() == 'o')
        {
            if(currability == spin)
            {
                currability = fly;
            }
            else if(currability == fly)
            {
                currability = spin;
            }
            m.StopNSwap(currability);
            AbilityProbe.GetComponent<UIAbilityProbe>().Signal(currability);
        }
        if(Input.GetAxis("LT") > 0.0f)
        {
            sb = 'l';
        }
        else if (Input.GetAxis("RT") > 0.0f)
        {
            sb = 'r';
        }
        else
        {
            sb = secondary.GetExtButton();
        }
        abin = m.GetAnimKey();
        pb = digikey.GetSig(pad.GetButton());
        hb = pad.GetExtButton();
        animsp = anims;
        anims = nerves.getkey()[0];
    }
    */
}