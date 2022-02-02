using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class CardManager : NetworkBehaviour
{
    [SerializeField] DeckData _deckData;
    [SerializeField] GameObject _cardPrefab;

    public readonly SyncList<GameObject> _cardsOnDeck = new SyncList<GameObject>();
    public readonly SyncList<GameObject> _cardsOnHand = new SyncList<GameObject>();
    public readonly SyncList<GameObject> _cardsOnField = new SyncList<GameObject>();
    public readonly SyncList<GameObject> _cardGraveyard = new SyncList<GameObject>();

    List<GameObject> _tempCardsOnDeck = new List<GameObject>(); //used for shuffling

    public static CardManager clientInstance;

    public override void OnStartLocalPlayer()
    {
        clientInstance = this;
    }

    private void Awake()
    {
        GameManager.instance.OnGameStart += RPCStartGame;
        TurnManager.instance.OnDrawPhase += RPCDrawCard;
    }

    [ClientRpc]
    private void RPCDrawCard()
    {
        CmdDrawCards(1);
    }

    [ClientRpc]
    public void RPCStartGame()
    {
        CmdLoadDeck();
        CmdDrawCards(3);
    }

    [Command]
    private void CmdLoadDeck()
    {
        foreach (var cardData in _deckData.deck)
        {
            GameObject card = Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity);
            card.GetComponent<Card>().LoadCardData(cardData);
            card.GetComponent<Card>().ownerOnServer = this.GetComponent<Player>();
            NetworkServer.Spawn(card, connectionToClient);
            card.transform.SetParent(this.transform);
            _cardsOnDeck.Add(card);
        }

        ShuffleDeck();
    }

    [Server]
    private void ShuffleDeck()
    {
        //needs to use [Server] attribute because this function is called by a command which is only
        //allowed to execute either server code or RPCs

        //perform a member copy of the _cardsOnDeck list
        foreach (var card in _cardsOnDeck)
            _tempCardsOnDeck.Add(card);

        _cardsOnDeck.Clear();

        while (_tempCardsOnDeck.Count > 1)
        {
            int index = UnityEngine.Random.Range(0, _tempCardsOnDeck.Count);
            _cardsOnDeck.Add(_tempCardsOnDeck[index]);
            _tempCardsOnDeck.RemoveAt(index);
        }
    }

    [Command]
    private void CmdDrawCards(int drawCount)
    {
        for (int i = 0; i < drawCount; i++)
            _cardsOnHand.Add(_cardsOnDeck[i]);

        //SyncLists doesn't have a RemoveRange()
        for (int i = 0; i < drawCount; i++)
        {
            _cardsOnDeck.RemoveAt(0);
        }

        //client/s card draw 
        foreach (var card in _cardsOnHand)
        {
            RPCDrawCards(card);
        }
        
    }

    [ClientRpc]
    private void RPCDrawCards(GameObject card)
    {
        if(hasAuthority)
            card.transform.SetParent(BoardManager.instance.board.playerHandArea.dropArea, false);
        else
        {
            card.GetComponent<CardUI>().EnableCardBack(true);
            card.transform.SetParent(BoardManager.instance.board.enemyCardArea.dropArea, false);
        }

    }

    [Command]
    public void CMDPlayCard(GameObject card)
    {
        //[refactor] create a more generic class for handling collection of cards(ex: hands, deck, graveyard, etc.) with their own remove functions
        _cardsOnHand.Remove(card);
        _cardsOnField.Add(card);
    }


    [Server]
    public void RemoveCardFromField(GameObject card)
    {
        _cardsOnField.Remove(card);
        _cardGraveyard.Add(card);
    }

}
