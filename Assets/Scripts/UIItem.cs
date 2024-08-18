using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIItem : MonoBehaviour
{
	public Texture2D itemsTexture;
	[SerializeField] TMP_Text Amount;
	[SerializeField] TMP_Text Title;
	[HideInInspector] public Image image=>GetComponent<Image>();
	public void SetUpItem(string itemID,int amount)
	{
		Amount.text= amount!=0 ?amount.ToString():null;
		image.sprite=StaticMethods.CropTextureToSprite(itemsTexture,InfoDataBase.ItemsDataBase.GetInfo(itemID).PixelsOffset,InfoDataBase.ItemsDataBase.GetInfo(itemID).SizeOfTexture);
	}
}