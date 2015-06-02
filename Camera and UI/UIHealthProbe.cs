using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

    public class UIHealthProbe : MonoBehaviour
    {
        Slider k;
        Transform whos;
        
        void Start()
        {

            k = GetComponent<Slider>();
        }
        //Match value of Slider with whos's Mortality's health value.
        void Update()
        {
            if (!whos)
                whos = Liz.StaticLiz.transform;
            float sc = whos.GetComponent<Mortality>().PercentHealth();
            k.value = sc;
            
        }

    }

