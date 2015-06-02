using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using UnityEngine;

[AddComponentMenu("Nite-Basik/Action Coordination/Main Only/Vital Body")]
    public class VitalBody : MonoBehaviour
    {
        public string StatsManifest;
        public bool startmaxes;
        public float maxrad;
        VitalStat[] stats;

        void Start()
        {
            ReadManifest();
        }

        public void ReadManifest()
        {
            string path = "Assets/statmanifest" + StatsManifest + ".txt";
            StreamReader Manifest = new StreamReader(path);

            string ts = Manifest.ReadLine();
            stats = new VitalStat[Convert.ToInt32(ts)];
            Manifest.ReadLine();
            for (int i = 0; i < stats.Length; ++i)
            {
                
                string[] splits = Manifest.ReadLine().Split(':');
                bool ibt = splits[4].Equals("1");
                stats[i] = new VitalStat(splits[1][0], i, splits[2], (float)(Convert.ToInt32(splits[3]) * maxrad), ibt);
                 
            }
        }


        public void AlterMax(int i, float newmax)
        {
            stats[i].NewMax(newmax);
        }

        public void IncrMax(int i, float maxinc)
        {
            stats[i].IncrMax(maxinc);
        }

        public bool QueryCon(int i, float f)
        {
            return stats[i].Query(f);
        }


        public bool QueryCon(int i, int j)
        {
            return stats[i].Query(j);
        }

        public float GetValCon(int i)
        {
            return stats[i].GetVal();
        }

        public float GetPCon(int i)
        {
            return stats[i].Percentage();
        }

        public int GetValDiscreteCon(int i)
        {
            return stats[i].GetValDiscrete();
        }

        public void UseCon(int i, float f)
        {
            stats[i].Use(f);

        }

        public void UseCon(int i, int j)
        {
            stats[i].Use(j);
        }

        public void RegenCon(int i, float f)
        {
            stats[i].Regen(f);
        }

        public void RegenCon(int i, int j)
        {
            stats[i].Regen(j);
        }
    }

