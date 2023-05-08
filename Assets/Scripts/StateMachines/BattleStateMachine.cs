using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleStateMachine : MonoBehaviour
{
    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }
    public PerformAction BattleStates;


    public enum PlayerInput
    {
        ACTIVATE,
        WAITING,
        DONE
    }
    public PlayerInput HeroInput;
    public GameObject ActionButton;
    public GameObject MagicSkillButton;
    public GameObject EnemyButton;
    public Transform Spacer;
    private HandleTurn HeroChoice;


    [Header("UI Panels")]
    public GameObject ActionPanel;
    public GameObject EnemySelectPanel;
    public GameObject MagicPanel;
    //public GameObject PartySelectPanel;     // just in case we need it
    public Transform ActionSpacer;
    public Transform MagicSpacer;
    private List<GameObject> attackButtons = new List<GameObject>();
    private List<GameObject> enemyButtons = new List<GameObject>();

    [Header("Lists")]
    public List<GameObject> HeroesToManage = new List<GameObject>();
    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> HeroesInBattle = new List<GameObject>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        BattleStates = PerformAction.WAIT;
        HeroesToManage.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HeroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        HeroInput = PlayerInput.ACTIVATE;

        ActionPanel.SetActive(false);
        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);

        EnemyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        switch (BattleStates)
        {
            case (PerformAction.WAIT):
                if(PerformList.Count > 0)
                {
                    BattleStates = PerformAction.TAKEACTION;
                }
                break;
            case (PerformAction.TAKEACTION):
                GameObject performer = PerformList[0].AttackersGameObject;
                if(PerformList[0].Type== "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    for(int i = 0; i < HeroesInBattle.Count; i++)
                    {
                        if (PerformList[0].AttackersTarget == HeroesInBattle[i])
                        {
                            ESM.HeroToAttack = PerformList[0].AttackersTarget;
                            ESM.CurrentState = EnemyStateMachine.TurnState.ACTION;
                            break;
                        }
                        else
                        {
                            PerformList[0].AttackersTarget = HeroesInBattle[Random.Range(0, HeroesInBattle.Count)];
                            ESM.HeroToAttack = PerformList[0].AttackersTarget;
                            ESM.CurrentState = EnemyStateMachine.TurnState.ACTION;
                        }
                    }
                }
                if(PerformList[0].Type== "Hero")
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.EnemyToAttack = PerformList[0].AttackersTarget;
                    HSM.CurrentState = HeroStateMachine.TurnState.ACTION;
                }
                BattleStates = PerformAction.PERFORMACTION;
                break;
            case (PerformAction.PERFORMACTION):
                
                break;
            case (PerformAction.CHECKALIVE):
                if (HeroesInBattle.Count < 1)
                {
                    BattleStates = PerformAction.LOSE;
                    //  lose the battle
                }
                else if (EnemiesInBattle.Count < 1)
                {
                    BattleStates = PerformAction.WIN;
                    // win the battle;
                }
                else
                {
                    //  call function;
                    ClearAttackPanel();
                    HeroInput = PlayerInput.ACTIVATE;
                    BattleStates = PerformAction.WAIT;
                }
                break;
            case (PerformAction.WIN):
                Debug.Log("hey, you won the game!");
                for( int i = 0; i < HeroesInBattle.Count; i++)
                {
                    HeroesInBattle[i].GetComponent<HeroStateMachine>().CurrentState = HeroStateMachine.TurnState.WAITING;
                }
                
                SceneManager.LoadScene(0);
                break;
            case (PerformAction.LOSE):
                Debug.Log("hey, you lost the game!");
                SceneManager.LoadScene(0);
                break;
        }
        switch (HeroInput)
        {
            case (PlayerInput.ACTIVATE):
                if(HeroesInBattle.Count > 0)
                {
                    //do something
                    if (HeroesToManage.Count > 0)
                    {
                        HeroesToManage[0].GetComponent<HeroStateMachine>().ActiveUnit.SetActive(true);
                        HeroChoice = new HandleTurn();
                        HeroChoice.Attacker = HeroesToManage[0].name;
                        HeroChoice.AttackersGameObject = HeroesToManage[0];
                        HeroChoice.Type = "Hero";
                        ActionPanel.SetActive(true);
                        CreateAttackButtons();
                        HeroInput = PlayerInput.WAITING;
                    }
                }
                break;
            case (PlayerInput.WAITING):
                
                break;
            case (PlayerInput.DONE):
                HeroInputDone();
                break;
        }
    }
    public void CollectActions(HandleTurn input)
    {
        PerformList.Add(input);
    }
    public void EnemyButtons()
    {
        foreach(GameObject enemyBtn in enemyButtons)
        {
            Destroy(enemyBtn);
        }
        enemyButtons.Clear();
        // cleanup
        foreach(GameObject enemy in EnemiesInBattle)
        {
            GameObject newButton = Instantiate(EnemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();
            TMP_Text button_Text = newButton.GetComponentInChildren<TMP_Text>();
            button_Text.text = cur_enemy.enemy.Name;
            button.EnemyPrefab = enemy;
            newButton.transform.SetParent(Spacer,false);
            enemyButtons.Add(newButton);
        }
    }
    public void Input1()
    {
        HeroChoice.Attacker = HeroesToManage[0].gameObject.name;
        HeroChoice.AttackersGameObject = HeroesToManage[0].gameObject;
        HeroChoice.Type = "Hero";
        HeroChoice.ChooseAttack = HeroesToManage[0].GetComponent<HeroStateMachine>().hero.ListOfAttacks[0];
        ActionPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }
    public void Input2(GameObject chooseEnemy)
    {
        HeroChoice.AttackersTarget = chooseEnemy;
        HeroInput = PlayerInput.DONE;
    }
    private void CreateAttackButtons()
    {
        GameObject AttackButton = Instantiate(ActionButton) as GameObject;
        TMP_Text AttackButtonText = AttackButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        AttackButton.gameObject.transform.SetParent(ActionSpacer.transform, false);
        attackButtons.Add(AttackButton);
        GameObject MagicAttackButton = Instantiate(ActionButton) as GameObject;
        TMP_Text MagicAttackText = MagicAttackButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        MagicAttackText.text = "Magic";
        MagicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        MagicAttackButton.gameObject.transform.SetParent(ActionSpacer.transform, false);
        attackButtons.Add(MagicAttackButton);
        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.ListOfMagicSpells.Count > 0)
        {
            foreach(BaseAttack mAttack in HeroesToManage[0].GetComponent<HeroStateMachine>().hero.ListOfMagicSpells)
            {
                GameObject MagicButton = Instantiate(MagicSkillButton) as GameObject;
                TMP_Text MagicButtonText = MagicButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
                MagicButtonText.text = mAttack.AttackName;
                AttackButton ATB = MagicButton.GetComponent<AttackButton>();
                ATB.MagicAttackToPerform = mAttack;
                MagicButton.transform.SetParent(MagicSpacer, false);
                attackButtons.Add(MagicButton);
            }
        }
        else
        {
            MagicSkillButton.GetComponent<Button>().interactable = false;
        }
    }
    //  Choose MagicAttack if Able
    public void Input4(BaseAttack chosenMagic)
    {
        HeroChoice.Attacker = HeroesToManage[0].name;
        HeroChoice.AttackersGameObject = HeroesToManage[0].gameObject;
        HeroChoice.Type = "Hero";

        HeroChoice.ChooseAttack = chosenMagic;
        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }
    //  switch to magic attacks
    public void Input3()
    {
        ActionPanel.SetActive(false);
        MagicPanel.SetActive(true);
    }
    private void HeroInputDone()
    {
        PerformList.Add(HeroChoice);
        ClearAttackPanel();
        HeroesToManage[0].GetComponent<HeroStateMachine>().ActiveUnit.SetActive(false);
        HeroesToManage.RemoveAt(0);
        HeroInput = PlayerInput.ACTIVATE;
    }
    private void ClearAttackPanel()
    {
        EnemySelectPanel.SetActive(false);
        ActionPanel.SetActive(false);
        MagicPanel.SetActive(false);

        foreach(GameObject atkBtn in attackButtons)
        {
            Destroy(atkBtn.gameObject);
        }
        attackButtons.Clear();

    }
}
