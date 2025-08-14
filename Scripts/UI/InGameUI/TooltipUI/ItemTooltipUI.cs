using TMPro;
using UnityEngine;

public class ItemTooltipUI : WorldTooltipUI
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI valueText;
    //[SerializeField] private TextMeshProUGUI weightText;
    //[SerializeField] private TextMeshProUGUI sizeText;

    public void SetData(ItemInstance item)
    {
        if (item == null)
        {
            nameText.text = "";
            valueText.text = "";
            //weightText.text = "";
            //sizeText.text = "";
            return;
        }

        nameText.text = item.data.name;
        valueText.text = item.data.value.ToString();
        //weightText.text = item.data.weight.ToString();
        //sizeText.text = $"{item.data.sizeX} x {item.data.sizeY}";
    }
}
