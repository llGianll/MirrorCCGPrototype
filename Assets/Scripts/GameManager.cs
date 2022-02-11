using UnityEngine;
using Mirror;
using System;

public class GameManager : NetworkBehaviour
{
    [SerializeField] GameObject _waitingPanel;
    public static GameManager instance;

    [SyncVar(hook = nameof(OnHasStartedChanged))]
    public bool hasStarted;

    public event Action OnGameStart = delegate { };

    //syncvar hook executes on clients
    private void OnHasStartedChanged(bool old, bool newValue) => _waitingPanel.SetActive(false);
    private void Awake() => instance = this;

    public void StartGame()
    {
        hasStarted = true; 
        OnGameStart();
    }
    public override void OnStartClient() => Debug.Log("Client Started");

    #region Unnecessary code for turning off the server build waiting panel UI since it's a headless server build anyways
    private void Update()
    {
        if (_waitingPanel.activeInHierarchy && hasStarted) //for turning off server waiting UI
            _waitingPanel.SetActive(false);
    } 
    #endregion
}
