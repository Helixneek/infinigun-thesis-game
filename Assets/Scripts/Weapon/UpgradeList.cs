using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunUpgrade", menuName = "Scriptable Objects/Gun Upgrade List")]
public class UpgradeList : ScriptableObject
{
    public List<GunUpgrade> gunUpgrades = new List<GunUpgrade>();
}
