using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell : BaseAttack
{
    public FireSpell()
    {
        AttackName = "Fire";
        AttackDescription = "through sheer concentration, u were able to procure a small flame, is it getting warmer in here?";
        AttackDamage = 20f;
        AttackCost = 5f;
    }
}
