using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] PartyScreen partyScreen;

    public StateMachine<GameController> StateMachine { get; private set; }

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    public static GameController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        partyScreen.Init();

        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(GameFreeRoamState.Instance);

        DialogManager.Instance.OnShowDialog += () => StateMachine.PushState(GameDialogueState.Instance);
    }

    private void Update()
    {
        StateMachine.HandleUpdate();

        // Just for testing
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartBattle();
        }
        if (Input.GetKeyDown(KeyCode.P))
            Debug.Log(StateMachine.CurrentState.ToString());
    }

    public TrainerController Trainer { get; set; }
    public void StartBattle(TrainerController trainer = null)
    {
        Trainer = trainer;
        StateMachine.ChangeState(GameBattleState.Instance);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        StateMachine.ChangeState(GameBusyState.Instance);
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    public PlayerController PlayerController => playerController;
    public PartyScreen PartyScreen => partyScreen;
}
