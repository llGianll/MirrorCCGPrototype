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

    public Player ownerOnServer { set { _ownerOnServer = value; } } //which of the copy of the players on the server owns this card

    //[refactor] hooks later
    void OnCardStatsUpdate(CardStats oldValue, CardStats newValue) => _cardUI.UpdateCardUI();
    void OnCardNameUpdate(string oldValue, string newValue) => _cardUI.UpdateCardUI();
    void OnCardCostUpdate(int oldValue, int newValue) => _cardUI.UpdateCardUI();


    public void LoadCardData(CardData cardData)
    {
        cardStats = cardData.cardStats;
        cardName = cardData.cardName;
        cardDescription = cardData.cardDescription;
        cardCost = cardData.cardCost;
    }

    [Command]
    public void CMDRequestPlayCard()
    {
        //mana validation here
        if(_ownerOnServer.currentMana >= cardCost)
        {
            RPCPlayCard();
            RPCDisplayPlayedCard();
        }

    }

    [ClientRpc]
    public void RPCDisplayPlayedCard() 
    {
        if (hasAuthority)
            this.transform.SetParent(BoardManager.instance.board.playerFrontlineArea.dropArea, false);
        else
            this.transform.SetParent(BoardManager.instance.board.enemyFrontlineArea.dropArea, false);

        _cardUI.enableCardBack(false);
        
    }

    [TargetRpc]
    public void RPCPlayCard()
    {
        Deck.ownDeck.CMDRemoveCardFromHand(this.gameObject);
        Player.localPlayer.CMDDecreaseMana(cardCost);
    }
}
