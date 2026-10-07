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

    private NavMeshAgent agent;
    private Transform player;
    private bool catchTriggered;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                CheckPlayerDetection();
                break;
            case AIState.Chase:
                ChaseBehavior();
                CheckCatchPlayer();
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
        if (AgentReady())
        {
            agent.isStopped = false;
            agent.speed = speed;
            agent.SetDestination(targetPos);
        }
        else
        {
            // Fallback nếu chưa bake NavMesh
            Vector3 direction = (targetPos - transform.position);
            direction.y = 0;
            if (direction.sqrMagnitude > 0.05f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                transform.position += transform.forward * speed * Time.deltaTime;
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

    void CatchBehavior()
    {
        if (AgentReady())
            agent.isStopped = true;

        if (catchTriggered)
            return;

        catchTriggered = true;
        GameEvents.TriggerPlayerCaught();
        Invoke(nameof(ReloadScene), 2f);
    }

    void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.blue;
        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2, transform.up) * transform.forward * detectionRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2, transform.up) * transform.forward * detectionRadius;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
    }
}
