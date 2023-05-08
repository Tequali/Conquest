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
    public GameObject ActiveUnit;
    public Image HP_Bar;

    public float Max_Cooldown = 5f;
    private float cur_Cooldown = 0f;

    private bool actionStarted;
    private bool alive = true;

    private BattleStateMachine BSM;
    private void Start()
    {
        CurrentState = TurnState.PROCESSING;
        BSM = GameObject.FindObjectOfType<BattleStateMachine>();
    }
    void Update()
    {
        switch (CurrentState)
        {
            case (TurnState.PROCESSING):
                UpdateProgressBar();
                break;
            case (TurnState.CHOOSEACTION):
                if (BSM.HeroesInBattle.Count > 0)
                {
                    ChooseAction();
                }
                CurrentState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
                //mach schleife solange liste nicht alle aktiven teilnehmer registriert
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEAD):
                if (!alive)
                {
                    return;
                }
                else
                {
                    //  change tag
                    this.gameObject.tag = "DeadEnemy";
                    // cannot be targeted anymore by heroes
                    BSM.EnemiesInBattle.Remove(this.gameObject);
                    // disable selector
                    ActiveUnit.SetActive(false);
                    for (int i=0; i < BSM.PerformList.Count; i++)
                    {
                        if (BSM.PerformList[i].AttackersGameObject == this.gameObject)
                        {
                            BSM.PerformList.Remove(BSM.PerformList[i]);
                        }
                    }
                    //  change color to gray
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    // set alive to be false
                    alive = false;

                    //  reset enemy buttons
                    BSM.EnemyButtons();
                    // check alive
                    BSM.BattleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                }
                break;
        }
    }
    private void UpdateProgressBar()
    {
        cur_Cooldown += Time.deltaTime;
        if (cur_Cooldown >= Max_Cooldown)
        {
            CurrentState = TurnState.CHOOSEACTION;
        }
    }
    private void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.Attacker = enemy.Name;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.HeroesInBattle[Random.Range(0, BSM.HeroesInBattle.Count)];

        int num = Random.Range(0, enemy.ListOfAttacks.Count);
        myAttack.ChooseAttack = enemy.ListOfAttacks[num];
        Debug.Log(this.gameObject + " has chosen " + myAttack.ChooseAttack.AttackName+ " and does "+ myAttack.ChooseAttack.AttackDamage);
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
        yield return new WaitForSeconds(1f);
        // do damage
        DoDamage();
        //animate back to start
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

        //remove from performer list in bsm && this causes performers to rotate
        BSM.PerformList.RemoveAt(0);

        //reset bsm -> wait
        BSM.BattleStates = BattleStateMachine.PerformAction.WAIT;

        actionStarted = false;
        //reset this enemy
        CurrentState = TurnState.PROCESSING;
    }
    private void DoDamage()
    {
        float calc_Damage = enemy.Cur_Atk + BSM.PerformList[0].ChooseAttack.AttackDamage;
        HeroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calc_Damage);
    }
    public void TakeDamage(float getDamageAmount)
    {
        enemy.CurrentHP -= getDamageAmount;
        if (enemy.CurrentHP <= 0)
        {
            enemy.CurrentHP = 0;
            CurrentState = TurnState.DEAD;
        }
    }
}
