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

    public static CardManager clientInstance;

    public override void OnStartLocalPlayer()
    {
        clientInstance = this;
    }

    private void Start()
    {
        //GameManager.instance.OnGameStart += RPCStartGame;
    }

    [ClientRpc]
    public void RPCStartGame()
    {
        CmdLoadDeck();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdLoadDeck();
            CmdDrawCards(5);
        }
    }

    [Command]
    private void CmdLoadDeck()
    {
        Debug.Log("LOAD DECK");
        foreach (var cardData in _deckData.deck)
        {
            GameObject card = Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity);
            card.GetComponent<Card>().LoadCardData(cardData);
            card.GetComponent<Card>().ownerOnServer = this.GetComponent<Player>();
            NetworkServer.Spawn(card, connectionToClient);
            card.transform.SetParent(this.transform);
            _cardsOnDeck.Add(card);
        }
    }

    [Command]
    private void CmdDrawCards(int drawCount)
    {
        Debug.Log("Draw Starting Hand");
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
            card.GetComponent<CardUI>().enableCardBack(true);
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
