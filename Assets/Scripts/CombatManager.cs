using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CombatManager : NetworkBehaviour
{
    public static CombatManager instance;

    [SyncVar] int _readyPlayersCount;

    [Client]
    public void EndMainPhaseBtn(Button endButton)
    {
        endButton.interactable = false;
        CMDUpdateReadyCount();
    }

    [Command(requiresAuthority = false)]
    public void CMDUpdateReadyCount()
    {
        _readyPlayersCount++;
        if (_readyPlayersCount >= 2)
            StartCombatEvaluation();
    }

    [Server]
    private void StartCombatEvaluation()
    {
        Debug.Log("Start Combat");
    }

}
