using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{	public Texture2D itemsTexture;
	[HideInInspector] public Transform parentAfterDrag;	
	
	[SerializeField] TMP_Text Title;
	[SerializeField] TMP_Text Amount;
	Image image=>GetComponent<Image>();
	RectTransform _rectTransform=> GetComponent<RectTransform>();
	Canvas canvas => GetComponentInParent<Canvas>();
	
	
	public void SetUpItem(string itemID,int amount)
	{
		Title.text=null;
		Amount.text=amount.ToString();
		image.sprite=StaticMethods.CropTextureToSprite(itemsTexture,InfoDataBase.ItemsDataBase.GetInfo(itemID).PixelsOffset,InfoDataBase.ItemsDataBase.GetInfo(itemID).SizeOfTexture);
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		parentAfterDrag=_rectTransform.parent;
		_rectTransform.SetParent(canvas.transform);
		_rectTransform.SetAsLastSibling();
		image.raycastTarget=false;
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
	