using UnityEngine;
using System;

public class FastSlotsHudView : MonoBehaviour
{
	[SerializeField] HudSlotView[] _hudSlots;
	public HudSlotView[] hudSlots=> _hudSlots;
}
