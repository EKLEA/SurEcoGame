using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReadOnlyInventorySlot
{
	event Action<string> ItemIDChanged;
	event Action<int > ItemAmountChanged;
	string ItemID{get;}
	int Amount{get;}
	bool IsEmpty{get;}
}