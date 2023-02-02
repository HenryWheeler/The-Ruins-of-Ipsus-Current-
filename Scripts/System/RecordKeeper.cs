using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Ruins_of_Ipsus
{
    public class RecordKeeper
    {
        public static Record record { get; set; }
        public RecordKeeper(Record recordKeeper) { record = new Record(recordKeeper); }
        public RecordKeeper(string _playerName, int _totalWealth, string _floorsVisited, bool _playerEscaped)
        { record = new Record(_playerName, _totalWealth, _floorsVisited, _playerEscaped); }
        public RecordKeeper() { }
    }
    [Serializable]
    public class Record
    {
        public string playerName { get; set; }
        public int totalWealth { get; set; }
        public string floorsVisited { get; set; }
        public bool playerEscaped { get; set; }
        public Record(Record recordKeeper)
        {
            playerName = recordKeeper.playerName;
            totalWealth = recordKeeper.totalWealth;
            floorsVisited = recordKeeper.floorsVisited;
            playerEscaped = recordKeeper.playerEscaped;
        }
        public Record(string _playerName, int _totalWealth, string _floorsVisited, bool _playerEscaped)
        {
            playerName = _playerName;
            totalWealth = _totalWealth;
            floorsVisited = _floorsVisited;
            playerEscaped = _playerEscaped;
        }
        public Record() { }
    }
}
