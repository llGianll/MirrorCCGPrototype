using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections;

[System.Serializable]
public class CardOnCombat
{
    public GameObject attackingCard;
    public bool isDead = false;

    public CardOnCombat(GameObject card, bool isDead)
    {
        attackingCard = card;
        this.isDead = isDead;
    }
}

public class CombatManager : NetworkBehaviour
{
    public static CombatManager instance;

    public List<CardOnCombat> _cardsOnCombat = new List<CardOnCombat>();

    [SerializeField] GameObject _endButton;

    [SyncVar] int _readyPlayersCount;

    List<CardManager> _cardManagers = new List<CardManager>(); //player cardManager references

    CardManager _opponentCardManager; //used in combat evaluation

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        TurnManager.instance.OnCombatPhase += CombatEvaluation;
        TurnManager.instance.OnMainPhase += ShowEndMainPhaseButton;
    }

    [Server]
    private void ShowEndMainPhaseButton() => RPCShowEndMainButton();

    [ClientRpc]
    private void RPCShowEndMainButton() => _endButton.SetActive(true);

    [Client]
    public void EndMainPhaseBtn()
    {
        _endButton.SetActive(false);
        CMDUpdateReadyCount();
    }

    [Command(requiresAuthority = false)]
    public void CMDUpdateReadyCount()
    {
        _readyPlayersCount++;
        if (_readyPlayersCount >= 2)
        {
            TurnManager.instance.ManualPhaseChange(TurnPhase.Combat, 0f);
            _readyPlayersCount = 0;
        }
    }

    [Server]
    private void CombatEvaluation()
    {
        //find CardManager objects of the players on the server
        if (_cardManagers.Count <= 0)
            _cardManagers = FindObjectsOfType<CardManager>().ToList();
        
        //_cardManager empty list check 
        if (_cardManagers.Count <= 0) 
            return;

        _cardsOnCombat.Clear(); //clear _cardsOnCombat list if there's a content from the previous turn. 

        //create a temporary list combining all card units placed on the battlefield and order the list elements 
        //through the speed stat ordered from highest to lowest
        var tempList = _cardManagers[0]._cardsOnField.Concat(_cardManagers[1]._cardsOnField)
                                         .OrderByDescending(x => x.GetComponent<Card>().cardStats.speed)
                                         .ToList();

        //add each card on the temporary list to the _cardsOnCombat list.
        foreach (var cardGO in tempList)
        {
            CardOnCombat cardEntry = new CardOnCombat(cardGO, false);
            _cardsOnCombat.Add(cardEntry);
        }

        StartCoroutine(UnitCombat());
    }

    [Server]
    IEnumerator UnitCombat()
    {
        foreach (var card in _cardsOnCombat)
        {
            if (card.isDead) //skip the card when it died before its turn 
                continue;

            IdentifyOpponentCardManager(card);

            //get first card on field of that opponent or null if there's no enemy on the field
            Card opponentCard = _opponentCardManager._cardsOnField.FirstOrDefault()?.GetComponent<Card>();

            //activate marker of attacking unit
            card.attackingCard.GetComponent<CardUI>().RPCEnableCombatMarker(true);

            //[Todo]: add opponent player targeting here later 
            if (opponentCard == null)
                yield return StartCoroutine(AttackPlayer(card));
            else
                yield return StartCoroutine(AttackCardUnit(card, opponentCard));

            yield return new WaitForSeconds(1); //delay before processing the next match up

        }

        //end of combat 
        TurnManager.instance.ManualPhaseChange(TurnPhase.End, 0.5f);
    }

    private void IdentifyOpponentCardManager(CardOnCombat card)
    {
        foreach (var manager in _cardManagers)
        {
            if (manager.GetComponent<Player>() != card.attackingCard.GetComponent<Card>().ownerOnServer)
            {
                _opponentCardManager = manager;
                break;
            }
        }
    }

    private IEnumerator AttackPlayer(CardOnCombat card)
    {
        yield return new WaitForSeconds(0.5f);

        _opponentCardManager.gameObject.GetComponent<Player>().currentHealth -= card.attackingCard.GetComponent<Card>().cardStats.attack;

        yield return new WaitForSeconds(0.5f);

        card.attackingCard.GetComponent<CardUI>().RPCDisableCombatMarker();
    }

    private IEnumerator AttackCardUnit(CardOnCombat card, Card opponentCard)
    {
        //activate marker of target unit 
        opponentCard.gameObject.GetComponent<CardUI>().RPCEnableCombatMarker(false);

        yield return new WaitForSeconds(0.5f);

        opponentCard.cardStats.health -= card.attackingCard.GetComponent<Card>().cardStats.attack;

        //without using custom serializers, syncvar will not automatically synchronize custom data types values
        //which means that syncVar won't work for the CardStats class
        //so we instead send an rpc to the opponent card for it to decrease its health
        opponentCard.RPCUpdateStats(opponentCard.cardStats); //rpc to send stat update back to clients 

        yield return new WaitForSeconds(0.5f);

        card.attackingCard.GetComponent<CardUI>().RPCDisableCombatMarker();
        opponentCard.gameObject.GetComponent<CardUI>().RPCDisableCombatMarker();

        //check for attacked unit's death
        if (opponentCard.cardStats.health <= 0)
        {
            opponentCard.ownerOnServer.gameObject.GetComponent<CardManager>().RemoveCardFromField(opponentCard.gameObject);
            opponentCard.RPCDisableCard();
            var attacker = _cardsOnCombat.Where(x => x.attackingCard == opponentCard.gameObject).FirstOrDefault();
            attacker.isDead = true;

        }
    }


}
