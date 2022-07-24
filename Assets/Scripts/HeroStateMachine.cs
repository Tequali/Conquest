using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    public UnitBlueprint hero;
    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }
    public TurnState CurrentState;
    public GameObject EnemyToAttack;
    public Image HP_Bar;

    private float currentCooldown = 0f;
    private float maxCooldown = 5f;
    void Start()
    {
        CurrentState = TurnState.PROCESSING;
    }
    void Update()
    {
        switch (CurrentState)
        {
            case (TurnState.PROCESSING):
                UpdateHitPoints();
                break;
            case (TurnState.ADDTOLIST):
                break;
            case (TurnState.WAITING):
                break;
            case (TurnState.SELECTING):
                break;
            case (TurnState.ACTION):
                break;
            case (TurnState.DEAD):
                break;
        }
    }
    private void UpdateHitPoints()
    {
        //float calculateHpPercentage = hero.CurrentHP / hero.BaseHP;
        //HP_Bar.transform.localScale = new Vector3(Mathf.Clamp(calculateHpPercentage, 0, 1), HP_Bar.transform.localScale.y, HP_Bar.transform.localScale.z);
        CurrentState = TurnState.ADDTOLIST; 
    }
}
