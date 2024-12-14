using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunUpgradeType
{
    Damage,
    FireRate,
    Accuracy,
    AmmoCapacity,
    ReloadSpeed
}

[CreateAssetMenu(fileName = "GunUpgrade", menuName = "Scriptable Objects/Gun Upgrade")]
public class GunUpgrade : ScriptableObject
{
    public string upgradeName;
    public GunUpgradeType upgradeType;
    [TextArea(5, 10)]
    public string flavorText;

    [Space]
    public Sprite sprite;

    [Space]
    public bool isPercentage;
    public float upgradeValue;

    [Space]
    public int upgradeTier;
}
