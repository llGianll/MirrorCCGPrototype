using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using System;

public class CardDragDrop : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    bool isDraggable = true;
    Card _card;

    private void Awake()
    {
        _card = GetComponent<Card>();
    }

    public void OnBeginDrag(PointerEventData eventData) { }

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

    

}
