using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerSwing : BaseAttack
{
    public HammerSwing()
    {
        AttackName = "Hammer Swing";
        AttackDescription = "A powerful strike that can make the earth rumble";
        AttackDamage = 15f;
        AttackCost = 0f;
    }
}
