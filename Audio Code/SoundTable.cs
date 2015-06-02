using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
//Loads scene Sound Effects,
//Referenced by objects of a specific type (Sound Context)
//Loads all audio needed for a scene, and holds one copy of each needed
//sound effect in the SoundLib array.
//This way, we don't have one copy of an AudioClip loaded into memory
//For each and every clone of an object (they are stored globally, rather
//than locally, but are still played locally, in 3D space).
//Much more reliable and efficient than the old Sound Effects Cabinet system, which loaded
//clips locally for each object, even if several objects with common manifests were present.
//key words:
//"Sound Context" - the ID number of a Sound Manifest (document where Sound Effect file names
//are listed and assigned numeric codes) is assigned in-editor, 
//the array indexes from the public string array directly correspond with the
//table index of the manifest contained therein.  (i.e., if Liz was soundContext 0, 
//her table would be Element 0 in editor)
//Make sure to set it up carefully, so you don;t have once source playing a difference source's sound clips!4 fdfz dfDe5 qwe4zz Asdfe+

//"Clip Index" is the numeric code assigned to an individual audioclip
//within a manifest file.
//The sound effect can be retrieved with both a Context Number and a Clip Number

[AddComponentMenu("Nite-Basik/Sound/Static Sound Table")]
    public class SoundTable : MonoBehaviour
    {
        public string[] ManifestsByContext;

        static AudioClip[][] soundLib;
        //Get sound based on Table ID (context) and Clip ID (index)
        public static AudioClip GetSound(int context, int index)
        {
            return soundLib[context][index];
        }
        //At start, read in all locally declared Manifest files.
        void Start()
        {
            soundLib = new AudioClip[ManifestsByContext.Length][];
            for (int i = 0; i < ManifestsByContext.Length; ++i)
            {
                soundLib[i] = ReadManifest(ManifestsByContext[i]);
            }
        }

        public AudioClip[] ReadManifest(string m)
        {
            string path = "Assets/SNDmanifest" + m + ".txt";
            StreamReader Manifest = new StreamReader(path);
            string[] splits;
            AudioClip[] database = new AudioClip[20];
            int phase = 0;
            string ts = Manifest.ReadLine();
            while (ts != null)
            {

                if (ts[0] == '.')
                {
                    phase++;
                }
                else
                {
                    switch (phase)
                    {
                        case 0:
                            break;
                        case 1:
                            database = new AudioClip[Convert.ToInt32(ts)];
                            break;
                        case 2:
                            splits = ts.Split(':');
                            database[Convert.ToInt32(splits[0])] = (AudioClip)Resources.Load(splits[1], typeof(AudioClip));
                            break;
                    }
                }
                try
                {
                    ts = Manifest.ReadLine();
                }
                catch (EndOfStreamException e)
                {
                    ts = null;
                }
            }
            return database;
        }
    }

