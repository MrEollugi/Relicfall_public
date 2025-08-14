using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, ICombatEntity
{
    #region Enemy Data
    [SerializeField] private EnemyDataSO enemyData;
    public EnemyDataSO EnemyData => enemyData;
    public float AttackRange => enemyData.attackRange;
    public float MaxChaseRange => enemyData.maxChaseRange;
    #endregion

    private Vector3 lookDir = Vector3.left;//몬스터가 바라보는 방향

    #region Stat
    [Header("Enemy Stat")]
    [SerializeField] private EnemyStat stat;
    public IStat Stat => stat;
    public bool IsAlive => stat.CurrentHP > 0;
    #endregion

    #region Ref
    public Transform Transform => transform;
    #endregion

    #region Behavior
    [Header("Behaviors")]
    [Tooltip("Sort in order of priority")]
    [SerializeField] List<EnemyBehaviorSO> behaviorSOList;

    List<IEnemyBehavior> behaviors = new List<IEnemyBehavior>();
    public NavMeshAgent Agent { get; private set; }

    [Header("AI Type")]
    [SerializeField] private bool isStalker;
    public bool IsStalker => isStalker;

    [Header("AI Parameters")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float maxChaseRange = 20f;

    [Header("Perception")]
    private bool heardLoudSound = false;
    public bool HeardLoudSound => heardLoudSound;
    public Transform Player { get; private set; }

    [Header("Enemy Sound Emitter")]
    [SerializeField] private SoundEmitter soundEmitter;
    public SoundEmitter SoundEmitter => soundEmitter;
    #endregion

    #region Visual

    public Animator Animator { get; private set; }
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Coroutine hitFlashCoroutine;
    private Color originalColor;
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private GameObject deathParticlePrefab;

    #endregion

    #region Snatch

    private ItemData _stolenItem;
    [Header("Drop Prefabs")]
    [SerializeField] private GameObject stolenItemPickupPrefab;

    #endregion

    #region Init

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponentInChildren<Animator>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            Player = playerObj.transform;
        else
            Player = null;

        stat?.Initialize(enemyData);

        foreach(var so in behaviorSOList)
        {
            var beh = so as IEnemyBehavior;
            beh.Init(this);
            behaviors.Add(beh);
        }
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Start()
    {
        if (Player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                Player = playerObj.transform;
        }
    }

    #endregion

    #region Unity Update

    private void Update()
    {
        if (Player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                Player = playerObj.transform;
        }

        if (TryGetComponent<Blackboard>(out var bb))
            bb.IsPlayerVisible = CanSeePlayer();

        foreach (var b in behaviors)
        {
            b.Tick();
        }


    }

    private void LateUpdate()//몬스터가 바라보는 방향으로 sprite 플립
    {
        UpdateLookDir();
        ApplySpriteFlip();
    }

    #endregion

    public void OnSnatchPlayer(Transform player)
    {
        // 1) 플레이어 인벤토리에서 아이템 하나 뽑기 (Inventory API에 맞게 수정)
        //_stolenItem = player.Inventory.TakeRandomItem();
        // 2) 애니메이션 트리거(중복 방지용으로 필요 시)
        Animator.SetTrigger("Snatch");
    }

    public void OnFleeComplete()
    {

    }

    #region ICombatEntity

    public void TakeDamage(float damage, Vector3 attackDirection = default)
    {
        // Debug.Log($"{gameObject.name}가 {damage} 데미지 받음");
        stat.TakeDamage(damage);
        InGameUIManager.Instance.ShowDamageText(transform.position, damage);

        PlayHitEffect();

        if (TryGetComponent<Blackboard>(out var blackboard))
            blackboard.LastHitTime = Time.time;

        if (!IsAlive)
        {
            OnDeath();
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
    public void Heal(float amount)
    {

    }

    public void ApplyStun(float duration)
    {

    }

    public void ApplyBleed(float amount)
    {

    }

    #endregion


    public void OnDeath()
    {
        Animator?.SetTrigger("Death");

        if (deathParticlePrefab != null)
            Instantiate(deathParticlePrefab, transform.position + Vector3.up * 0.3f, Quaternion.identity);

        // Enmey Death 이벤트
        if (enemyData.type == EnemyType.Boss)
            EnemyEventChannel.RaiseBossKilled(enemyData.enemyName);
        else
            EnemyEventChannel.RaiseEnemyKilled(enemyData.enemyName);

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        Agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 3f);
    }

    public void PlayHitEffect()
    {
        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }
        hitFlashCoroutine = StartCoroutine(HitFlashRoutine());

        if (SoundManager.Instance != null && hitSound != null)
        {
            var hitSource = SoundManager.Instance.hitSfxPool.Get(false);
            hitSource.Play(hitSound);
        }
    }

    // 색상 플래시용 코루틴
    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = originalColor;
        hitFlashCoroutine = null;
    }

    public bool CanSeePlayer()
    {
        if (Player == null) return false;
        var playerPos = Player.position;
        var myPos = transform.position;
        //Vector3 toPlayer = playerPos - myPos;
        //float distance = toPlayer.magnitude;

        Vector3 toPlayer = (playerPos - myPos).normalized;
        float distance = Vector3.Distance(myPos, playerPos);
        float sightRange = enemyData.baseSightRange;      // Sight range
        float sightAngle = 120f; // or enemyData.baseSightAngle

        if (distance > sightRange)
            return false;

        //Vector3 forward = transform.forward;
        Vector3 forward = Agent.velocity.normalized;
        if (forward == Vector3.zero)
            forward = transform.forward;


        float angleToPlayer = Vector3.Angle(forward, toPlayer);

        if (angleToPlayer > sightAngle * 0.5f)
            return false;

        // obstacle check
        if (Physics.Raycast(myPos + Vector3.up * 1.0f, toPlayer.normalized, out var hit, distance, LayerMask.GetMask("Obstacle", "Default")))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                return false;
        }

        return true;
    }
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (enemyData == null) return;

        float sightRange = enemyData.baseSightRange;
        float sightAngle = 120f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 forward = Application.isPlaying ? Agent.velocity.normalized : transform.forward;

        if (forward == Vector3.zero)
            forward = transform.forward;

        Quaternion leftRot = Quaternion.Euler(0, -sightAngle * 0.5f, 0);
        Quaternion rightRot = Quaternion.Euler(0, sightAngle * 0.5f, 0);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 1.0f, leftDir * sightRange);
        Gizmos.DrawRay(transform.position + Vector3.up * 1.0f, rightDir * sightRange);
#endif
    }
    private Vector3 Front()
    {
        Vector3 vector3 = new Vector3(0,0,0);



        return vector3;
    }

    private void UpdateLookDir()
    {
        Vector3 dir = Vector3.zero;

        
        if (Agent && Agent.enabled && Agent.velocity.sqrMagnitude > 0.0001f)
            dir = Agent.velocity;

        else if (Player && TryGetComponent<Blackboard>(out var bb) && bb.IsPlayerVisible)
            dir = (Player.position - transform.position);

        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            dir.Normalize();
            lookDir = Vector3.Slerp(lookDir, dir, Time.deltaTime);
        }
    }
    private void ApplySpriteFlip()
    {
        if (!spriteRenderer) return;
        // 아트 기준이 오른쪽을 바라보는 게 기본이면 아래 식이 맞음
        spriteRenderer.flipX = lookDir.x > 0f;
    }
}
