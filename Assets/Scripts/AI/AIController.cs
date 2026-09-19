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
    [SerializeField] private float waypointReachDistance = 1f;
    private int currentWaypointIndex = 0;
    
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
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
        // TODO Sprint 2: Implement patrol logic
        // - Di chuyển đến waypoint hiện tại
        // - Khi đến gần, chuyển sang waypoint tiếp theo
        
        if (patrolWaypoints == null || patrolWaypoints.Length == 0)
            return;
        
        // Placeholder: AI đứng yên
        agent.isStopped = true;
    }
    
    void CheckPlayerDetection()
    {
        // TODO Sprint 2: Implement detection logic
        // - Kiểm tra khoảng cách đến player
        // - Kiểm tra góc nhìn (field of view)
        // - Kiểm tra raycast (không bị tường chặn)
        // - Nếu phát hiện → chuyển sang Chase state
    }
    
    // ============ CHASE STATE ============
    void ChaseBehavior()
    {
        // TODO Sprint 2: Implement chase logic
        // - Đuổi theo player
        // - Cập nhật destination liên tục
        
        if (player == null)
            return;
        
        // Placeholder
        agent.isStopped = true;
    }
    
    void CheckCatchPlayer()
    {
        // TODO Sprint 2: Implement catch check
        // - Kiểm tra khoảng cách đến player
        // - Nếu đủ gần → chuyển sang Catch state
    }
    
    // ============ CATCH STATE ============
    void CatchBehavior()
    {
        // TODO Sprint 2: Implement catch behavior
        // - Dừng AI
        // - Trigger game over event
        // - Reload scene hoặc hiện UI thua
        
        agent.isStopped = true;
        Debug.Log("AI bắt được player! Game Over!");
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
