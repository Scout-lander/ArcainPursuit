using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public RuneBagsData runeBagsData;
    public EquippedWeaponData equippedWeaponData;
    public ToggleStateData toggleStateData;
    public PassiveUpgradesData passiveUpgradesData;
}

[Serializable]
public class RuneBagsData
{
    public RuneBagSerializable runeBag;
    public RuneBagSerializable equippedRuneBag;
}

[Serializable]
public class EquippedWeaponData
{
    public WeaponData equippedWeapon;
}

[Serializable]
public class ToggleStateData
{
    public bool isOn;
}

[Serializable]
public class PassiveUpgradesData
{
    public List<PassiveUpgrades.Upgrade> upgrades;
    public List<PassiveUpgrades.OneOffItem> oneOffItems;
}
