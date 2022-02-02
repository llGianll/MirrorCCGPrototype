using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;

public class TurnManager : NetworkBehaviour
{
    [Header("UI References")]
    [SerializeField] TMP_Text _turnPhaseText;
    [SerializeField] TMP_Text _turnCountText;

    [Header("Fields")]
    [SerializeField] float _phaseChangeDelay = 0.5f;

    [SyncVar(hook = nameof(SyncOnTurnCountChanged))] int _currentTurnCount;
    [SyncVar(hook = nameof(SyncOnTurnPhaseChanged))] TurnPhase _currentPhase= TurnPhase.End;

    public static TurnManager instance;

    //turn phase events, [refactor] make a class for every phase instead, since right now you need to add an event here everytime you edit the enum 
    //editing on multiple places can be error-prone, but this is good for now
    public event Action<int> OnStartPhase = delegate { };
    public event Action OnDrawPhase = delegate { };
    public event Action OnMainPhase = delegate { };
    public event Action OnCombatPhase = delegate { };
    public event Action OnEndPhase = delegate { };

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GameManager.instance.OnGameStart += StartGame;
    }

    private void StartGame()
    {
        //this only executes on the server since this is a non-spawned networked game object 
        ManualPhaseChange(TurnPhase.Start, 0.1f);
    }

    [Server]
    public void ManualPhaseChange(TurnPhase turnPhase, float changeDelay)
    {
        StartCoroutine(PhaseChangeTimer(turnPhase ,changeDelay));
    }

    [Server]
    private IEnumerator PhaseChangeTimer(TurnPhase turnPhase,float changeDelay)
    {
        yield return new WaitForSeconds(changeDelay);
        _currentPhase = turnPhase;
        ProcessPhaseChange();

    }

    [Server]
    private void ProcessPhaseChange()
    {
        switch (_currentPhase)
        {
            case TurnPhase.Start:
                _currentTurnCount++;
                OnStartPhase(_currentTurnCount);
                ManualPhaseChange(TurnPhase.Draw, _phaseChangeDelay);
                break;
            case TurnPhase.Draw:
                OnDrawPhase();
                ManualPhaseChange(TurnPhase.Main, _phaseChangeDelay);
                break;
            case TurnPhase.Main: //manual phase change through end main phase button
                OnMainPhase();
                break;
            case TurnPhase.Combat: //manual phase change through CombatManager
                OnCombatPhase();
                break;
            case TurnPhase.End:
                OnEndPhase();
                ManualPhaseChange(TurnPhase.Start, _phaseChangeDelay);
                break;
            default:
                break;
        }
    }

    //SyncVar hooks 
    public void SyncOnTurnCountChanged(int oldValue, int newValue) => _turnCountText.text = "Turn " + newValue;
    public void SyncOnTurnPhaseChanged(TurnPhase oldValue, TurnPhase newValue)
    {
        _turnPhaseText.text = newValue.ToString() + " Phase";
    }
}

public enum TurnPhase
{
    Start, 
    Draw,
    Main, 
    Combat, 
    End
}
