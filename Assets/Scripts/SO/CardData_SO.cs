using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class CardData_SO : ReadExcelDataBaseSO
{
	public List<Card> SkillCard; // Replace 'EntityType' to an actual type that is serializable.
}
