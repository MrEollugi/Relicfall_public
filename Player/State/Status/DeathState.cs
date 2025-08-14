using UnityEngine;
using System.Collections;
using System.Linq;

public class DeathState : IStatusState
{
    private readonly PlayerController player;
    private readonly PlayerStatusFSM fsm;

    public DeathState(PlayerController player, PlayerStatusFSM fsm)
    {
        this.player = player;
        this.fsm = fsm;
    }

    public void Enter()
    {
        Debug.Log("Player Died");
        player.IsDead = true;
        player.PlayerRigidbody.velocity = Vector3.zero;
        player.StopHealRoutine();

        if (player.GraphicsObject != null)
            player.GraphicsObject.SetActive(false);

        var deadObj = player.SpawnDeadOnDeath();

        player.SetDeadVisuals(true);

        if (player.DeathParticlePrefab != null)
        {
            GameObject fx = GameObject.Instantiate(player.DeathParticlePrefab, player.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            GameObject.Destroy(fx, 3f);
        }

        var items = player.Inventory.GetAllItems().ToList();
        foreach (var item in items)
        {
            player.StartCoroutine(DropItemToField(item, player.transform.position));
        }

        player.Inventory.Clear();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveCurrentState();
        }
        else
        {
            Debug.LogWarning("[DeathState] GameManager.Instance is null; cannot save on death.");
        }

        // UIManager.Instance.ShowPanel(UIPanelID.GameOver);

        player.StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(5f); // 5초 대기



        // HP/MP/스태미나 회복 (필요시)
        player.Stat.SetCurrentHP(player.Stat.MaxHP);
        player.Stat.SetCurrentMP(player.Stat.MaxMP);
        player.Stat.SetCurrentStamina(player.Stat.MaxStamina);

        // 사망 UI 닫기
        // UIManager.Instance.HidePanel(UIPanelID.GameOver);

        // HubScene(거점)으로 이동
        //UnityEngine.SceneManagement.SceneManager.LoadScene("HubScene");
        GameManager.Instance._loadingManager.StartLoad("HubScene");

        yield return null;

        player.IsDead = false;
    }

    private IEnumerator DropItemToField(ItemInstance item, Vector3 center)
    {
        // 아이템 데이터에서 Addressables 주소 얻기
        string prefabAddress = item.data.dropPrefabAddress; // ItemData에 반드시 포함되어야 함

        // 주변 랜덤 위치 계산
        float radius = 1.2f;
        Vector2 offset = Random.insideUnitCircle * radius;
        Vector3 dropPos = center + new Vector3(offset.x, 0.1f, offset.y);

        // 어드레서블 프리팹 비동기 로드 및 인스턴스화
        var loadTask = ItemAddressable.LoadPrefabAsync(prefabAddress);
        while (!loadTask.IsCompleted) yield return null;
        var prefab = loadTask.Result;
        if (prefab != null)
        {
            GameObject go = GameObject.Instantiate(prefab, dropPos, Quaternion.identity);
            var itemComp = go.GetComponent<Item>();
            if (itemComp != null)
            {
                itemComp.Init(item); // 데이터 할당(스택, 랜덤값 등)
            }
        }
    }

    public void Update(InputData input) { }

    public void Exit() { }


}