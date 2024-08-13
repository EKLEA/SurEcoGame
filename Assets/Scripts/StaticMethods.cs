using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticMethods
{
	public static Sprite CropTextureToSprite(Texture2D texture, Vector2Int origin, int size )
	{
		if (texture == null)
		{
			Debug.LogError("Texture is null.");
			return null;
		}

		if (origin.x < 0 || origin.y < 0 || origin.x + size > texture.width || origin.y + size > texture.height)
		{
			Debug.LogError("Crop area is out of bounds.");
			return null;
		}

		// Получаем пиксели из указанной области
		Color[] pixels = texture.GetPixels(
			origin.x, 
			origin.y, 
			size, 
			size
			
		);

		// Создаем новую текстуру и заполняем ее пикселями
		Texture2D croppedTexture = new Texture2D(size, size);
		croppedTexture.SetPixels(pixels);
		croppedTexture.Apply(); // Применяем изменения

		// Создаем спрайт из новой текстуры
		Sprite croppedSprite = Sprite.Create(
			croppedTexture, 
			new Rect(0, 0, croppedTexture.width, croppedTexture.height), 
			new Vector2(0.5f, 0.5f) // Центрирование спрайта
		);

		return croppedSprite;
	}
	public static Texture2D CropTexture(Texture2D texture, Vector2Int origin, int size )
	{
		if (texture == null)
		{
			Debug.LogError("Texture is null.");
			return null;
		}

		if (origin.x < 0 || origin.y < 0 || origin.x + size > texture.width || origin.y + size > texture.height)
		{
			Debug.LogError("Crop area is out of bounds.");
			return null;
		}
		// Получаем пиксели из указанной области
		Color[] pixels = texture.GetPixels(
			origin.x, 
			origin.y, 
			size, 
			size
		);

		// Создаем новую текстуру и заполняем ее пикселями
		Texture2D croppedTexture = new Texture2D(size, size);
		croppedTexture.SetPixels(pixels);
		croppedTexture.Apply(); // Применяем изменения

		return croppedTexture;
	}
	
}
