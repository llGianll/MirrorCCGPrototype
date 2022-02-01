using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

[System.Serializable]
public class CardsOnCombat
{
    public GameObject attackingCard;
    public bool isDead = false;

    public CardsOnCombat(GameObject card, bool isDead)
    {
        attackingCard = card;
        this.isDead = isDead;
    }
}

public class CombatManager : NetworkBehaviour
{
    public static CombatManager instance;

    public List<CardsOnCombat> _cardsOnCombat = new List<CardsOnCombat>();

    [SyncVar] int _readyPlayersCount;

    List<CardManager> _cardManagers = new List<CardManager>(); //player cardManager references

    private void Awake()
    {
        instance = this;
    }

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
            CombatEvaluation();
    }

    [Server]
    private void CombatEvaluation()
    {
        //[refactor] unoptimized & unclean code, does the job for now
        Debug.Log("Start Combat");
        if (_cardManagers.Count <= 0)
            _cardManagers = FindObjectsOfType<CardManager>().ToList();

        if (_cardManagers.Count <= 0)
            return;

        _cardsOnCombat.Clear();

        var tempList = _cardManagers[0]._cardsOnField.Concat(_cardManagers[1]._cardsOnField)
                                         .OrderByDescending(x => x.GetComponent<Card>().cardStats.speed)
                                         .ToList();

        foreach (var cardGO in tempList)
        {
            CardsOnCombat cardEntry = new CardsOnCombat(cardGO, false);
            _cardsOnCombat.Add(cardEntry);
        }

        StartCoroutine(UnitCombat());
    }

    [Server]
    IEnumerator UnitCombat()
    {
        //[refactor] too much GetComponent calls, there should be a better way to structure this
        //It will be better if the server has its own version of the board state, but for now this unoptmized code will do
        foreach (var card in _cardsOnCombat)
        {
            if (card.isDead) //skip the card when it died before its turn 
                continue;

            //identify which is the opponent player 
            CardManager opponentCardManager = new CardManager();
            foreach (var manager in _cardManagers)
            {
                if (manager.GetComponent<Player>() != card.attackingCard.GetComponent<Card>().ownerOnServer)
                {
                    opponentCardManager = manager;
                    break;
                }
            }

            //get first card on field of that opponent 
            Card opponentCard = opponentCardManager._cardsOnField.FirstOrDefault()?.GetComponent<Card>();

            //[Todo]: add opponent player targeting here later 
            if (opponentCard == null)
            {
                Debug.Log("No Opponent Unit on Board!");
                continue;
            }

            opponentCard.cardStats.health -= card.attackingCard.GetComponent<Card>().cardStats.attack;

            //without using custom serializers, syncvar will not automatically synchronize custom data types values
            //which means that syncVar won't work for the CardStats class
            //so we instead send an rpc to the opponent card for it to decrease its health
            opponentCard.RPCUpdateStats(opponentCard.cardStats); //rpc to send stat update back to clients 

            //check for attacked unit's death
            if (opponentCard.cardStats.health <= 0)
            {
                opponentCard.ownerOnServer.gameObject.GetComponent<CardManager>().RemoveCardFromField(opponentCard.gameObject);
                opponentCard.RPCDisableCard();
                var attacker = _cardsOnCombat.Where(x => x.attackingCard == opponentCard.gameObject).FirstOrDefault();
                attacker.isDead = true;

            }
            
            yield return new WaitForSeconds(1);

        }
    }

}
