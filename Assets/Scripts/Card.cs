using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Card : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnCardStatsUpdate))] public CardStats cardStats;
    [SyncVar(hook = nameof(OnCardNameUpdate))] public string cardName;
    [SyncVar] public string cardDescription;
    [SyncVar(hook = nameof(OnCardCostUpdate))] public int cardCost;

    [SerializeField] CardUI _cardUI;

    Player _ownerOnServer;

    public Player ownerOnServer //which of the copy of the players on the server owns this card
    {
        get { return _ownerOnServer; }
        set { _ownerOnServer = value; }
    }

    //[refactor] hooks later
    void OnCardStatsUpdate(CardStats oldValue, CardStats newValue)
    {
        _cardUI.UpdateCardUI();
    }
    void OnCardNameUpdate(string oldValue, string newValue) => _cardUI.UpdateCardUI();
    void OnCardCostUpdate(int oldValue, int newValue) => _cardUI.UpdateCardUI();

    public void LoadCardData(CardData cardData)
    {
        cardStats = new CardStats(cardData.cardStats);

        cardName = cardData.cardName;
        cardDescription = cardData.cardDescription;
        cardCost = cardData.cardCost;
        gameObject.name = cardName;
    }

    [Command]
    public void CMDRequestPlayCard()
    {
        //mana validation here
        if (_ownerOnServer.currentMana >= cardCost)
        {
            RPCPlayCard();
            RPCDisplayPlayedCard();
        }
        else
        {
            RPCReturnToHand();
        }
    }

    [TargetRpc]
    private void RPCReturnToHand()
    {
        gameObject.GetComponent<CardDragDrop>().ResetAnchoredPosition();
    }

    [TargetRpc]
    public void RPCPlayCard()
    {
        //[Note] I'm using [TargetRpc] here because there's no need to send the
        //collection(cardsOnHand & cardsOnField) modification to the opponent's client
        CardManager.clientInstance.CMDPlayCard(this.gameObject);

        //Also, only let that specific client call for a server request in decreasing it's mana 
        //If this uses [ClientRpc] then mana would be decrease x(no. of client) times 
        Player.localPlayer.CMDDecreaseMana(cardCost);
    }

    [ClientRpc]
    public void RPCDisplayPlayedCard()
    {
        //[Note] This uses the [ClientRpc] attribute because we want both clients to update their respective fields
        //set the card to player field (lower half) if you own it and if not set the card to the opponent's field
        //also hide the card(using card back image) if you don't own it

        if (hasAuthority)
            this.transform.SetParent(BoardManager.instance.board.playerFrontlineArea.dropArea, false);
        else
            this.transform.SetParent(BoardManager.instance.board.enemyFrontlineArea.dropArea, false);

        _cardUI.EnableCardBack(false);

    }
    

    [ClientRpc]
    public void RPCUpdateStats(CardStats stats)
    {
        cardStats = stats;
        _cardUI.UpdateCardUI();
    }

    [ClientRpc]
    public void RPCDisableCard()
    {
        gameObject.SetActive(false);
    }
}
