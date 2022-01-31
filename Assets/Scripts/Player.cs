using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public class Player : NetworkBehaviour
{
    [SyncVar] int _currenthealth = 30;
    [SyncVar] int _currentMana = 5;

    public int currentMana { get { return _currentMana; }} 

    public static Player localPlayer;
    public override void OnStartLocalPlayer()
    {
        localPlayer = this;
    }

    private void Start()
    {

    }

    private void InitializePlayerValues()
    {
        _currenthealth = 30;
        _currentMana = 5;

        //RPCUpdatePlayerValues();
    }

    [ClientRpc]
    private void RPCUpdatePlayerValues(int currentMana)
    {
        if (isLocalPlayer)
            BoardManager.instance.playerInfo.manaText.text = "Mana: " + currentMana;
        else
            BoardManager.instance.opponentInfo.manaText.text = "Mana: " + currentMana;
        
    }

    [Command]
    public void CMDDecreaseMana(int cardCost)
    {
        //only allow change of mana through the server 
        _currentMana -= cardCost;
        RPCUpdatePlayerValues(_currentMana);
    }

    
}
