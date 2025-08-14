using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// CSV��JSON���� ����� skills.json�� ��Ÿ�ӿ� �Ľ��ؼ� id��SkillData ���� ����.
// ��𼭵� SkillRepository.Get(id)�� ��ų ������ ������ �� �ֽ��ϴ�.
public static class SkillRepository
{
    // SkillDataListWrapper�� �ݵ�� [System.Serializable]�̾�� ��
    [System.Serializable]
    private class SkillDataListWrapper
    {
        public List<SkillData> skills;
    }

    private static Dictionary<string, SkillData> skillDict;
    private static bool isLoaded = false;

    // ���ο��� �ڵ� �ʱ�ȭ (���� Get ����� �ε��)
    private static void EnsureLoaded()
    {
        if (isLoaded && skillDict != null)
            return;

        // Resources ���� ������ skills.json or skills.txt�� �μ���.
        // ���� ����� .txt�� ���� (����Ƽ Ư���� json Ȯ���ڴ� ���ҽ��� ���� �ȵ� ���� ����)
        TextAsset jsonAsset = Resources.Load<TextAsset>("Data/skills");
        Debug.Log($"skills.json = {jsonAsset.text}");
        if (jsonAsset == null)
        {
            Debug.LogError("[SkillRepository] skills.json(.txt) ������ Resources/Data/ �� �־��ּ���.");
            skillDict = new Dictionary<string, SkillData>();
            isLoaded = true;
            return;
        }

        SkillDataListWrapper wrapper = JsonUtility.FromJson<SkillDataListWrapper>(jsonAsset.text);
        if (wrapper == null || wrapper.skills == null)
        {
            Debug.LogError("[SkillRepository] skills.json �Ľ� ����!");
            skillDict = new Dictionary<string, SkillData>();
            isLoaded = true;
            return;
        }

        skillDict = wrapper.skills
            .Where(s => !string.IsNullOrEmpty(s.id))
            .ToDictionary(s => s.id, s => s);

        isLoaded = true;
    }

    // ��ų ID�� SkillData ��ȯ. ������ null.
    public static SkillData Get(string id)
    {
        EnsureLoaded();
        if (string.IsNullOrEmpty(id)) return null;
        return skillDict.TryGetValue(id, out var s) ? s : null;
    }

    // ��ü SkillData ����Ʈ ��ȯ (�б�����)
    public static IReadOnlyList<SkillData> GetAll()
    {
        EnsureLoaded();
        return skillDict.Values.ToList();
    }

#if UNITY_EDITOR
    // �����Ϳ��� skills.json ����� ������ ���ε� (�ָ��ε��)
    [UnityEditor.MenuItem("Tools/SkillRepository/Reload Skills (Runtime Only)")]
    public static void ForceReload()
    {
        isLoaded = false;
        skillDict = null;
        Debug.Log("[SkillRepository] Skill ������ ���� ���ε� �Ϸ�");
    }
#endif

}
