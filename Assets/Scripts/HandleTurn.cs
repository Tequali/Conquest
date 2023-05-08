using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class HandleTurn
{
    public string Attacker;                 // Name of Attacker
    public string Type;                     // Enemy or Hero
    public GameObject AttackersGameObject;  // GameObject of Attacker
    public GameObject AttackersTarget;      // Who is going to be attacked

    // which attack is performed
    public BaseAttack ChooseAttack;
}
