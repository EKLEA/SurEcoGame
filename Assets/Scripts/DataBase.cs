using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataBase<TKey, TValue> where TValue : UnityEngine.Object
{
	private Dictionary<TKey, TValue> dataBase = new Dictionary<TKey, TValue>();

	public DataBase(string resourcePath, System.Func<TValue, TKey> keySelector)
	{
		dataBase.Clear();
		TValue[] assets = Resources.LoadAll<TValue>(resourcePath);
		foreach (TValue obj in assets)
		{
			TKey key = keySelector(obj);
			if (!dataBase.ContainsKey(key))
			{
				dataBase.Add(key, obj);
			}
		}
	}

	public void Add(TKey key, TValue value)
	{
		if (!dataBase.ContainsKey(key))
		{
			dataBase.Add(key, value);
		}
	}

	public TValue GetInfo(TKey key)
	{
		if (dataBase.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}
}
