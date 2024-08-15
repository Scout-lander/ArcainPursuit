using UnityEngine;

[CreateAssetMenu(fileName = "OneOffItemData", menuName = "Shop/OneOffItemData")]
public class OneOffItemData : ScriptableObject
{
    public string itemName;
    public PlayerStats.Stats statBoost;
    public int cost;
    public string description;
    public Sprite icon;
}
