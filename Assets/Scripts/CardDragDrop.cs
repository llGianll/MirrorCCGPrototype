using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using System;

public class CardDragDrop : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    bool isDraggable = true;
    Card _card;
    RectTransform _rectTransform;
    Vector3 _defaultAnchoredPosition;

    private void Awake()
    {
        _card = GetComponent<Card>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
        _defaultAnchoredPosition = _rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(hasAuthority && isDraggable)
            this.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable)
            return;

        _card.CMDRequestPlayCard();
    }

    [Client]
    public void ResetAnchoredPosition()
    {
        _rectTransform.anchoredPosition = _defaultAnchoredPosition;
    }

}
