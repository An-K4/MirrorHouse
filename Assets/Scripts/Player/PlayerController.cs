// PlayerController.cs
// Điều khiển di chuyển nhân vật cho Android Mobile
// Sử dụng Virtual Joystick (tay trái) thay vì WASD
// Role A - Sprint 1

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Sprint Settings")]
    [SerializeField] private float sprintMultiplier = 1.5f;
    
    [Header("Mobile Input - QUAN TRỌNG")]
    [Tooltip("Kéo Virtual Joystick component vào đây (Role D sẽ tạo UI này)")]
    // TODO: Uncomment sau khi import Joystick Pack từ Asset Store
    // [SerializeField] private VariableJoystick joystick;
    
    // Components
    private CharacterController controller;
    private Vector3 velocity;
    private bool isSprinting = false;
    
    void Start()
    {
        // Lấy CharacterController component
        controller = GetComponent<CharacterController>();
        
        if (controller == null)
        {
            Debug.LogError("PlayerController: Thiếu CharacterController component!");
        }
        
        // Cảnh báo nếu chưa gắn joystick
        // TODO: Uncomment sau khi có Joystick
        // if (joystick == null)
        // {
        //     Debug.LogWarning("PlayerController: Chưa gắn Virtual Joystick! Player sẽ không di chuyển được.");
        //     Debug.LogWarning("→ Role D sẽ tạo UI Joystick và gắn vào đây sau.");
        // }
        Debug.LogWarning("PlayerController: Joystick chưa được kích hoạt. Import Joystick Pack để sử dụng!");
    }
    
    void Update()
    {
        MovePlayer();
        ApplyGravity();
    }
    
    void MovePlayer()
    {
        // TODO: Sẽ dùng joystick sau khi import Joystick Pack
        // Tạm thời dùng keyboard cho testing trong Unity Editor
        if (controller == null)
            return;
        
        // FALLBACK: Dùng WASD tạm thời trong Unity Editor
        float horizontal = Input.GetAxis("Horizontal");  // A/D hoặc Left/Right arrow
        float vertical = Input.GetAxis("Vertical");      // W/S hoặc Up/Down arrow
        
        // Tính hướng di chuyển (relative to camera)
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        
        // Áp dụng tốc độ
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        
        // Di chuyển
        controller.Move(direction * currentSpeed * Time.deltaTime);
    }
    
    void ApplyGravity()
    {
        if (controller == null)
            return;
        
        // Áp dụng trọng lực
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Giữ player sát mặt đất
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    // Public method cho UI Button Sprint (Role D sẽ gọi)
    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
        
        // Event cho footstep sound (Role D sẽ subscribe)
        // TODO: GameEvents.OnPlayerSprintChanged?.Invoke(sprinting);
    }
    
    // Getter cho các script khác
    public bool IsSprinting() => isSprinting;
    public bool IsMoving() => controller != null && controller.velocity.magnitude > 0.1f;
}
