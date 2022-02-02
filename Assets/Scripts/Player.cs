using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public class Player : NetworkBehaviour
{
    [SyncVar] int _currentHealth;
    [SyncVar] int _currentMana;

    public int currentMana { get { return _currentMana; }} 

    public static Player localPlayer;

    public override void OnStartLocalPlayer()
    {
        localPlayer = this;
    }

    private void Awake()
    {
        GameManager.instance.OnGameStart += InitializePlayerValues;
    }

    private void Start()
    {
        TurnManager.instance.OnStartPhase += AddMana;
    }

    [Server]
    private void InitializePlayerValues()
    {
        _currentHealth = 30;
        _currentMana = 0;

        //using RPCs instead of syncvar hooks since we need to properly update the UI depending on which side the client is in
        RPCUpdatePlayerValues(_currentMana, _currentHealth);
    }

    [ClientRpc]
    private void RPCUpdatePlayerValues(int currentMana, int currentHealth)
    {
        if (isLocalPlayer)
        {
            BoardManager.instance.playerInfo.manaText.text = "Mana: " + currentMana;
            BoardManager.instance.playerInfo.healthText.text = "Health: " + currentHealth;
        }
        else
        {
            BoardManager.instance.opponentInfo.manaText.text = "Mana: " + currentMana;
            BoardManager.instance.opponentInfo.healthText.text = "Health: " + currentHealth;
        }
    }

    [Command]
    public void CMDDecreaseMana(int cardCost)
    {
        //only allow change of mana through the server 
        _currentMana -= cardCost;
        RPCUpdatePlayerValues(_currentMana, _currentHealth);
    }

    //TurnManager event subscribers
    [Server]
    private void AddMana(int mana)
    {
        _currentMana += mana;
        RPCUpdatePlayerValues(_currentMana, _currentHealth);
    }
}
