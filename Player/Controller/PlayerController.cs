using Cinemachine;
using DG.Tweening;
﻿using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour, ICombatEntity
{
    public string playerId;

    #region Stats

    public IStat Stat => stat;
    public Transform Transform => transform;

    [Header("Player Data")]
    [SerializeField] private PlayerJobSO jobData;
    [SerializeField] private PlayerStat stat;

    private PlayerData _data;

    private float staminaExhaustedCooldown = 1.0f; // 1초 쿨타임
    private float exhaustedTimer = 0f;
    public bool IsStaminaExhausted => exhaustedTimer > 0f;

    #endregion

    public ICombatEntity CurrentTarget { get; private set; }

    #region References
    [Header("References")]
    private Rigidbody playerRigidbody;
    public Rigidbody PlayerRigidbody => playerRigidbody;

    #region For Movement
    public Vector3 MoveDirection { get; set; } = Vector3.zero;
    public float MoveSpeed { get; set; } = 0f;
    #endregion

    [SerializeField] private GameObject graphicsObject;
    public GameObject GraphicsObject => graphicsObject;

    [Header("Animator")]
    [SerializeField] private AnimatorManager bodyAnimatorManager;
    public AnimatorManager BodyAnimatorManager => bodyAnimatorManager;

    [Header("Weapon Controller")]
    [SerializeField] private LeftWeaponController leftWeapon;
    [SerializeField] private RightWeaponController rightWeapon;
    public LeftWeaponController LeftWeaponController => leftWeapon;
    public RightWeaponController RightWeaponController => rightWeapon;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Camera mainCamera;

    [Header("Sound Emitter")]
    [SerializeField] private SoundEmitter soundEmitter;
    public SoundEmitter SoundEmitter => soundEmitter;

    #endregion

    #region Skills

    [Header("Skills")]
    //[SerializeField] private PlayerSkillSetSO skillSet;
    //[SerializeField] private List<SkillSO> skillList;
    //public List<SkillSO> SkillList => skillList;

    //private Dictionary<string, SkillSO> skillMap;
    private Dictionary<ESkillSlotType, string> equippedSkills = new();

    private Dictionary<string, float> lastSkillTime = new();

    public Dictionary<string, SkillData> skillDict;

    public PlayerSkillSetSO skillSet;

    #region Dash

    private bool isDashing = false;
    private float dashCooldown = 1f;
    private float lastDashTime = -9999f;
    private float dashStaminaCost = 7.5f;

    [SerializeField] private AudioClip dashClip;
    public AudioClip DashClip => dashClip;

    private Vector2 lastNonZeroMoveDirection = Vector2.zero;

    [SerializeField] private GameObject afterimagePrefab;
    [SerializeField] private float afterimageInterval = 0.025f;
    [SerializeField] private int afterimageCount = 6;

    #endregion

    #endregion

    #region FSM
    [Header("FSMs")]
    private InputManager inputManager;
    public InputManager InputManager => inputManager;

    private PlayerMoveFSM moveFsm;
    private PlayerActionFSM actionFsm;
    private PlayerStatusFSM statusFsm;
    public PlayerActionFSM ActionFSM => actionFsm;
    public PlayerMoveFSM MoveFSM => moveFsm;
    public PlayerStatusFSM StatusFSM => statusFsm;

    public bool IsDead { get; set; } = false;
    public bool IsAlive => stat.CurrentHP > 0;
    #endregion

    #region Interaction
    [Header("Interaction Settings")]
    [SerializeField, Tooltip("interaction range Mouse cursor highlight")]
    private float interactRange = 5f;
    public float InteractRange => interactRange;
    #endregion

    #region Inventory

    [Header("Inventory Settings")]
    [SerializeField] private int inventoryWidth = 5;
    [SerializeField] private int inventoryHeight = 4;
    [SerializeField] private List<InventoryData> inventoryData ;

    private int weight = 0;
    [SerializeField] private int weightLimit = 60;
    [SerializeField] private int maxWeight = 100;

    private InventoryGrid playerInventoryGrid;
    public InventoryGrid Inventory => playerInventoryGrid;

    [Header("UI")]
    private InventoryUI inventoryUI;

    private ItemInstance[] equippedRelics = new ItemInstance[2];
    public List<ItemInstance> GetEquippedRelics() => equippedRelics.ToList();

    #region Quickslot
    public ItemInstance[] QuickSlots = new ItemInstance[2];

    #endregion

    public bool wasChestJustClosed = false;

    #endregion

    #region Option

    private OptionMenuUI optionMenuUI;

    #endregion

    #region UI

    [SerializeField] private DamageOverlayUI damageOverlay;
    [SerializeField] private GameObject hudPrefab;
    private PlayerUI playerUI;
    private TimedInteractionUI interactionUI;

    private bool IsUIOpen()
    {
        return
            IsInputBlocked ||  // 입력 차단 상태
            UIManager.Instance?.IsAnyUIOpen == true ||  
            (inventoryUI?.IsOpen ?? false) ||  // 인벤토리 UI
            (optionMenuUI?.IsOpen ?? false) ||  // 옵션 메뉴 UI
            (ChestInteractable.OpenedUI != null && !wasChestJustClosed);  // 창고 UI가 켜져 있음
    }

    #endregion

    #region Sound

    [SerializeField] private AudioClip hitClip;
    public AudioClip HitClip => hitClip;

    [Header("발소리")]
    [SerializeField] private AudioClip[] walkClips;
    [SerializeField] private AudioClip[] runClips;
    public AudioClip GetRandomWalkClip() =>
    walkClips[UnityEngine.Random.Range(0, walkClips.Length)];
    public AudioClip GetRandomRunClip() =>
    runClips != null && runClips.Length > 0 ? runClips[UnityEngine.Random.Range(0, runClips.Length)] : GetRandomWalkClip();

    #endregion

    #region Guard
    [Header("Guard")]
    public bool IsGuarding = false;
    [SerializeField] private GameObject guardIndicator;
    public GameObject GuardIndicator => guardIndicator;
    [SerializeField] private GameObject guardBreakEffectPrefab;
    public GameObject GuardBreakEffect => guardBreakEffectPrefab;

    #endregion

    [Tooltip("speed multifier")]
    public float MovementSpeedModifier { get; private set; } = 1f;

    #region Input Block
    public bool IsInputBlocked { get; private set; } = false;
    public void BlockInput() => IsInputBlocked = true;
    public void UnblockInput() => IsInputBlocked = false;

    #endregion

    #region Gun Point
    [Header("Point")]
    public Transform RotationPoint;
    public Transform ProjectilePoint;
    public Transform MuzzlePoint => leftWeapon.MuzzlePoint;


    #endregion

    #region Recoil Settings

    public float recoilRadius = 0.35f;    // CapsuleCollider.radius
    public float recoilHeight = 1.8f;     // CapsuleCollider.height
    public LayerMask recoilBlockMask;     // Wall, Obstacle

    private CapsuleCollider capsule;

    public bool IsMoveBlocked { get; private set; } = false;
    public void BlockMove() => IsMoveBlocked = true;
    public void UnblockMove() => IsMoveBlocked = false;

    #endregion

    #region Coroutine Handler
    private Coroutine healCoroutine;

    private Coroutine flashRedCoroutine;

    #endregion

    private float playTimeAccum = 0f;

    #region VFX
    [SerializeField]private GameObject deathParticlePrefab;
    public GameObject DeathParticlePrefab => deathParticlePrefab;
    #endregion

    #region Init
    private void Awake()
    {
        if (string.IsNullOrEmpty(playerId))
            playerId = Guid.NewGuid().ToString();

        playerRigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        recoilRadius = capsule.radius;
        recoilHeight = Mathf.Max(capsule.height, capsule.radius * 2f);

        inputManager = new InputManager();
        stat = new PlayerStat();

        actionFsm = new PlayerActionFSM(this);
        moveFsm = new PlayerMoveFSM(this);
        statusFsm = new PlayerStatusFSM(this);

        bodyAnimatorManager = bodyAnimatorManager ?? GetComponentInChildren<AnimatorManager>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        originalColor = spriteRenderer.color;
        soundEmitter = soundEmitter ?? GetComponent<SoundEmitter>();
    }

    private void Start()
    {
        if (_data == null)
            playerInventoryGrid = new InventoryGrid(inventoryWidth, inventoryHeight);

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.RegisterInventory(Inventory);
        }

        inventoryUI.Init(inventoryWidth, inventoryHeight, Inventory, InventoryGridViewMode.Player);

        //QuickSlotPanelUI.Instance.SyncQuickSlots(QuickSlots);

        damageOverlay = FindObjectOfType<DamageOverlayUI>();

        //equippedSkills[ESkillSlotType.BasicAttack] = "atk_shotgun";
        //equippedSkills[ESkillSlotType.E] = "heal_second_wind";
        //equippedSkills[ESkillSlotType.RightClick] = "guard_guard";

        if (skillSet != null)
        {
            equippedSkills.Clear();
            equippedSkills[ESkillSlotType.BasicAttack] = skillSet.equippedBasicAttackSkillId;
            equippedSkills[ESkillSlotType.RightClick] = skillSet.equippedRightClickSkillId;
            equippedSkills[ESkillSlotType.Q] = skillSet.equippedQSkillId;
            equippedSkills[ESkillSlotType.E] = skillSet.equippedESkillId;
            equippedSkills[ESkillSlotType.Ultimate] = skillSet.equippedUltimateSkillId;
            equippedSkills[ESkillSlotType.UseItem1] = skillSet.equippedUseItem1SkillId;
            equippedSkills[ESkillSlotType.UseItem2] = skillSet.equippedUseItem2SkillId;
        }
        else
        {
            // 혹시 null일 경우를 대비한 fallback
            equippedSkills[ESkillSlotType.BasicAttack] = "atk_shotgun";
            equippedSkills[ESkillSlotType.E] = "heal_second_wind";
            equippedSkills[ESkillSlotType.RightClick] = "guard_guard";
        }

        Debug.Log("Equipped Skills: " + string.Join(", ", equippedSkills));

        var hudGo = Instantiate(hudPrefab);
        playerUI = hudGo.GetComponentInChildren<PlayerUI>();
        if (playerUI != null) playerUI.Init(this);
        interactionUI = hudGo.GetComponentInChildren<TimedInteractionUI>();

        #region Interaction Manager

        var interactionManager = GetComponent<InteractionManager>();
        if (interactionManager != null)
        {
            interactionManager.player = this;
            interactionManager.interactionUI = interactionUI;
        }
        else
        {
            // 필요하다면 동적으로 붙여주는 코드 (안 붙어있으면)
            interactionManager = gameObject.AddComponent<InteractionManager>();
            interactionManager.player = this;
            interactionManager.interactionUI = interactionUI;
        }

        #endregion

        var asset = inputManager.GetInputActions().asset;
        if (PlayerPrefs.HasKey("keybindings"))
        {
            string json = PlayerPrefs.GetString("keybindings");
            asset.LoadBindingOverridesFromJson(json);
        }
    }

    private void OnEnable()
    {
        inputManager.Enable();
    }

    private void OnDisable()
    {
        inputManager.Disable();
    }
    #endregion

    #region Unity Update
    private void Update()
    {
        playTimeAccum += Time.deltaTime;

        inputManager.UpdateInputData();
        var input = inputManager.GetInput();

        #region UI 켜져있을 경우 

        // UI가 켜져 있을 경우
        if (IsUIOpen())
        {
            MoveDirection = Vector3.zero;

            // 강제로 이동 상태를 Idle로
            if (!(moveFsm.CurrentState is MoveIdleState))
            {
                moveFsm.ChangeState(new MoveIdleState(this, moveFsm));
            }

            // 회복은 허용
            if (!IsDead && !IsGuarding)
            {
                stat.Regenerate(Time.deltaTime);
            }
        }

        #endregion

        statusFsm.Update(input);

        #region Close ChestUI

        if (!wasChestJustClosed && ChestInteractable.OpenedUI != null)
        {
            if (input.isInventory || input.isOption)
            {
                ChestInteractable.OpenedUI.Close();
                wasChestJustClosed = true;
                return;
            }
        }

        #endregion

        if (IsInputBlocked) return;
        if (IsDead) return;

        if (input.isUseItem1Down) UseQuickSlot(0);
        if (input.isUseItem2Down) UseQuickSlot(1);


        if (input.MoveDirection.sqrMagnitude > 0.01f)
            lastNonZeroMoveDirection = input.MoveDirection;

        #region Update Inventory

        if (input.isInventory)
        {
            inventoryUI.Toggle();
            return;             // 인벤토리 열려 있는 동안에는 나머지 입력 무시
        }

        if (inventoryUI.IsOpen)
            return;

        #endregion

        #region MiniMap
        var MiniMapCon = MiniMapController.Instance;
        if (MiniMapCon != null)
        {
            if (input.isMiniMap)
                MiniMapCon.Toggle();

            MiniMapCon.SetCenter(transform.position);
        }
        #endregion

        #region 설정
        if (input.isOption)
        {
            optionMenuUI.Toggle();
            return;
        }

        if (optionMenuUI.IsOpen)
            return;
        #endregion

        if (UIManager.Instance.IsAnyUIOpen)
            return;

        #region ImmediateInteraction
        if (input.isInteractDown)
        {
            HandleImmediateInteraction();
            return;
        }
        #endregion

        #region Light ON/OFF
        if (input.isLightDown && PlayerSightController.Instance != null)
        {
            PlayerSightController.Instance.ToggleSight();
        }
        #endregion

        #region CollectAll
        if (input.isRootAll)
        {
            CollectNearbyItems();
            return;
        }
        #endregion

        HandleAiming(input);
        moveFsm.Update(input);
        //bool isUIOpen = (inventoryUI != null && inventoryUI.IsOpen);
        //if (!isUIOpen)
        //{
        //    actionFsm.Update(input);
        //}
        actionFsm.Update(input);

        if (exhaustedTimer > 0f)
            exhaustedTimer -= Time.deltaTime;

        if (!(moveFsm.CurrentState is MoveRunState))
        {
            if (!IsGuarding)
                stat.Regenerate(Time.deltaTime);
        }

        UpdateAiming();
    }

    private void FixedUpdate()
    {
        if (IsMoveBlocked)
        {
            playerRigidbody.velocity = Vector3.zero;
            return;
        }

        Vector3 velocity = MoveDirection * MoveSpeed;
        velocity.y = playerRigidbody.velocity.y;
        playerRigidbody.velocity = velocity;
    }

    #endregion

    #region Data

    // 
    public void Init(PlayerJobSO jobSO, PlayerData data)
    {
        // apply SO
        this.jobData = jobSO;
        // initialize stat from SO
        stat.InitializeFromJob(jobData);
        // restore runtime data
        InitializeFromData(data);
    }

    public void InitializeFromData(PlayerData data)
    {
        _data = data;

        stat.SetLevel(data.Level);

        stat.SetCurrentHP(data.CurrentHP > 0 ? data.CurrentHP : stat.MaxHP);
        stat.SetCurrentMP(data.CurrentMP > 0 ? data.CurrentMP : stat.MaxMP);
        stat.SetCurrentStamina(data.CurrentStamina > 0 ? data.CurrentStamina : stat.MaxStamina);

        // restore inventory
        playerInventoryGrid = new InventoryGrid(
            data.Inventory.Width,
            data.Inventory.Height
        );
        // Debug.Log("[PlayerController] playerInventoryGrid ref = " + playerInventoryGrid.GetHashCode());
        playerInventoryGrid.LoadFromList(data.Inventory.Items);
        InventoryManager.Instance.InventoryUI.SetGrid(playerInventoryGrid);

        if (inventoryUI != null)
            inventoryUI.UpdateUI();

        playTimeAccum = data.PlayTimeSeconds;

        // Debug.Log($"[PlayerController] Loaded {data.Inventory.Items.Count} items");

        // 4) FSM init
        //actionFsm = new PlayerActionFSM(this);
        //moveFsm   = new PlayerMoveFSM(this);
        //statusFsm = new PlayerStatusFSM(this);
    }

    // Collect current runtime state for saving.
    public PlayerData CollectData()
    {
        Debug.Log("PlayerController.playerInventoryGrid: " + playerInventoryGrid.GetHashCode());
        if (inventoryUI != null)
            Debug.Log("InventoryUI.Grid: " + inventoryUI.Grid.GetHashCode());

        return new PlayerData
        {
            Nickname = _data.Nickname,
            JobId = _data.JobId,
            Level = stat.level,
            CurrentHP = stat.CurrentHP,
            CurrentMP = stat.CurrentMP,
            CurrentStamina = stat.CurrentStamina,
            PlayTimeSeconds = (long)playTimeAccum,

            Inventory = new InventoryData
            {
                Width = playerInventoryGrid.Width,
                Height = playerInventoryGrid.Height,
                Items = playerInventoryGrid.GetAllItems()
                    .Select(item => item.ToSaveData()).ToList()
            }

        };
    }

    #endregion

    public void SetMainCamera(Camera cam)
    {
        mainCamera = cam;
    }

    public void SetStaminaExhausted()
    {
        exhaustedTimer = staminaExhaustedCooldown;
    }

    #region Inventory

    public void SetInventoryUI(InventoryUI ui)
    {
        inventoryUI = ui;
        InventoryManager.Instance.InventoryUI = ui;

        if (playerInventoryGrid != null)
        {
            // Debug.Log($"[PlayerController] Re-Init UI with {playerInventoryGrid.Width}x{playerInventoryGrid.Height}");
            inventoryUI.Init(playerInventoryGrid.Width, playerInventoryGrid.Height, playerInventoryGrid);
        }
        else
        {
            // Debug.Log($"[PlayerController] Re-Init UI with default size {inventoryWidth}x{inventoryHeight}");
            var tempGrid = new InventoryGrid(inventoryWidth, inventoryHeight);
            inventoryUI.Init(inventoryWidth, inventoryHeight, tempGrid);
        }
    }

    public bool AddItemToInventory(ItemInstance item)
    {
        if (playerInventoryGrid.AddItem(item))
        {
            
            inventoryUI?.UpdateUI();
            return true;
        }

        return false;
    }

    public void CollectNearbyItems()
    {
        float collectRadius = 3.0f;
        Collider[] hits = Physics.OverlapSphere(transform.position, collectRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Item")) continue;

            if (hit.TryGetComponent<IImmediateInteractable>(out var interactable))
            {
                interactable.Interact(this);
            }
        }
    }

    public void ApplyWeight(ItemInstance item)
    {
        if (item == null) return;

        weight += item.data.weight;

        UpdateMovementSpeedByWeight();
    }

    public void RemoveWeight(ItemInstance item)
    {
        if (item == null) return;

        weight = Mathf.Max(0, weight - item.data.weight);
        UpdateMovementSpeedByWeight();
    }

    private void UpdateMovementSpeedByWeight()
    {
        float baseSpeed = stat.MaxMovementSpeed;

        if (weight <= weightLimit)
        {
            stat.SetCurrentMoveSpeed(baseSpeed);
            return;
        }

        if (weight > maxWeight)
        {
            stat.SetCurrentMoveSpeed(0f);
            return;
        }

        float slowRatio = (weight - weightLimit) / (float)(maxWeight - weightLimit);
        float moveSpeed = Mathf.Lerp(baseSpeed, 0f, slowRatio);
        stat.SetCurrentMoveSpeed(moveSpeed);
    }

    public bool EquipItem(ItemInstance item, int slotIndex = -1)
    {
        if (Enum.TryParse<ItemType>(item.data.type, out var type))
        {
            if (item != null && type == ItemType.Relic)
            {
                int currentIdx = Array.IndexOf(equippedRelics, item);
                if (currentIdx >= 0)
                {
                    if (slotIndex == currentIdx)
                    {
                        Debug.Log($"[EquipItem] {item.data.name} 이미 장착됨 (동일 슬롯)");
                        return false;
                    }
                    // 이동하려는 곳에 다른 유물이 있다면 → 스왑!
                    if (equippedRelics[slotIndex] != null)
                    {
                        SwapRelics(currentIdx, slotIndex);
                        Debug.Log($"[EquipItem] {item.data.name}와 {equippedRelics[currentIdx]?.data.name} 스왑 완료");
                        return true;
                    }
                    // 아니면 단순 이동
                    equippedRelics[currentIdx] = null;
                    equippedRelics[slotIndex] = item;
                    Debug.Log($"[EquipItem] {item.data.name} 슬롯 이동: {currentIdx} → {slotIndex}");
                    inventoryUI?.UpdateUI();
                    inventoryUI.RelicSlotUI?.UpdateUI();

                    if (ChestInteractable.OpenedUI != null)
                    {
                        ChestInteractable.OpenedUI.playerInventoryUI?.RelicSlotUI?.UpdateUI();
                    }

                    return true;
                }


                // 지정된 슬롯에 장착
                if (slotIndex >= 0 && slotIndex < equippedRelics.Length)
                {
                    if (equippedRelics[slotIndex] != null)
                    {
                        Debug.Log($"[EquipItem] 기존 유물({equippedRelics[slotIndex]?.data?.name}) 해제");
                        UnEquipItem(equippedRelics[slotIndex]);
                    }
                    equippedRelics[slotIndex] = item;
                    Debug.Log($"[EquipItem] {item.data.name} → 슬롯 {slotIndex}에 장착!");
                }
                else
                {
                    // 빈 슬롯 찾기
                    for (int i = 0; i < equippedRelics.Length; i++)
                    {
                        if (equippedRelics[i] == null)
                        {
                            equippedRelics[i] = item;
                            Debug.Log($"[EquipItem] {item.data.name} → 빈 슬롯 {i}에 장착!");
                            break;
                        }
                    }
                }

                RelicStat.ApplyStat(item.data, stat);
                inventoryUI?.UpdateUI();
                inventoryUI.RelicSlotUI?.UpdateUI();
                
                if (ChestInteractable.OpenedUI != null)
                {
                    ChestInteractable.OpenedUI.playerInventoryUI?.RelicSlotUI?.UpdateUI();
                }

                Debug.Log($"[EquipItem] 현재 장착 상태: " +
                    $"0: {(equippedRelics[0] != null ? equippedRelics[0].data.name : "null")}, " +
                    $"1: {(equippedRelics[1] != null ? equippedRelics[1].data.name : "null")}"
);
                return true;
            }
        }

        return false;
    }

    public bool UnEquipItem(ItemInstance item)
    {
        if (Enum.TryParse<ItemType>(item.data.type, out var type))
        {
            if (item != null && type == ItemType.Relic)
            {
                for (int i = 0; i < equippedRelics.Length; i++)
                {
                    if (equippedRelics[i] == item)
                    {
                        equippedRelics[i] = null;
                        RelicStat.RemoveStat(item.data, stat);
                        inventoryUI?.UpdateUI();
                        inventoryUI.RelicSlotUI?.UpdateUI();

                        if (ChestInteractable.OpenedUI != null)
                        {
                            ChestInteractable.OpenedUI.playerInventoryUI?.RelicSlotUI?.UpdateUI();
                        }

                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void SwapRelics(int indexA, int indexB)
    {
        if (indexA == indexB) return;

        if (indexA < 0 || indexA >= equippedRelics.Length) return;
        if (indexB < 0 || indexB >= equippedRelics.Length) return;

        ItemInstance itemA = equippedRelics[indexA];
        ItemInstance itemB = equippedRelics[indexB];

        bool isRelicA = itemA != null && Enum.TryParse<ItemType>(itemA.data.type, out var typeA) && typeA == ItemType.Relic;
        bool isRelicB = itemB != null && Enum.TryParse<ItemType>(itemB.data.type, out var typeB) && typeB == ItemType.Relic;

        if (isRelicA)
            RelicStat.RemoveStat(itemA.data, stat);
        if (isRelicB)
            RelicStat.RemoveStat(itemB.data, stat);

        // 스왑
        (equippedRelics[indexA], equippedRelics[indexB]) = (equippedRelics[indexB], equippedRelics[indexA]);

        if (isRelicA)
            RelicStat.ApplyStat(itemA.data, stat);
        if (isRelicB)
            RelicStat.ApplyStat(itemB.data, stat);

        inventoryUI?.UpdateUI();
        inventoryUI.RelicSlotUI?.UpdateUI();

        if (ChestInteractable.OpenedUI != null)
        {
            ChestInteractable.OpenedUI.playerInventoryUI?.RelicSlotUI?.UpdateUI();
        }
    }

    public bool IsEquipped(ItemInstance item)
    {
        return equippedRelics.Contains(item);
    }


    #region Quickslot

    public void UseQuickSlot(int idx)
    {
        if (idx < 0 || idx >= QuickSlots.Length) return;
        var item = QuickSlots[idx];
        if (item == null) 
        {
            Debug.Log("lol");
            return;  
        }

        Debug.Log($"[QuickSlot] 사용 전: {item.data.name}, stack={item.stackCount}");

        UseItem(item);

        var inventory = this.Inventory;
        ItemInstance found = null;

        // 1. 동일 참조(==)로 우선 찾기
        foreach (var invItem in inventory.GetAllItems())
        {
            if (invItem == item && invItem.stackCount > 0)
            {
                found = invItem;
                break;
            }
        }

        // 2. 없으면 동일 데이터 중 stackCount>0인 첫번째 인스턴스
        if (found == null)
        {
            foreach (var invItem in inventory.GetAllItems())
            {
                if (invItem.data == item.data && invItem.stackCount > 0)
                {
                    found = invItem;
                    break;
                }
            }
        }

        if (found == null)
        {
            Debug.Log($"[QuickSlot] 사용 후 {item.data.name} 남은 아이템 없음 → 퀵슬롯 비움");
            QuickSlots[idx] = null;
            QuickSlotManager.Instance.SetQuickSlot(idx, null);
        }
        else
        {
            Debug.Log($"[QuickSlot] 사용 후 {item.data.name} 여전히 남음, 스택: {found.stackCount} → 퀵슬롯에 유지");
            QuickSlots[idx] = found;
            QuickSlotManager.Instance.SetQuickSlot(idx, found);
        }
    }

    #endregion

    #endregion

    #region Option

    public void SetOptionMenuUI(OptionMenuUI ui)
    {
        optionMenuUI = ui;
    }

    #endregion

    public void SetCurrentTarget(ICombatEntity target) => CurrentTarget = target;

    #region ICombat Entity

    public void TakeDamage(float damage, Vector3 attackDirection = default)
	{
        Debug.Log($"{gameObject.name}가 {damage} 데미지 받음");
        Debug.Log($"[TakeDamage] Called during {statusFsm.CurrentStateName}");

        var rightClickSkillId = GetEquippedSkillId(ESkillSlotType.RightClick);
        var rightClickSkill = SkillRepository.Get(rightClickSkillId);

        #region Guard

        bool isGuardSuccess = false;

        if (IsGuarding && rightClickSkill != null && rightClickSkill.effects.Contains("guard"))
        {
            Vector3 guardDir = RotationPoint.forward;
            float angle = Vector3.Angle(attackDirection, guardDir);

            // Debug.Log($"[Guard] Angle: {angle}, Limit: {rightClickSkill.guardAngle * 0.5f}");
            if (angle <= rightClickSkill.guardAngle * 0.5f)
            {
                isGuardSuccess = true;
                float reduce = rightClickSkill.guardReduce;
                float damageReduced = damage * reduce;
                float finalDamage = damage * (1f - reduce);

                // Stamina Reduce
                Stat.SetCurrentStamina(Stat.CurrentStamina - rightClickSkill.guardStaminaCostMin);

                // Debug.Log($"Guard 성공! 원래 데미지: {damage}, 감소된 데미지: {damageReduced}, 받은 데미지: {finalDamage}");

                damage = finalDamage;

                // Guard Break
                if (Stat.CurrentStamina <= 0)
                {
                    IsGuarding = false;
                    ResetMovementModifier();

                    statusFsm.ChangeState(new GuardBreakState(this, statusFsm, rightClickSkill.guardBreakTime));
                    Debug.Log("Guard Break! 플레이어가 기절합니다.");
                    return;
                }
            }
        }

        #endregion

        stat.TakeDamage(damage);

        if (stat.IsDead())
        {
            statusFsm.ChangeState(new DeathState(this, statusFsm));
        }
        else if (isGuardSuccess)
        {
            statusFsm.ChangeState(new GuardHitState(this, statusFsm));
        }
        else
        {
            statusFsm.ChangeState(new HitState(this, statusFsm));
        }

    }
    public float GetAttackPower()
    {
        return stat.Attack;
    }

    public void ApplyStagger(float value)
    {
        Debug.Log($"{gameObject.name}에게 경직 {value} 적용");
    }

    public void ApplyStun(float duration)
    {

    }

    public void ApplyBleed(float amount)
    {

    }

    public void Heal(float amount)
    {
        stat.SetCurrentHP(stat.CurrentHP + amount);
        Debug.Log($"[Heal] {amount} 만큼 회복! 현재 HP: {stat.CurrentHP}/{stat.MaxHP}");
    }

    #endregion

    #region Skill
    public string GetEquippedSkillId(ESkillSlotType slot)
    {
        if (equippedSkills.TryGetValue(slot, out var id))
            return id;
        return null;
    }

    public void UseSkill(string skillId,  ICombatEntity caster, float effectValue)
    {
        // Debug.Log($"[UseSkill] skillId: {skillId}");
        SkillData skill = SkillRepository.Get(skillId);
        if (skill == null)
        {
            // Debug.LogError("[UseSkill] SkillData is null!");
            return;
        }

        if (skill.lightCostValue > 0f && PlayerSightController.Instance != null)
        {
            if (PlayerSightController.Instance.Brightness < skill.lightCostValue)
            {
                // Debug.LogWarning("[UseSkill] 밝기 부족!");
                return;
            }
        }
        // Check Mana
        if (skill.manaCost > 0f)
        {
            if (Stat.CurrentMP < skill.manaCost)
            {
                // Debug.LogWarning("[UseSkill] 마나 부족!");
                return;
            }
        }

        float now = Time.time;
        if (lastSkillTime.TryGetValue(skillId, out float last) && now < last + skill.attackCooldown)
            return;
        lastSkillTime[skillId] = now;

        var context = new SkillExecutionContext
        {
            Caster = caster,
            Skill = skill,
        };

        Vector3 origin = caster.Transform.position;
        Vector3 forward = caster.Transform.forward;
        float range = skill.range;
        float angle = skill.angleOrWidth;

        var targets = BattleManager.Instance.FindEnemiesInArea(origin, range, angle, forward);
        if (targets == null) targets = new List<ICombatEntity>();

        var effects = skill.effects ?? new List<string>();
        // Debug.Log($"[UseSkill] effects: {(effects == null ? "null" : string.Join(",", effects))}");

        if (targets.Count == 0)
        {
            foreach (var effectName in effects)
            {
                // Debug.Log($"[UseSkill] effectName: {effectName}");
                if (string.IsNullOrEmpty(effectName))
                {
                    Debug.LogWarning("[UseSkill] effectName is null or empty!");
                    continue;
                }
                if (!SkillEffectFactory.EffectMap.TryGetValue(effectName, out var effect) || effect == null)
                {
                    Debug.LogError($"[UseSkill] Effect not found or null: {effectName}");
                    continue;
                }
                // Debug.Log($"[UseSkill] effect.Apply({effectName}) 호출!");
                effect.Apply(caster, null, context); // target == null
            }
        }
        else
        {
            foreach (var target in targets)
            {
                context.Target = target;
                foreach (var effectName in effects)
                {
                    if (string.IsNullOrEmpty(effectName))
                    {
                        Debug.LogWarning("[UseSkill] effectName is null or empty!");
                        continue;
                    }
                    if (!SkillEffectFactory.EffectMap.TryGetValue(effectName, out var effect) || effect == null)
                    {
                        Debug.LogError($"[UseSkill] Effect not found or null: {effectName}");
                        continue;
                    }
                    effect.Apply(caster, target, context);
                }
            }
        }
    }

    public float GetLastSkillUseTime(string skillId)
    {
        if (lastSkillTime.TryGetValue(skillId, out float time))
            return time;
        return -99999f; // 사용한 적 없으면 아주 과거값 반환 (항상 쿨타임 아님 처리)
    }

    #region Dash

    public void TryDash()
    {
        if(isDashing) return;
        if (Time.time < lastDashTime + dashCooldown) return;
        if (Stat.CurrentStamina < dashStaminaCost) return;

        Stat.SetCurrentStamina(Stat.CurrentStamina - dashStaminaCost);

        SoundManager.Instance.PlaySFXAtPosition(dashClip, transform.position, 1f, 16f, 0.05f);

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float dashDistance = 2f;
        float dashDuration = 0.1f;
        Vector3 dashDir = (lastNonZeroMoveDirection.sqrMagnitude > 0.01f)
            ? new Vector3(lastNonZeroMoveDirection.x, 0, lastNonZeroMoveDirection.y).normalized
            : RotationPoint.forward;

        Vector3 start = transform.position;
        float elapsed = 0f;

        BlockMove();
        StartCoroutine(SpawnAfterimagesDuringDash(dashDuration, afterimageInterval));
        while (elapsed < dashDuration)
        {
            float t = elapsed / dashDuration;
            Vector3 nextPos = Vector3.Lerp(start, start + dashDir * dashDistance, t);

            CapsuleCollider capsule = GetComponent<CapsuleCollider>();
            float playerHeight = capsule.height;
            float playerRadius = capsule.radius;
            Vector3 capStart = nextPos + Vector3.up * playerRadius;
            Vector3 capEnd = nextPos + Vector3.up * (playerHeight - playerRadius);

            if (Physics.CheckCapsule(capStart, capEnd, playerRadius, recoilBlockMask))
            {
                break;
            }

            PlayerRigidbody.MovePosition(nextPos);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        UnblockMove();
        isDashing = false;
    }

    private IEnumerator SpawnAfterimagesDuringDash(float totalTime, float interval)
    {
        float elapsed = 0f;
        while (elapsed < totalTime)
        {
            CreateAfterimage();
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }
    private void CreateAfterimage()
    {
        if (afterimagePrefab == null || spriteRenderer == null) return;

        GameObject obj = Instantiate(afterimagePrefab, graphicsObject.transform.position, graphicsObject.transform.rotation);
        var ai = obj.GetComponent<Afterimage>();
        if (ai != null)
        {
            // 연보라, 파랑, 흰색, 투명 등 색상은 취향껏
            Color col = spriteRenderer.color * new Color(1f, 1f, 1f, 0.6f); // 반투명
            ai.SetSprite(spriteRenderer.sprite, col, spriteRenderer.flipX);
        }
    }

    #endregion

    #endregion

    #region Interaction (Item, NPC)

    public void HandleImmediateInteraction()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out var hit))
        {
            float dist = Vector3.Distance(hit.point, transform.position);
            if (dist > interactRange)
            {
                return;
            }

            // Immadiate Interaction
            if (hit.collider.TryGetComponent<IImmediateInteractable>(out var imm))
            {
                imm.Interact(this);
                return;
            }
        }
        else
        {
        }
    }

    public void HandleChestOpened()
    {
        // ex) Sound etc..
    }



    #endregion

    #region Direction

    public void UpdateAnimDirection(Vector3 worldDir)
    {
        Vector3 localDir = transform.InverseTransformDirection(worldDir.normalized);
        //bodyAnimatorManager.SetFloat("MoveX", localDir.x);
        //bodyAnimatorManager.SetFloat("MoveY", localDir.z);
    }

	private void HandleAiming(InputData input)
	{
		Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
		Vector2 mousePos = Mouse.current.position.ReadValue();
		bool isFlipped = mousePos.x < screenPos.x;
		spriteRenderer.flipX = isFlipped;

        var mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_FlipX", spriteRenderer.flipX ? 1f : 0f);
        spriteRenderer.SetPropertyBlock(mpb);

        leftWeapon.UpdateWeapon(mousePos, screenPos, !isFlipped, mousePos.y > screenPos.y);
		rightWeapon.UpdateShield(!isFlipped);
		leftWeapon.SetSide(!isFlipped);
		rightWeapon.SetSide(!isFlipped);
    }

    private void UpdateAiming()
    {
        Plane plane = new Plane(Vector3.up, transform.position.y);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (plane.Raycast(ray, out float dist))
        {
            Vector3 hitPoint = ray.GetPoint(dist);
            Vector3 dir = (hitPoint - RotationPoint.position);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                RotationPoint.forward = dir.normalized;
        }
    }

    #region Event
    //public void OnSlashHit()
    //{
    //    actionFsm.ForceHitCurrent();
    //}
    #endregion

    #endregion

    #region VFX
    public void FlashRed(float flashTime = 0.1f)
    {
        if (flashRedCoroutine != null)
            StopCoroutine(flashRedCoroutine);

        flashRedCoroutine = StartCoroutine(FlashRedRoutine(flashTime));
    }

    private IEnumerator FlashRedRoutine(float flashTime)
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        spriteRenderer.color = originalColor;
    }
    #endregion

    #region UI
    public void ShowDamageOverlay()
    {
        damageOverlay?.Flash();
    }

    #endregion

    #region Movement Modify

    // 현재 이동속도 얻는 용(이동, 달리기, 은닉 걷기에 사용)
    public float GetCurrentMoveSpeed(bool isRun, bool isSneak)
	{
		float speed = stat.CurrentMovementSpeed;
		if (isRun) speed *= stat.CurrentRunSpeedMultiplier;
		if (isSneak) speed *= stat.CurrentSneakSpeedMultiplier;
		return speed;
	}

    // 공격 중 이동속도가 감소하거나 할 때 사용
	public void ApplyMovementModifier(float multiplier)
    {
        MovementSpeedModifier = multiplier;
    }
    // 위에꺼 끝나면 다시 돌려주는 초기화하는 용도
    public void ResetMovementModifier()
    {
        MovementSpeedModifier = 1f;
    }

    #endregion

    #region Recoil

    public void DoRecoil(Vector3 recoilDir, float recoilDist)
    {
        StartCoroutine(DoSafeRecoilRoutine(recoilDir, recoilDist));
    }


    private IEnumerator DoSafeRecoilRoutine(Vector3 recoilDir, float recoilDist)
    {
        Vector3 start = transform.position;
        Vector3 dir = new Vector3(recoilDir.x, 0, recoilDir.z).normalized;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        float moveDist = recoilDist;

        Vector3 target = start + dir * moveDist;
        float playerHeight = capsule.height;
        float playerRadius = capsule.radius;


        // 1. 목표 위치에서 OverlapCapsule로 "빈 공간"만 이동
        Vector3 checkStart = target + Vector3.up * playerRadius;
        Vector3 checkEnd = target + Vector3.up * (playerHeight - playerRadius);

        if (Physics.CheckCapsule(checkStart, checkEnd, playerRadius, recoilBlockMask))
        {
            // 이동 불가, 벽/지형/오브젝트에 겹침 → 이동하지 않음!
            yield break;
        }

        // 2. 목표 위치에서 바닥 높이(Raycast) 한 번만 보정
        if (Physics.Raycast(target + Vector3.up * 1.5f, Vector3.down, out RaycastHit ground, 3f, LayerMask.GetMask("Ground")))
        {
            target.y = ground.point.y;
        }
        else
        {
            target.y = start.y;
        }

        // 3. 이동 (짧고 부드럽게, FixedUpdate 프레임에 맞춰 진행)
        BlockMove();
        float duration = 0.07f;
        float t = 0;
        while (t < duration)
        {
            Vector3 pos = Vector3.Lerp(start, target, t / duration);
            PlayerRigidbody.MovePosition(pos);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate(); // FixedUpdate 주기로 변경
        }
        PlayerRigidbody.MovePosition(target);
        UnblockMove();
    }

    #endregion

    #region Heal Coroutine
    public void StartHealRoutine(IEnumerator routine)
    {
        if (healCoroutine != null)
            StopCoroutine(healCoroutine);

        healCoroutine = StartCoroutine(routine);
    }

    public void StopHealRoutine()
    {
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
            healCoroutine = null;
        }
    }

    #endregion

    #region Death

    public void OnDeath(Vector3 deadPosition)
    {
        Transform ears = transform.Find("Ears");
        if (ears != null)
            ears.position = deadPosition; // 또는 시체 머리 위치 등으로 옮김
    }

    public void SetDeadVisuals(bool isDead)
    {
        foreach (var collider in GetComponentsInChildren<Collider>())
            collider.enabled = !isDead;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (isDead)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

    }

    public GameObject SpawnDeadOnDeath()
    {
        string jobId = jobData.JobId;
        GameObject deadPrefab = DeadPrefabRegistry.Get(jobId);
        var deadObj = Instantiate(deadPrefab, transform.position, Quaternion.identity);

        var playerDeadComp = deadObj.GetComponent<PlayerDead>();
        if (playerDeadComp != null)
            playerDeadComp.Init(playerId);

        return deadObj;
    }

    public void OnDeathWithCamera(GameObject deadObj)
    {
        if (!IsLocalPlayer) return;

        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null && deadObj != null)
        {
            vcam.Follow = deadObj.transform;
            vcam.LookAt = deadObj.transform;
        }
    }

    #endregion

    #region Network
    public bool IsLocalPlayer
    {
        get
        {
            // Photon: return photonView.IsMine;
            // Mirror: return isLocalPlayer;
            // Steamworks/커스텀: return myNetworkId == NetworkManager.LocalPlayerId;
            return true; // 싱글이면 항상 true
        }
    }
    #endregion

    #region Item

    public bool UseItem(ItemInstance item)
    {
        if (item == null) return false;
        if (!Enum.TryParse<ItemType>(item.data.type, out var type)) return false;

        if (type == ItemType.Consumable && !string.IsNullOrEmpty(item.data.skillId))
        {
            UseSkill(item.data.skillId, this, item.data.effectValue);
            item.stackCount--;
            if (item.stackCount <= 0)
                Inventory.RemoveItem(item);
            inventoryUI?.UpdateUI();
            return true;
        }

        return false;
    }

    private void ApplyConsumableEffect(ItemInstance item)
    {
        if (Enum.TryParse<ConsumableType>(item.data.consumableType, out var ctype))
        {
            float value = item.data.effectValue;
            switch (ctype)
            {
                case ConsumableType.Health:
                    Stat.SetCurrentHP(Stat.CurrentHP + value);
                    break;
                case ConsumableType.Mana:
                    Stat.SetCurrentMP(Stat.CurrentMP + value);
                    break;
                case ConsumableType.Stamina:
                    Stat.SetCurrentStamina(Stat.CurrentStamina + value);
                    break;
                // Torch / Light
                default:
                    // Change to Enum later
                    if (item.data.consumableType == "Torch" || item.data.consumableType == "Light")
                    {
                        var sight = GetComponent<PlayerSightController>();
                        if (sight != null)
                        {
                            sight.AddBrightness(value);
                            // TODO: Effect and Sound
                        }
                    }
                    break;
            }
        }
        else if (item.data.consumableType == "Torch" || item.data.consumableType == "Light")
        {
            var sight = GetComponent<PlayerSightController>();
            if (sight != null)
            {
                sight.AddBrightness(item.data.effectValue);
                // TODO: Effect and Sound
            }
        }
    }

    #region Sight

    void UseBrightnessItem(float amount)
    {
        var sight = GetComponent<PlayerSightController>();
        if (sight != null)
            sight.AddBrightness(amount);
    }

    #endregion

    #endregion
}