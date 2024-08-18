using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : UIItem, IBeginDragHandler, IDragHandler, IEndDragHandler
{	
	[HideInInspector] public Transform parentAfterDrag;	
	RectTransform _rectTransform=> GetComponent<RectTransform>();
	Canvas canvas => GetComponentInParent<Canvas>();
	
	public void OnBeginDrag(PointerEventData eventData)
	{
		parentAfterDrag=_rectTransform.parent;
		_rectTransform.SetParent(canvas.transform);
		_rectTransform.SetAsLastSibling();
		image.raycastTarget=false;
		PlayerController.Instance.playerState.isDragging=true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		_rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

	}

	public void OnEndDrag(PointerEventData eventData)
	{
		
		_rectTransform.SetParent(parentAfterDrag);
		//_rectTransform.localScale=Vector3.one;
		_rectTransform.SetAsFirstSibling();
		image.raycastTarget=true;
		
	}
}
	