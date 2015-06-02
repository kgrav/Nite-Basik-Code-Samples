using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
//Class for handling the physiological stage of a character
//Contains health, plays damage sound,
//Calls death function if parent dies,
//And initializes mesh flash on damage.
//Serves as the logical entry point of all
//damage and external force in animate objects.
//Even external forces which do not cause damage
//enter here.  This component is shared
//by every character involved in real-time gameplay,
//including players, bosses, and enemies.
//(the HitZone acts as the physical and temporal entry point, where the
//attack collisions are originally received.)

//This class should be contained at the Root of the Game Object which it describes.
//At least one Hitzone must be present anywhere in the Game Object's heirarchy in order
//for the object to receive general external damaging forces.
public enum MORTCOLL {HLF, SLF, PLF, PSH, NONE}
public enum MORTQUAL { FH, HH, CH, DD }
[AddComponentMenu("Nite-Basik/Action Coordination/Everyone/Mortality")]
public class Mortality : MonoBehaviour
{
    public MORTCOLL qh, qhp;
    public MORTQUAL q, qp;
    public int HitSound;
    VitalStat healthstat;
    NumberCard rep;
    public bool flashMesh;
    bool blinking = false;
    public Transform MeshFlash;
    public float MaxHealth;
    public float Resistance; // ultimate damage = forcein/resistance
    public float invuldelay; //seconds which this object will remain invulnerable for following an attack
    public float mass;
    public bool AUTOTRANSFORM; //do we call transformation methods?
    public bool Deferred; //If this Mortality immune to direct hits? (i.e., damage done purely through hitboxes)
    public bool invulor;
    public int sndctx;
    public int[] ElementalImmunities;
    public bool reflect;
    public float health;
    Vector3 hitvector;
    Renderer[] flsh;
    AICharacter holder;
    bool droogy = false;
    public void SetInvul(bool a)
    {
        invulor = a;
    }
    public void PlaySound()
    {
            GetComponent<AudioSource>().PlayOneShot(SoundTable.GetSound(sndctx, HitSound));
     
    }

    public void SetReflect(bool a)
    {
        reflect = a;
    }

    public void DirectDamagePercent(float f)
    {
        PlaySound();
        if (droogy)
            holder.XORDamage(MaxHealth * (f / 100.0f));
        print("ddP");
        if (invulor)
            return;
        health -= MaxHealth * (f / 100.0f);
        HealthChange();
        Invul();
        Invoke("Outvul", invuldelay);
    }

    public void DirectRestorePercent(float f)
    {
        health += MaxHealth * (f / 100.0f);
        HealthChange();
    }
    public void Invul()
    {
        invulor = true; Invoke("Outvul", invuldelay);
        blinking = true;
        MeshBlink();
    }
    public void DangerFluid(DangerFluid v)
    {
        foreach (int t in ElementalImmunities)
        {
            if (v.type == t)
                return;
        }
            DirectDamage(v.damage);
    }

    public void MeshBlink()
    {
        if (flashMesh) {
            foreach (Renderer r in flsh)
                r.enabled = false; 
            Invoke("MeshRea", 0.03f);
        }
    }

    public void MeshRea()
    {if (flashMesh) {
        foreach (Renderer r in flsh)
            r.enabled = true;
            if (blinking)
            {
                Invoke("MeshBlink", 0.1f);
            }
        }
    }

    public void Outvul()
    {
        blinking = false;
        invulor = false;
    }
    public void DirectDamage(float f)
    {
        if (!invulor)
        {
            health -= f;
            HealthChange();
            if(q != MORTQUAL.DD && qp != MORTQUAL.DD)
            PlaySound();
            if(holder)
            holder.XORDamage(f);
            if (!invulor && AUTOTRANSFORM)

            Invul();
            
        }
    }

    public void DirectRestore(float f)
    {
        health += f;
        HealthChange();
    }

    public float PercentHealth()
    {
        return health / MaxHealth;
    }
    void Start()
    {
        holder = GetComponent<AICharacter>();
        if (holder)
            droogy = true;
        rep = GetComponentInChildren<NumberCard>();
        health = MaxHealth;
        flsh = new Renderer[1];
        flsh[0] = GetComponent<Renderer>();
        if (!flsh[0])
        {
            List<Renderer> temp = new List<Renderer>();
            flsh = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < flsh.Length; ++i)
            {
                if (!flsh[i].GetComponent<SpriteRenderer>())
                    temp.Add(flsh[i]);
            }
            flsh = temp.ToArray();
        }
        hitvector = Vector3.zero;
    }

    void Update()
    {
        qp = q;
        HealthChange();
    }

    public bool HitRecently()
    {
        return qh != MORTCOLL.NONE || qhp != MORTCOLL.NONE;
    }

    public bool HealthChange()
    {

        float dam = MaxHealth - health;
        if (dam < 5.0f)
            q = MORTQUAL.FH;
        else if (dam < MaxHealth / 2.0f)
            q = MORTQUAL.HH;
        else if (dam > MaxHealth)
        {
            q = MORTQUAL.DD;
            if (droogy)
                holder.Untangle();
        }
        return q != qp;
    }

    public Vector3 Shoot(Vector3 impulse, Vector3 at, float force)
    {
        if(invulor&&reflect)
            return Vector3.Reflect(impulse*force, GetComponent<Transform>().position - at);

        if (rep)
            rep.TurnOn(force);
        print(transform + " S");
        qh = MORTCOLL.SLF;
        hitvector = impulse * force;
        AnimatorTranceiver cq = GetComponent<AnimatorTranceiver>();
        if (!invulor && !Deferred)
        { health -= force / Resistance; HealthChange(); }
        if (!invulor&& cq != null && !droogy)
            cq.SpecialTrig("OR");
        if (!invulor && !reflect && droogy)
            holder.ReceiveAttack(impulse, force);
        if (!invulor && AUTOTRANSFORM)
            GetComponent<BaseMover>().HitWithAtk(impulse,force);
        Invul();
        if(reflect)
            return Vector3.Reflect(impulse*force, GetComponent<Transform>().position - at);
        return Vector3.zero;
    }

    public void StoreFlags()
    {
        qp = q;
        qhp = qh;
    }

    public Vector3 BluntOn(Vector3 impulse, float force, Transform source)
    {

        if (invulor)
            return Vector3.zero;
        PlaySound();
        if (rep && force>0)
            rep.TurnOn(force);
        qh = MORTCOLL.HLF;
        hitvector = impulse * force;
        AnimatorTranceiver cq = GetComponent<AnimatorTranceiver>();
        if (!invulor && !Deferred)
        { health -= force / Resistance; HealthChange(); }
        if (!invulor && cq != null)
            cq.SpecialTrig("OR");
        if (!invulor && !reflect && droogy)
            holder.ReceiveAttack(impulse, force);
        if (!invulor && AUTOTRANSFORM)
            GetComponent<BaseMover>().HitWithAtk(impulse,force);
        Invul();
        if (force < Resistance)
            return -hitvector.normalized * (force - mass);
        else
            return Vector3.zero;
    }

    public MORTCOLL CollisionInfo()
    {
        return qh;
    }

    public float HealthCurrent()
    {
        return health;
    }

    public Vector3 HitVector()
    {
        return hitvector;
    }

    public Vector3 Push(Vector3 impulse, float force)
    {
        qh = MORTCOLL.PSH;
        hitvector = impulse * (force - mass);
        return hitvector;
    }

    public void BluntOff()
    {
        qh = MORTCOLL.NONE;
    }
}
