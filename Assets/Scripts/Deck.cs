using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Deck : NetworkBehaviour
{
    [SerializeField] DeckData _deckData;
    [SerializeField] GameObject _cardPrefab;

    public readonly SyncList<GameObject> _cardsOnDeck = new SyncList<GameObject>();
    public readonly SyncList<GameObject> _cardsOnHand = new SyncList<GameObject>();

    //public List<GameObject> _cardsOnDeck = new List<GameObject>();

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
            card.transform.SetParent(BoardManager.instance.board.playerCardArea, false);
        else
        {
            card.GetComponent<CardUI>().enableCardBack(true);
            card.transform.SetParent(BoardManager.instance.board.enemyCardArea, false);
        }

    }
}
