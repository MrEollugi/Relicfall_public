using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ItemInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemWeightText;
    [SerializeField] private TextMeshProUGUI itemValueText;
    [SerializeField] private TextMeshProUGUI itemSizeText;
    [SerializeField] private TextMeshProUGUI itemStatText;

    #region 매핑 딕셔너리
    private static readonly Dictionary<ItemType, string> ItemTypeDict = new()
    {
        { ItemType.Consumable, "Consumable" },
        { ItemType.Relic,      "Relic" },
        { ItemType.Curios,     "Curios" }
    };

    private static readonly Dictionary<string, string> ConsumableEffectDict = new()
    {
        { "Health",  "Restores {0} health." },
        { "Mana",    "Restores {0} mana." },
        { "Stamina", "Restores {0} stamina." },
        { "Torch",   "Restores {0} brightness gauge." }
    };

    private static readonly Dictionary<StatType, string> StatTypeDict = new()
    {
        { StatType.MaxHP, "Max HP" },
        { StatType.MaxMP, "Max MP" },
        { StatType.MaxStamina, "Max Stamina" },
        { StatType.Attack, "Attack" },
        { StatType.Defense, "Defense" },
        { StatType.AttackSpeed, "Attack Speed" },
        { StatType.CritChance, "Critical Chance" },
        { StatType.CritDamage, "Critical Damage" },
        { StatType.MoveSpeed, "Move Speed" },
    };
    #endregion

    #region UI 텍스트 설정
    public void SetInfo(ItemInstance item)
    {
        if (item == null || !Enum.TryParse(item.data.type, out ItemType itemType))
        {
            ClearInfo();
            return;
        }

        itemNameText.text = item.data.name;
        itemTypeText.text = GetItemTypeName(itemType);
        itemWeightText.text = item.data.weight.ToString();
        itemValueText.text = item.ActualValue.ToString();
        itemSizeText.text = $"{item.SizeX} x {item.SizeY}";
        itemStatText.text = GetStatText(item.data, itemType);
    }

    public void ClearInfo()
    {
        itemNameText.text = string.Empty;
        itemTypeText.text = string.Empty;
        itemWeightText.text = string.Empty;
        itemValueText.text = string.Empty;
        itemSizeText.text = string.Empty;
        itemStatText.text = "\n-";
    }
    #endregion

    #region 정보 가공 및 출력 함수
    public string GetItemTypeName(ItemType type)
    {
        return ItemTypeDict.TryGetValue(type, out var str) ? str : "-";
    }

    public string GetStatText(ItemData data, ItemType type)
    {
        if (type == ItemType.Consumable)
        {
            if (ConsumableEffectDict.TryGetValue(data.consumableType, out var format))
                return string.Format(format, data.effectValue);
            return "-";
        }

        if (type == ItemType.Relic)
        {
            if (data.statModifiers == null || data.statModifiers.Count == 0)
                return "-";

            var sb = new StringBuilder();
            foreach (var stat in data.statModifiers)
            {
                if (Enum.TryParse(stat.statType, out StatType stEnum))
                {
                    string typeName = StatTypeDict.TryGetValue(stEnum, out var str) ? str : stat.statType;
                    string valueText = stat.value % 1 == 0
                        ? ((int)stat.value).ToString()
                        : stat.value.ToString("0.##");

                    sb.AppendLine($"{typeName}   +{valueText}");
                }
                else
                {
                    sb.AppendLine($"{stat.statType}   +{stat.value}");
                }
            }
            return sb.ToString();
        }

        return "-";
    }
    #endregion
}
