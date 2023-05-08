using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoison : BaseAttack
{
    public WeakPoison()
    {
        AttackName = "Weak Poison";
        AttackDescription = "A weak Poison, not enough to kill someone, but sure as heck is it annoying";
        AttackDamage = 5f;
        AttackCost = 4f;
    }
}
