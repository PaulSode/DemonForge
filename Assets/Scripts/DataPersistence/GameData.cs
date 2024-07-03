using System.Collections.Generic;

public class GameData
{
        
    [System.Serializable]
    public class Minion
    {
        public int index;
        public int level;
        public List<string> upgrades;
    }
    public List<Minion> minions;
    
    public BFN gold;
    public List<string> inkJSONNames;
    public List<BFN> inkJSONThresholds;
    public bool mainButtonShown;

    public GameData()
    {
        this.gold = BFN.Zero;
        this.minions = null;
        this.inkJSONNames = new List<string>();
        this.inkJSONThresholds = new List<BFN>();
        this.mainButtonShown = false;
    }
}
