using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ItemDataConverter : MonoBehaviour
{
    [Header("CSV 파일명")]
    public string csvFileName = "item";
    [Header("저장할 JSON 파일명")]
    public string outputJsonFileName = "item";

    private void Awake()
    {
        TryConvertIfChanged();  // 자동 변환
    }

    #region item.csv 자동 변환
    private void TryConvertIfChanged()
    {
        TextAsset csvAsset = Resources.Load<TextAsset>("Data/" + csvFileName);
        if (csvAsset == null)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: Resources/Data/{csvFileName}");
            return;
        }

        string newJson = ConvertCSVToJsonString(csvAsset.text);
        if (string.IsNullOrEmpty(newJson)) return;

        string jsonPath = Path.Combine(Application.persistentDataPath, outputJsonFileName + ".json");

        if (File.Exists(jsonPath))
        {
            string existingJson = File.ReadAllText(jsonPath);
            if (existingJson == newJson)
            {
                //Debug.Log("CSV 내용이 이전과 같아 JSON 변환 생략.");
                return;
            }
        }

        File.WriteAllText(jsonPath, newJson);
    }

    private string ConvertCSVToJsonString(string csvText)
    {
        var lines = csvText.Split('\n');
        if (lines.Length < 2)
        {
            Debug.LogError("CSV 파일에 데이터가 없습니다.");
            return null;
        }

        var items = new List<ItemData>();
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = SplitCsvLine(line);
            string statModifiersRaw = fields.Count > 14 ? fields[14] : "";
            string skillId = fields.Count > 15 ? fields[15] : "";

            int weight = 0, value = 0, valueRange = 0, sizeX = 0, sizeY = 0, maxStack = 1;
            float effectVal = 0f;
            bool isStack = false;

            int.TryParse(fields[5], out weight);
            int.TryParse(fields[6], out value);
            int.TryParse(fields[7], out valueRange);
            int.TryParse(fields[8], out sizeX);
            int.TryParse(fields[9], out sizeY);
            bool.TryParse(fields[10].ToLower(), out isStack);
            int.TryParse(fields[11], out maxStack);
            float.TryParse(fields[13], out effectVal);

            var item = new ItemData
            {
                id = fields[0],
                name = fields[1],
                type = fields[2],
                iconPath = fields[3],
                dropPrefabAddress = fields[4],
                weight = weight,
                value = value,
                valueRange = valueRange,
                sizeX = sizeX,
                sizeY = sizeY,
                isStackable = isStack,
                maxStackCount = maxStack,
                consumableType = fields[12],
                effectValue = effectVal,
                statModifiers = ParseStatModifiers(statModifiersRaw),
                skillId = skillId,
            };

            items.Add(item);
        }

        var list = new ItemDataList { items = items };
        return JsonUtility.ToJson(list, true);
    }
    #endregion

    #region item.csv 수동 변환
    public void ConvertCSVToJSON()
    {
        TextAsset csvAsset = Resources.Load<TextAsset>("Data/" + csvFileName);
        if (csvAsset == null)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: Resources/Data/{csvFileName}");
            return;
        }

        var lines = csvAsset.text.Split('\n');
        if (lines.Length < 2)
        {
            Debug.LogError("CSV 파일에 데이터가 없습니다.");
            return;
        }

        var items = new List<ItemData>();
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = SplitCsvLine(line);

            string statModifiersRaw = fields.Count > 14 ? fields[14] : "";
            string skillId = fields.Count > 15 ? fields[15] : "";

            int weight = 0, value = 0, valueRange = 0, sizeX = 0, sizeY = 0, maxStack = 1;
            float effectVal = 0f;
            bool isStack = false;
            int.TryParse(fields[5], out weight);
            int.TryParse(fields[6], out value);
            int.TryParse(fields[7], out valueRange);
            int.TryParse(fields[8], out sizeX);
            int.TryParse(fields[9], out sizeY);
            bool.TryParse(fields[10].ToLower(), out isStack);
            int.TryParse(fields[11], out maxStack);
            float.TryParse(fields[13], out effectVal);

            var item = new ItemData
            {
                id = fields[0],
                name = fields[1],
                type = fields[2],
                iconPath = fields[3],
                dropPrefabAddress = fields[4],
                weight = weight,
                value = value,
                valueRange = valueRange,
                sizeX = sizeX,
                sizeY = sizeY,
                isStackable = isStack,
                maxStackCount = maxStack,
                consumableType = fields[12],
                effectValue = effectVal,
                statModifiers = ParseStatModifiers(statModifiersRaw),
                skillId = skillId,
            };
            items.Add(item);
        }

        var list = new ItemDataList { items = items };
        string json = JsonUtility.ToJson(list, true);

        string outPath = Path.Combine(Application.persistentDataPath, outputJsonFileName + ".json");
        File.WriteAllText(outPath, json);

        Debug.Log($"CSV → JSON 변환 완료!\n{outPath}");
    }
    #endregion

    #region StatModifier 파싱 
    private List<StatModifier> ParseStatModifiers(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "," || json == "\"\"" || json == "[]")
            return new List<StatModifier>();

        json = json.Trim().Replace('\'', '"'); 

        while (json.StartsWith("\"")) json = json.Substring(1);
        while (json.EndsWith("\"")) json = json.Substring(0, json.Length - 1);
        if (string.IsNullOrWhiteSpace(json)) return new List<StatModifier>();

        try
        {
            return JsonUtility.FromJson<StatModifierList>("{\"list\":" + json + "}").list;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"statModifiers 파싱 실패!\n입력값: {json}\n에러: {ex}");
            return new List<StatModifier>();
        }
    }

    [System.Serializable]
    public class StatModifierList
    {
        public List<StatModifier> list;
    }
    #endregion

    #region CSV 파싱
    public static List<string> SplitCsvLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string cur = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    cur += '"';
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(cur);
                cur = "";
            }
            else
            {
                cur += c;
            }
        }

        fields.Add(cur);
        return fields;
    }
    #endregion
}