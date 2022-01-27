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

    private void OnHasStartedChanged(bool old, bool newValue)
    {
        _waitingPanel.SetActive(false);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (_waitingPanel.activeInHierarchy && hasStarted) //for turning off server waiting UI
            _waitingPanel.SetActive(false);
    }

    public void StartGame() 
    {
        Debug.Log("START");
        hasStarted = true;
        OnGameStart();
    }

}
