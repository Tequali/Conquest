using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour
{
    public UnitBlueprint enemy;
    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }
    public TurnState CurrentState;
    public GameObject HeroToAttack;
    public Image HP_Bar;
    private bool actionStarted;

    private BattleStateMachine BSM;
    void Start()
    {
        CurrentState = TurnState.PROCESSING;
        BSM = GameObject.FindObjectOfType<BattleStateMachine>();
    }
    void Update()
    {
        switch (CurrentState)
        {
            case (TurnState.PROCESSING):
                UpdateHitPoints();
                break;
            case (TurnState.CHOOSEACTION):
                ChooseAction();
                CurrentState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
                
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEAD):
                break;
        }
    }
    private void UpdateHitPoints()
    {
        //float calculateHpPercentage = enemy.CurrentHP / enemy.BaseHP;
        //HP_Bar.transform.localScale = new Vector3(Mathf.Clamp(calculateHpPercentage, 0, 1), HP_Bar.transform.localScale.y, HP_Bar.transform.localScale.z);
        CurrentState = TurnState.CHOOSEACTION;
    }
    private void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.Attacker = enemy.Name;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.HeroesInBattle[Random.Range(0, BSM.HeroesInBattle.Count)];
        BSM.CollectActions(myAttack);
    }
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        yield return new WaitForSeconds(0.5f);
        // do damage

        //animate back to start
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

        //remove from performer list in bsm
        BSM.PerformList.RemoveAt(0);

        //reset bsm -> wait
        BSM.BattleStates = BattleStateMachine.PerformAction.WAIT;

        actionStarted = false;
        //reset this enemy
        CurrentState = TurnState.PROCESSING;
    }
}
