using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using System;

public enum GameMode
{
    Single,
    Multi
}


// 씬이 로드될 때마다( 로비 > 거점, 거점 > 던전 > 거점)
// 기존 플레이어 파괴 > SaveManager.LoadPlayer > IPlayerFactory.Spawn(...) 호출
// 위의 흐름이 OnSceneLoaded 내부에서
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //public ItemDatabase itemDatabase;

    [Header("Player")]
    public PlayerController PlayerPrefab;

    [Header("UI Prefabs")]
    public GameObject InventoryCanvasPrefab;
    public InventoryUI InventoryUIPrefab;
    public GameObject OptionCanvasPrefab;
    public OptionMenuUI OptionMenuUIPrefab;
    public GameObject DamageTextCanvasPrefab;
    public GameObject DragUICanvasPrefab;

    private SaveManager _saveManager;
    private JobSORepository _jobRepo;
    private IPlayerFactory _playerFactory;
    private PlayerController _currentPlayer;
    public PlayerController CurrentPlayer => _currentPlayer;

    // 런타임 월드 데이터
    private WorldData _worldData;
    public WorldData WorldData => _worldData;

    // 메모리용 캐시
    private PlayerData _currentData;

    private int _currentSaveSlot = 0;

    public event Action<int> OnGoldChanged;

    public LoadingManager _loadingManager;

    private GameMode _currentGameMode = GameMode.Single;
    private string _currentSaveId = "0";

    #region Map Data

    private DungeonTheme nextDungeonData;

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject);

            bool isMultiplayer = false; // 또는 설정에서 읽어오기

            _saveManager = new SaveManager();
            _jobRepo = FindObjectOfType<JobSORepository>();
            if (_loadingManager == null)
                _loadingManager = FindObjectOfType<LoadingManager>();

            if (ItemDatabase.Instance != null)
                ItemDatabase.Instance.Init();
            else
                Debug.LogError("ItemDatabase reference is null! Inspector에서 연결해주세요.");

            if (isMultiplayer == false)
            {
                LocalPlayerFactory localFactory = new LocalPlayerFactory(PlayerPrefab);
                _playerFactory = localFactory;
            }
            //else
            //{
            //    NetworkPlayerFactory networkFactory = new NetworkPlayerFactory(PlayerPrefab);
            //    _playerFactory = networkFactory;
            //}

            _currentGameMode = GameMode.Single;   // 기본값, LobbyMenu에서 바꿀 것
            _currentSaveId = "0";                 // 기본값, LobbyMenu에서 바꿀 것

            // string id, GameMode 사용!
            _currentData = _saveManager.LoadPlayer(_currentSaveId, _currentGameMode);
            if (_currentData == null)
                _currentData = new PlayerData(); // 반드시 null 방지!
            if (string.IsNullOrEmpty(_currentData.JobId))
                _currentData.JobId = "job_knight";

        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SkillRepository.GetAll();
        ProjectilePrefabRegistry.Initialize();
        DeadPrefabRegistry.Init();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetSaveSlot(string id, GameMode mode)
    {
        _currentSaveId = id;
        _currentGameMode = mode;
        _currentData = _saveManager.LoadPlayer(id, mode);
        if (_currentData == null)
            _currentData = new PlayerData();
        _worldData = _saveManager.LoadWorld(_currentSaveId, _currentGameMode);
        if (_worldData == null)
            _worldData = new WorldData();
    }

    public void SaveCurrentState()
    {
        if (_currentPlayer == null) return;
        _currentData = _currentPlayer.CollectData();
        _saveManager.SavePlayer(_currentData, _currentSaveId, _currentGameMode);
        _saveManager.SaveWorld(_worldData, _currentSaveId, _currentGameMode);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Debug.Log($"[GameManager] OnSceneLoaded 호출 → scene.name = {scene.name}");

        if (scene.name == "LobbyScene") // 로비씬 이름을 정확하게 입력!
            return;

        // destroy old player
        if (_currentPlayer != null)
        {
            Destroy(_currentPlayer.gameObject);
        }

        // Find SpawnPoint Location
        Transform spawnPoint = null;
        var spawnObj = GameObject.Find("SpawnPoint");
        if (spawnObj != null)
            spawnPoint = spawnObj.transform;

        // load saved data
        PlayerData saveData = _currentData;
        if (saveData == null)
            saveData = new PlayerData();
        if (string.IsNullOrEmpty(saveData.JobId))
            saveData.JobId = "job_knight";

        // map jobId → SO
        PlayerJobSO jobSO = _jobRepo.Get(saveData.JobId);
        // spawn at hub point for All scenes (0, 0, 0 hard-coded or hubSpawnPoint)
        Vector3 spawnPosition = spawnPoint != null
            ? spawnPoint.position
            : Vector3.zero;

        // spawn & init
        _currentPlayer = _playerFactory.Spawn(spawnPosition);
        _currentPlayer.SetMainCamera(Camera.main);

        if (InventoryCanvasPrefab != null)
        {
            GameObject canvasGO = Instantiate(InventoryCanvasPrefab);
            InventoryUI ui = canvasGO.GetComponentInChildren<InventoryUI>();
            _currentPlayer.SetInventoryUI(ui);
        }

        if (OptionCanvasPrefab != null)
        {
            GameObject canvasGO = Instantiate(OptionCanvasPrefab);
            OptionMenuUI ui = canvasGO.GetComponent<OptionMenuUI>();
            _currentPlayer.SetOptionMenuUI(ui);
        }

        if (KeyBindManager.Instance != null)
        {
            var inputAction = _currentPlayer.InputManager.GetInputActions().asset;
            KeyBindManager.Instance.LoadKeyBindings(inputAction);
        }

        if (DamageTextCanvasPrefab != null)
        {
            GameObject canvasGO = Instantiate(DamageTextCanvasPrefab);
        }

        if (DragUICanvasPrefab != null)
        {
            GameObject canvasGO = Instantiate(DragUICanvasPrefab);
        }

        PlayerUI playerUI = FindObjectOfType<PlayerUI>();
        if (playerUI != null)
        {
            playerUI.Init(_currentPlayer);
        }
        else
        {
            Debug.LogWarning("PlayerUI를 찾을 수 없습니다! PlayerUI가 씬/Canvas에 포함되어 있는지 확인하세요.");
        }

        _currentPlayer.Init(jobSO, saveData);
    }

    public void AddGold(int amount)
    {
        _worldData.TotalGold += amount;
        OnGoldChanged?.Invoke(_worldData.TotalGold);
    }

    public bool SpendGold(int amount)
    {
        if (_worldData.TotalGold >= amount)
        {
            _worldData.TotalGold -= amount;
            OnGoldChanged?.Invoke(_worldData.TotalGold);
            return true;
        }
        return false; // 잔액 부족
    }


}
