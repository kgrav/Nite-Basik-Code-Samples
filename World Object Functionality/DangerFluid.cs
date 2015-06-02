using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Particle effects which cause damage to colliding objects,
//unless they are immune (Mortality -> Elemental Immunities)
//"SELength" is the length of the status effect caused,
//"prefab" is the link to the status effect copy of the fluid,
//"contagious" is true when the fluid causes a status effect.
//Status effects will be removed by affected object when they expire.

//The Poison in William's Bombs is an example of a Danger Fluid which causes
//a status effect.
[RequireComponent(typeof(ParticleSystem))]

[AddComponentMenu("Nite-Basik/Mechanical Constructs/Danger Particles")]
    public class DangerFluid : MonoBehaviour
    {
        public int type;
        public float damage;
        public bool contagious;
        public GameObject prefab;
        public float SELength;
        public GameObject Catch()
        {
            return (GameObject)GameObject.Instantiate(prefab);
        }

        void OnParticleCollision(GameObject other)
        {
            if (other.GetComponent<Mortality>())
            { other.GetComponent<Mortality>().DangerFluid(this); }
            else if (other.GetComponent<HitZone>())
            {
                other.GetComponent<HitZone>().DangerFluid(this);
            }
        }
    }
