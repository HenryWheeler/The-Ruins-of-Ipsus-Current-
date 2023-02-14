using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    [Serializable]
    public class Stats: Component
    {
        /// <summary>
        /// An int to determine actor level, this variable once set does not change and instead will represent the strength
        /// of a creature upon normally spawning. This is used to determine things like the value of equipment randomly generated
        /// for an actor to have.
        /// </summary>
        public int level { get; }
        public int sight { get; set; }
        public int ac { get; set; }
        public float maxAction { get; set; }
        public int hp { get; set; }
        public int hpCap { get; set; }
        public int strength { get; set; }
        public int acuity { get; set; }
        public List<string> immunities = new List<string>();
        public List<string> weaknesses = new List<string>();
        public Stats(int _sight, int _ac, float _maxAction, int _hpCap, int _strength, int _acuity, int _level, List<string> _immunities = null, List<string> _weaknesses = null) 
        { 
            sight = _sight; ac = _ac; maxAction = _maxAction; hp = _hpCap; hpCap = _hpCap;
            strength = _strength; acuity = _acuity;
            level = _level;
            if (_immunities != null) { immunities = _immunities; }
            if (_weaknesses != null) { weaknesses = _weaknesses; }
        }
        public Stats() { }
    }
}
