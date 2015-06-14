using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[AddComponentMenu("Nite-Basik/UI/Ability Image")]
    public class UIAbilityProbe : MonoBehaviour
    {
        int[] keys;
        public Sprite[] images;
    public int defailt;
        public AudioClip sound;
        public Transform liz;
    public RectTransform thisImage;
        int max;
        int ptr;
        bool[] Unlocked;

        public void Init(int[] ke)
        {
            keys = ke;
            thisImage.GetComponent<UnityEngine.UI.Image>().sprite = images[defailt];
        }


        public void Signal(int key)
        {
            for (int i = 0; i < keys.Length; ++i)
            {
                if (keys[i] == key)
                {
                    StorySounds.Sound(sound);
                    thisImage.GetComponent<UnityEngine.UI.Image>().sprite = images[key];
                }
            }
        }
    }
