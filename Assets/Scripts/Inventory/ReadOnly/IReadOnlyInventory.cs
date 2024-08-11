using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReadOnlyInventory
{
	event Action<string,int > ItemsAdded;
	event Action<string,int > ItemsRemoved;
	
	string OwnerID { get;}
	
	int GetAmount(string itemID);
	bool Has(string itemID,int amount);
}