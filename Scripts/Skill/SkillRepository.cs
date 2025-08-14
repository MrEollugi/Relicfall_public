using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// CSV→JSON으로 저장된 skills.json을 런타임에 파싱해서 id→SkillData 매핑 제공.
// 어디서든 SkillRepository.Get(id)로 스킬 정보를 가져올 수 있습니다.
public static class SkillRepository
{
    // SkillDataListWrapper는 반드시 [System.Serializable]이어야 함
    [System.Serializable]
    private class SkillDataListWrapper
    {
        public List<SkillData> skills;
    }

    private static Dictionary<string, SkillData> skillDict;
    private static bool isLoaded = false;

    // 내부에서 자동 초기화 (최초 Get 등에서만 로드됨)
    private static void EnsureLoaded()
    {
        if (isLoaded && skillDict != null)
            return;

        // Resources 폴더 하위에 skills.json or skills.txt로 두세요.
        // 실제 빌드시 .txt를 권장 (유니티 특성상 json 확장자는 리소스에 포함 안될 수도 있음)
        TextAsset jsonAsset = Resources.Load<TextAsset>("Data/skills");
        Debug.Log($"skills.json = {jsonAsset.text}");
        if (jsonAsset == null)
        {
            Debug.LogError("[SkillRepository] skills.json(.txt) 파일을 Resources/Data/ 에 넣어주세요.");
            skillDict = new Dictionary<string, SkillData>();
            isLoaded = true;
            return;
        }

        SkillDataListWrapper wrapper = JsonUtility.FromJson<SkillDataListWrapper>(jsonAsset.text);
        if (wrapper == null || wrapper.skills == null)
        {
            Debug.LogError("[SkillRepository] skills.json 파싱 실패!");
            skillDict = new Dictionary<string, SkillData>();
            isLoaded = true;
            return;
        }

        skillDict = wrapper.skills
            .Where(s => !string.IsNullOrEmpty(s.id))
            .ToDictionary(s => s.id, s => s);

        isLoaded = true;
    }

    // 스킬 ID로 SkillData 반환. 없으면 null.
    public static SkillData Get(string id)
    {
        EnsureLoaded();
        if (string.IsNullOrEmpty(id)) return null;
        return skillDict.TryGetValue(id, out var s) ? s : null;
    }

    // 전체 SkillData 리스트 반환 (읽기전용)
    public static IReadOnlyList<SkillData> GetAll()
    {
        EnsureLoaded();
        return skillDict.Values.ToList();
    }

#if UNITY_EDITOR
    // 에디터에서 skills.json 변경시 강제로 리로드 (핫리로드용)
    [UnityEditor.MenuItem("Tools/SkillRepository/Reload Skills (Runtime Only)")]
    public static void ForceReload()
    {
        isLoaded = false;
        skillDict = null;
        Debug.Log("[SkillRepository] Skill 데이터 강제 리로드 완료");
    }
#endif

}
