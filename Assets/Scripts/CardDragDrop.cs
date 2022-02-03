using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using System;

public class CardDragDrop : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SyncVar] bool _isPlayable = false;
    Card _card;
    RectTransform _rectTransform;
    Vector3 _defaultAnchoredPosition;

    private void Awake()
    {
        _card = GetComponent<Card>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        TurnManager.instance.OnMainPhase += EnableCardPlay;
        TurnManager.instance.OnCombatPhase += DisableCardPlay;
    }

    [Server]
    private void EnableCardPlay() => _isPlayable = true;
    [Server]
    private void DisableCardPlay() => _isPlayable = false;

    public void OnBeginDrag(PointerEventData eventData) 
    {
        _defaultAnchoredPosition = _rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(hasAuthority && _isPlayable)
            this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isPlayable)
            return;

        _card.CMDRequestPlayCard();
    }

    [Client]
    public void ResetAnchoredPosition()
    {
        _rectTransform.anchoredPosition = _defaultAnchoredPosition;
    }

}
