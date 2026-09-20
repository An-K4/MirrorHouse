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

    [Header("Patrol Settings")]
    [Tooltip("Danh sách waypoints AI sẽ tuần tra (Role C sẽ tạo)")]
    [SerializeField] private Transform[] patrolWaypoints;
    [SerializeField] private float waypointReachDistance = 1.5f;  // Phải lớn hơn actual distance AI stops (1.08)
    private int currentWaypointIndex = 0;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float fieldOfView = 120f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float catchRange = 2f;

    // Components
    private NavMeshAgent agent;
    private Transform player;

    void Start()
    {
        // Lấy components
        agent = GetComponent<NavMeshAgent>();

        // Tìm player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("AIController: Không tìm thấy GameObject với tag 'Player'!");
        }

        // Kiểm tra waypoints
        if (patrolWaypoints == null || patrolWaypoints.Length == 0)
        {
            Debug.LogWarning("AIController: Chưa có waypoints! AI sẽ đứng yên.");
            Debug.LogWarning("→ Role C sẽ tạo waypoints trong scene sau.");
        }
    }

    void Update()
    {
        // FSM - State machine
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

    // ============ PATROL STATE ============
    void PatrolBehavior()
    {
        if (patrolWaypoints == null || patrolWaypoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        // Lấy waypoint hiện tại
        Transform targetWaypoint = patrolWaypoints[currentWaypointIndex];

        // Di chuyển đến waypoint
        agent.isStopped = false;
        agent.speed = 3.5f;
        agent.SetDestination(targetWaypoint.position);

        // Visualize patrol path trong Scene view
        Debug.DrawLine(transform.position, targetWaypoint.position, Color.blue);

        // Kiểm tra đã đến waypoint chưa
        float checkDistance = Vector3.Distance(transform.position, targetWaypoint.position);
        if (checkDistance < waypointReachDistance)
        {
            // Chuyển sang waypoint tiếp theo (loop)
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }

    void CheckPlayerDetection()
    {
        if (player == null)
            return;

        // Check distance
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange)
            return;

        // Check FOV angle
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfView / 2)
            return;

        // Granny-style: Detect player khi trong range + FOV (không cần line of sight)
        currentState = AIState.Chase;
    }

    // ============ CHASE STATE ============
    void ChaseBehavior()
    {
        if (player == null)
        {
            currentState = AIState.Patrol;
            return;
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        Debug.DrawLine(transform.position, player.position, Color.red);
    }

    void CheckCatchPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < catchRange)
        {
            Debug.Log("🔴 [AI CATCH] Bắt được Player! Chuyển sang Catch mode.");
            currentState = AIState.Catch;
        }
    }

    // ============ CATCH STATE ============
    void CatchBehavior()
    {
        agent.isStopped = true;
        GameEvents.TriggerPlayerCaught();
        
        // Reload scene sau 2 giây
        Invoke("ReloadScene", 2f);
    }

    void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    // ============ DEBUG VISUALIZATION ============
    void OnDrawGizmosSelected()
    {
        // Vẽ detection range trong Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Vẽ catch range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchRange);

        // Vẽ field of view
        Gizmos.color = Color.blue;
        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2, transform.up) * transform.forward * detectionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2, transform.up) * transform.forward * detectionRange;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
    }
}
