using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class UnitBlueprint
{
    [Header("Name of the Unit")]
    public string Name;

    [Header("Health Pool of the Unit")]
    public float BaseHP;
    public float CurrentHP;

    [Header("Morale of the Unit")]
    public float BaseMorale;
    public float CurrentMorale;

    [Header("Basic Attributes of the Unit")]
    public float BaseAtk;
    public float Cur_Atk;
    public float Defense;
    public float Speed;

    [Header("List of Attacks the Unit can Perform")]
    public List<BaseAttack> ListOfAttacks = new List<BaseAttack>();
    [Header("List of Attacks the Unit can Perform")]
    public List<BaseAttack> ListOfMagicSpells = new List<BaseAttack>();
}
