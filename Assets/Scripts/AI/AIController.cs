// AIController.cs
// AI Controller với FSM: Patrol → Chase → Catch
// Role A - Sprint 1 (Skeleton) / Sprint 2 (Hoàn chỉnh)

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private AIState currentState = AIState.Patrol;

    [Header("Target")]
    public Transform playerTarget;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    [SerializeField] private float waypointReachDistance = 1.5f;
    public float patrolSpeed = 0.25f; // Giảm xuống 0.25f (25% tốc độ gốc)
    private int currentWaypointIndex = 0;

    [Header("Detection Settings")]
    public float detectionRadius = 12f;
    [SerializeField] private float fieldOfView = 120f;

    [Header("Chase Settings")]
    public float chaseSpeed = 0.5625f; // Giảm xuống 0.5625f (25% tốc độ gốc)
    public float attackRadius = 2f;

    [Header("Door Interaction Settings")]
    [Tooltip("Cho phép quái vật tự động mở cửa để lùng bắt player")]
    [SerializeField] private bool canOpenDoors = true;
    [Tooltip("Khoảng cách quái vật phát hiện và mở cửa (mét)")]
    [SerializeField] private float doorCheckDistance = 2.5f;
    [Tooltip("Góc hình nón trước mặt quái vật để mở cửa (độ)")]
    [SerializeField] private float doorDetectionAngle = 85f;
    [Tooltip("Khoảng thời gian (giây) giữa các lần kiểm tra cửa")]
    [SerializeField] private float doorCheckInterval = 0.15f;

    private NavMeshAgent agent;
    private Transform player;
    private bool catchTriggered;
    private float doorCheckTimer = 0f;
    private DoorInteractive[] cachedDoors;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            // Tối ưu thể hình NavMeshAgent (radius 0.35m, height 1.8m) để chui lọt qua mọi khung cửa biệt thự
            agent.radius = 0.35f;
            agent.height = 1.8f;
            agent.stoppingDistance = 0.4f; // Không dừng sớm ở ngưỡng cửa
            agent.autoTraverseOffMeshLink = true; // Tự động đi qua OffMeshLink liên phòng
        }

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
        {
            col.radius = 0.35f;
            col.height = 1.8f;
            col.center = new Vector3(0, 0.9f, 0);
        }

        if (playerTarget != null)
            player = playerTarget;
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("AIController: Không tìm thấy GameObject với tag 'Player'!");
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
            Debug.LogWarning("AIController: Chưa có waypoints. AI sẽ đứng yên cho đến khi phát hiện player.");

        RefreshDoorsCache();
    }

    public void RefreshDoorsCache()
    {
        cachedDoors = Object.FindObjectsByType<DoorInteractive>(FindObjectsSortMode.None);
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                CheckPlayerDetection();
                CheckAndOpenDoors();
                break;
            case AIState.Chase:
                ChaseBehavior();
                CheckCatchPlayer();
                CheckAndOpenDoors();
                break;
            case AIState.Catch:
                CatchBehavior();
                break;
        }
    }

    bool AgentReady()
    {
        return agent != null && agent.enabled && agent.isOnNavMesh;
    }

    void PatrolBehavior()
    {
        Vector3 targetPos = Vector3.zero;
        bool hasTarget = false;

        if (patrolPoints != null && patrolPoints.Length > 0 && patrolPoints[currentWaypointIndex] != null)
        {
            targetPos = patrolPoints[currentWaypointIndex].position;
            hasTarget = true;
        }

        if (!hasTarget)
        {
            return;
        }

        MoveTowards(targetPos, patrolSpeed);

        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPos.x, 0, targetPos.z)) < waypointReachDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Length;
        }
    }

    void CheckPlayerDetection()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRadius)
        {
            // Trong cự ly gần hoặc nhìn thấy trong tầm mắt
            if (distanceToPlayer <= 5f)
            {
                currentState = AIState.Chase;
                return;
            }

            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= fieldOfView / 2)
            {
                currentState = AIState.Chase;
            }
        }
    }

    void ChaseBehavior()
    {
        if (player == null)
        {
            currentState = AIState.Patrol;
            return;
        }

        MoveTowards(player.position, chaseSpeed);
    }

    void MoveTowards(Vector3 targetPos, float speed)
    {
        Vector3 myPos = transform.position;
        Vector3 toTarget = targetPos - myPos;
        toTarget.y = 0;
        float distToTarget = toTarget.magnitude;

        // Nếu đã ở cự ly bắt sát người chơi thì dừng để thực hiện jumpscare
        if (distToTarget <= attackRadius * 0.7f)
        {
            if (AgentReady())
                agent.isStopped = true;
            return;
        }

        if (AgentReady())
        {
            agent.isStopped = false;
            agent.speed = speed;
            agent.SetDestination(targetPos);

            // Kiểm tra xem quái vật có bị kẹt ở ngưỡng cửa hoặc ranh giới NavMesh không:
            // 1. PathPartial hoặc PathInvalid (NavMesh không thông suốt giữa 2 phòng)
            // 2. Không có đường đi (!hasPath) dù player ở xa (> attackRadius)
            // 3. Đã đến cuối đoạn đường NavMesh (remainingDistance <= stoppingDistance + 0.3f) nhưng vẫn còn xa player
            bool isBlockedOrPartial = agent.pathPending == false &&
                (agent.pathStatus == NavMeshPathStatus.PathPartial ||
                 agent.pathStatus == NavMeshPathStatus.PathInvalid ||
                 (!agent.hasPath && distToTarget > attackRadius) ||
                 (agent.hasPath && agent.remainingDistance <= agent.stoppingDistance + 0.3f && distToTarget > attackRadius));

            if (isBlockedOrPartial)
            {
                // Vượt qua ngưỡng cửa bước thẳng vào trong phòng
                TraverseThroughObstacleOrDoor(targetPos, speed);
                return;
            }
        }
        else
        {
            // Fallback nếu NavMeshAgent tạm thời chưa sẵn sàng
            DirectMoveTowards(targetPos, speed);
        }
    }

    private void DirectMoveTowards(Vector3 targetPos, float speed)
    {
        Vector3 direction = (targetPos - transform.position);
        direction.y = 0;
        if (direction.sqrMagnitude > 0.05f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 6f);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Xử lý quái vật vượt qua khe cửa / ranh giới NavMesh để tiến thẳng vào trong phòng bắt người chơi
    /// </summary>
    private void TraverseThroughObstacleOrDoor(Vector3 targetPos, float speed)
    {
        Vector3 myPos = transform.position;
        Vector3 dir = (targetPos - myPos);
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;
        dir.Normalize();

        // Luôn xoay mặt nhìn thẳng về phía player trong phòng
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);

        Vector3 step = dir * (speed * Time.deltaTime);
        Vector3 nextTargetPos = myPos + step;

        // Thử tìm vị trí NavMesh phía trước trong phòng
        if (NavMesh.SamplePosition(nextTargetPos + dir * 0.6f, out NavMeshHit forwardHit, 2.5f, NavMesh.AllAreas))
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.Warp(forwardHit.position);
                agent.SetDestination(targetPos);
            }
            else
            {
                transform.position = forwardHit.position;
                if (agent != null)
                {
                    agent.enabled = true;
                    agent.Warp(forwardHit.position);
                    agent.SetDestination(targetPos);
                }
            }
        }
        else
        {
            // Nếu vùng khe cửa không có NavMesh -> tạm thời di chuyển trực tiếp qua khung cửa
            if (agent != null && agent.enabled)
            {
                agent.enabled = false;
            }
            transform.position += step;

            // Kiểm tra xem đã bước tới vùng có NavMesh trên sàn phòng mới chưa
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit roomHit, 1.2f, NavMesh.AllAreas))
            {
                transform.position = roomHit.position;
                if (agent != null)
                {
                    agent.enabled = true;
                    agent.Warp(roomHit.position);
                    agent.SetDestination(targetPos);
                }
            }
        }
    }

    void CheckCatchPlayer()
    {
        if (player == null)
            return;

        if (Vector3.Distance(transform.position, player.position) < attackRadius)
        {
            Debug.Log("[AI CATCH] Bắt được Player! Chuyển sang Catch mode.");
            currentState = AIState.Catch;
        }
    }

    void OnEnable()
    {
        GameEvents.OnPlayerCaught += HandleGameEnd;
        GameEvents.OnGameWin += HandleGameEnd;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerCaught -= HandleGameEnd;
        GameEvents.OnGameWin -= HandleGameEnd;
    }

    void HandleGameEnd()
    {
        if (AgentReady())
            agent.isStopped = true;
        enabled = false;
    }

    void CatchBehavior()
    {
        if (AgentReady())
            agent.isStopped = true;

        if (catchTriggered)
            return;

        catchTriggered = true;
        GameEvents.TriggerPlayerCaught();

        // Fallback reload only if no MirrorHouseGameManager is present
        if (FindAnyObjectByType<MirrorHouseGameManager>() == null)
        {
            Invoke(nameof(ReloadScene), 2.5f);
        }
    }

    void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    /// <summary>
    /// Kiểm tra các cánh cửa xung quanh và tự động mở cửa nếu cửa đang đóng cản đường.
    /// Cho phép quái vật đuổi bắt người chơi xuyên phòng mượt mà.
    /// </summary>
    private void CheckAndOpenDoors()
    {
        if (!canOpenDoors) return;

        doorCheckTimer += Time.deltaTime;
        if (doorCheckTimer < doorCheckInterval) return;
        doorCheckTimer = 0f;

        if (cachedDoors == null || cachedDoors.Length == 0)
        {
            RefreshDoorsCache();
            if (cachedDoors == null || cachedDoors.Length == 0) return;
        }

        Vector3 monsterPos = transform.position;
        Vector3 monsterFwd = transform.forward;
        monsterFwd.y = 0;
        if (monsterFwd.sqrMagnitude > 0.001f)
            monsterFwd.Normalize();

        bool isChasing = (currentState == AIState.Chase);

        for (int i = 0; i < cachedDoors.Length; i++)
        {
            DoorInteractive door = cachedDoors[i];
            if (door == null || door.IsOpen || door.IsExitDoor)
                continue;

            Vector3 doorPos = door.transform.position;
            Vector3 toDoor = doorPos - monsterPos;
            toDoor.y = 0;
            float dist = toDoor.magnitude;

            // Nếu doorHinge tồn tại, lấy khoảng cách nhỏ nhất giữa root và hinge để đảm bảo độ chính xác
            if (door.DoorHinge != null)
            {
                Vector3 toHinge = door.DoorHinge.position - monsterPos;
                toHinge.y = 0;
                float distHinge = toHinge.magnitude;
                if (distHinge < dist)
                {
                    dist = distHinge;
                    toDoor = toHinge;
                }
            }

            if (dist > doorCheckDistance)
                continue;

            float angle = Vector3.Angle(monsterFwd, toDoor);

            // Mở cửa nếu cửa ở phía trước quái vật (<= doorDetectionAngle) HOẶC quái vật đã đứng rất sát cửa (<= 1.8m)
            if (angle <= doorDetectionAngle || dist <= 1.8f)
            {
                Collider monsterCol = GetComponent<Collider>();
                if (door.TryOpenByMonster(isChasing, monsterCol))
                {
                    Debug.Log($"<color=red>[AI MONSTER] Quái vật đã mở cánh cửa '{door.gameObject.name}' (State: {currentState})!</color>");

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayLockOpen();
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        if (canOpenDoors)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, doorCheckDistance);
        }

        Gizmos.color = Color.blue;
        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2, transform.up) * transform.forward * detectionRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2, transform.up) * transform.forward * detectionRadius;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
    }
}
