using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string AttackName;
    public string AttackDescription;

    //  explains itself, but could also show how strong a heal is
    public float AttackDamage;

    //  if the attack cost is covered by resource, then its useable.
    public float AttackCost;
}
