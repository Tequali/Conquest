using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public BaseAttack MagicAttackToPerform;
    public void CastingAttack()
    {
        GameObject.FindObjectOfType<BattleStateMachine>().Input4(MagicAttackToPerform);
    }
}
