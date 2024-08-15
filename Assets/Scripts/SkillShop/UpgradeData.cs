using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Shop/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    
    [System.Serializable]
    public class UpgradeLevel
    {
        public string description;
        public int cost;
        public PlayerStats.Stats statBoost;
    }

    public UpgradeLevel[] levels;
}
