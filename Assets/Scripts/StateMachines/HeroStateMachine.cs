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
    public BattleStateMachine BSM;

    public GameObject ActiveUnit;
    public GameObject EnemyToAttack;
    private Image ProgressBar;

    public float Max_Cooldown = 5f;
    private float cur_Cooldown = 0f;

    private bool alive = true;
    private bool actionStarted = false;
    private Vector3 startPosition;

    private HeroPanelStats stats;
    public GameObject HeroPanel;
    private Transform HeroPanelSpacer;

    void Start()
    {
        //  find Hero Spacer
        HeroPanelSpacer = GameObject.Find("BattleCanvas/HeroPanel/HeroSpacer").transform;
        //  fill hero panel with info
        CreateHeroPanel();

        BSM = GameObject.FindObjectOfType<BattleStateMachine>();
        CurrentState = TurnState.PROCESSING;
        ActiveUnit.SetActive(false);
        startPosition = transform.position;
    }
    void Update()
    {
        switch (CurrentState)
        {
            case (TurnState.ADDTOLIST):
                BSM.HeroesToManage.Add(this.gameObject);
                CurrentState = TurnState.WAITING;
                break;
            case (TurnState.PROCESSING):
                UpdateProgressBar();
                break;
            case (TurnState.WAITING):
                //      idle
                break;
            case (TurnState.SELECTING):
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEAD):
                if(!alive)
                {
                    return;
                }
                else
                {
                    // Change Tag for revive or something
                    this.gameObject.tag = "DeadHero";
                    // remove from target list for enemy
                    for (int i = 0; i < BSM.HeroesInBattle.Count; i++)
                    {
                        if (BSM.HeroesInBattle[i] == this.gameObject)
                        {
                            BSM.HeroesInBattle.Remove(BSM.HeroesInBattle[i]);
                        }
                    }
                    // cannot choose attacks
                    for (int i = 0; i < BSM.HeroesToManage.Count; i++)
                    {
                        if (BSM.HeroesToManage[i] == this.gameObject)
                        {
                            BSM.HeroesToManage.Remove(BSM.HeroesToManage[i]);
                        }
                    }
                    // deactivate selector
                    ActiveUnit.SetActive(false);
                    // reset gui
                    BSM.ActionPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    // remove from perform list
                    for(int i =0;i< BSM.PerformList.Count; i++)
                    {
                        if (BSM.PerformList[i].AttackersGameObject == this.gameObject)
                        {
                            BSM.PerformList.Remove(BSM.PerformList[i]);
                        }
                    }
                    // change color / play death animation
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //reset Hero input
                    BSM.BattleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                }
                break;
        }
    }
    
    private void UpdateProgressBar()
    {
        cur_Cooldown += Time.deltaTime;
        float calc_cooldown = cur_Cooldown / Max_Cooldown;
        if(cur_Cooldown>= Max_Cooldown)
        {
            CurrentState = TurnState.ADDTOLIST;
        }
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

        //remove from performer list in bsm
        BSM.PerformList.RemoveAt(0);
        if (BSM.BattleStates != BattleStateMachine.PerformAction.WIN && BSM.BattleStates != BattleStateMachine.PerformAction.LOSE)
        {
            //reset bsm -> wait
            BSM.BattleStates = BattleStateMachine.PerformAction.WAIT;
            cur_Cooldown = 0;
        //reset this enemy
        CurrentState = TurnState.PROCESSING;
        }
        else
        {
            CurrentState = TurnState.WAITING;
        }

        actionStarted = false;
    }
    public void TakeDamage(float getDamageAMount)
    {
        hero.CurrentHP -= getDamageAMount;
        if (hero.CurrentHP <=0)
        {
            hero.CurrentHP = 0;
            CurrentState = TurnState.DEAD;
        }
        UpdateHeroPanel();
    }
    // do damage
    private void DoDamage()
    {
        float calc_damage = hero.Cur_Atk + BSM.PerformList[0].ChooseAttack.AttackDamage;
        EnemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
    }
    private void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.HeroName.text = hero.Name;
        stats.HeroHP.text = "HP: "+hero.CurrentHP+"/"+hero.BaseHP;
        stats.HeroMP.text = "MP: "+hero.CurrentMorale+"/"+hero.BaseMorale;
        
        ProgressBar = stats.ProgressBar;
        HeroPanel.transform.SetParent(HeroPanelSpacer,false);
    }
    private void UpdateHeroPanel()
    {
        stats.HeroHP.text = "HP: " + hero.CurrentHP + "/" + hero.BaseHP;
        stats.HeroMP.text = "MP: " + hero.CurrentMorale + "/" + hero.BaseMorale;
    }
}
