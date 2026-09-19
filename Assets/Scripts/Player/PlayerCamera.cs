// PlayerCamera.cs
// Điều khiển camera FPS cho Android Mobile
// Sử dụng touch/swipe (tay phải) thay vì mouse
// Role A - Sprint 1

using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float touchSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;
    
    [Header("References")]
    [Tooltip("Kéo Player GameObject (cha của camera) vào đây")]
    [SerializeField] private Transform playerBody;
    
    [Header("Mobile Touch Area - QUAN TRỌNG")]
    [Tooltip("Vùng UI cho touch camera (Role D sẽ tạo sau). Để trống tạm thời.")]
    [SerializeField] private RectTransform touchArea;
    
    private float verticalRotation = 0f;
    private Vector2 lastTouchPosition;
    private bool isTouching = false;
    
    void Start()
    {
        // Cảnh báo nếu chưa gắn playerBody
        if (playerBody == null)
        {
            Debug.LogError("PlayerCamera: Chưa gắn Player Body reference!");
        }
        
        if (touchArea == null)
        {
            Debug.LogWarning("PlayerCamera: Chưa có Touch Area UI. Sẽ dùng toàn màn hình tạm thời.");
            Debug.LogWarning("→ Role D sẽ tạo Touch Area UI (right side) sau.");
        }
    }
    
    void Update()
    {
        HandleTouchInput();
    }
    
    void HandleTouchInput()
    {
        // Kiểm tra touch input trên mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            // TODO: Kiểm tra touch có nằm trong touchArea không (khi Role D tạo UI)
            // Tạm thời accept tất cả touch
            
            if (touch.phase == TouchPhase.Began)
            {
                // Bắt đầu touch
                lastTouchPosition = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                // Tính delta movement
                Vector2 delta = touch.position - lastTouchPosition;
                lastTouchPosition = touch.position;
                
                // Rotate camera
                RotateCamera(delta);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
        
        // FALLBACK: Dùng mouse cho testing trong Unity Editor
        #if UNITY_EDITOR
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) // Left hoặc Right mouse button
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            RotateCamera(mouseDelta * 10f); // Scale up vì mouse delta nhỏ hơn touch
        }
        #endif
    }
    
    void RotateCamera(Vector2 delta)
    {
        if (playerBody == null)
            return;
        
        // Horizontal rotation (xoay player body)
        float horizontalRotation = delta.x * touchSensitivity;
        playerBody.Rotate(Vector3.up * horizontalRotation);
        
        // Vertical rotation (xoay camera lên/xuống)
        verticalRotation -= delta.y * touchSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
        
        // Áp dụng rotation cho camera
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
