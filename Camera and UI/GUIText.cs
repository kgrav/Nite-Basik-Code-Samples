using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Component for displaying scriped text bubbles
//in-game.  Use for dialogue, inspectable objects, etc.
[AddComponentMenu("Nite-Basik/UI/GUI Text")]
    public class GUIText : MonoBehaviour
    {
        bool active = true, sigin = false;
        static GUIText StatiGUI;
        public RectTransform pane;
        public float textspeed;
        string text;
        string curr;
        string buffer;
        int buffered;
        public AudioClip textsnd, upsnd, dnsnd;
        int ptr;
        int ellipses;
        int length;
        InspectBody source;
    //Load text before displaying.
        public static bool SetText(InspectBody h)
        {
            StatiGUI.source = h;
            StatiGUI.buffer = h.GetBuffer();
            StatiGUI.buffered = 1; StatiGUI.Activate();
            return true;
        }
    //Called from inspectable object on interaction, after loading, and also
    //from within this class, to advance to the next block of text when button is pressed.  
        public static void OBE()
        {
            if (StatiGUI.active && StatiGUI.length - StatiGUI.ptr < 4)
            {
                if ((StatiGUI.buffered == 0))
                {
                    StatiGUI.PushButton();
                }
                else
                {
                    StatiGUI.Activate();
                }
            }
            else if (!StatiGUI.active && StatiGUI.buffered > 0)
            {
                StatiGUI.Activate();
            }
            
        }
        void PushButton()
        {
            if (active)
            {
                sigin = false;
                StorySounds.Sound(dnsnd);

            }
        }
        float timein = 0;
            void Activate()
            {
                buffered--;
                sigin = true;
                text = buffer;
                curr = "";
                ptr = 0;
                timein = Time.realtimeSinceStartup;
                length = text.Length;
                StorySounds.Sound(upsnd);
                string nxt = source.GetBuffer();
                if (nxt.Equals("END"))
                {
                    buffered = 0;

                }
                else
                {
                    buffered = 1;
                    buffer = nxt;
                }
            }
        void Start()
            {
                GetComponent<CanvasRenderer>().SetAlpha(0);
                pane.GetComponent<CanvasRenderer>().SetAlpha(0);
            buffered = 0;
            StatiGUI = this;
            active = false;
        }
    //On update, turn off rendering if inactive,
    //If active, print one character of the block at a time (according to text speed)
    //Stop printing and wait for button event when a text block has been completed.
    //Input is set up to mirror the buttons on an XBOX 360 controller; A and B refer to face buttons,
    //or keyboard inputs (equivalent of 'B' would traditionally be 'E', and the equivalent of 'A' is the left mouse button (in GUI) or space (out of GUI))
        void Update()
        {
            StatiGUI = this;
            if (Input.GetButtonUp("A") || Input.GetButtonUp("B"))
                OBE();
            if(active && !sigin)
            {
                Time.timeScale = 1.0f;
                active = false;
                GetComponent<CanvasRenderer>().SetAlpha(0);
                pane.GetComponent<CanvasRenderer>().SetAlpha(0);
            }
            else if (!active && sigin)
            {
                Time.timeScale = 0.0f;
                active = true;
                GetComponent<CanvasRenderer>().SetAlpha(1);
                pane.GetComponent<CanvasRenderer>().SetAlpha(1);
            }
            if (active && ptr < length)
            {
                float time = Time.realtimeSinceStartup - timein;
                if (time > textspeed)
                {
                    if (ptr < length)
                    {
                        timein += time;
                        StorySounds.Sound(textsnd);
                        curr += text[ptr];
                        ptr++;
                    }
                    else
                    {
                        timein += time;
                        string dot = "";
                        if (ellipses == 4)
                        {
                            ellipses = 1;
                        }
                        for (int i = 0; i < ellipses; ++i)
                        {
                            dot += ". ";
                        }
                        ellipses++;
                        curr = text + dot;
                    }
                    GetComponent<UnityEngine.UI.Text>().text = curr;
                }
            }
        }
    }

