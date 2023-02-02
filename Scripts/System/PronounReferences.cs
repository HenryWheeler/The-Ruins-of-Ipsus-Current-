using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    public class PronounReferences
    {
        public static Dictionary<string, PronounSet> pronounSets = new Dictionary<string, PronounSet>();
        public PronounReferences()
        {
            pronounSets.Add("Player", new PronounSet("you", "you", "your", "yourself", false));
            pronounSets.Add("Masculine", new PronounSet("he", "him", "his", "himself"));
            pronounSets.Add("Feminine", new PronounSet("she", "her", "hers", "herself"));
            pronounSets.Add("Nueter", new PronounSet("it", "it", "its", "itself"));
            pronounSets.Add("NueterPlural", new PronounSet("they", "them", "theirs", "themself"));
        }
    }
}
