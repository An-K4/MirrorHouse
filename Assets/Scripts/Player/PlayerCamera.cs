// PlayerCamera.cs
// Điều khiển camera FPS cho Android Mobile
// Sử dụng touch/swipe (tay phải) thay vì mouse
// Role A - Sprint 1

using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float touchSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle = 80f;

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
        if (playerBody == null && transform.parent != null)
            playerBody = transform.parent;

        if (playerBody == null)
            Debug.LogError("PlayerCamera: Chưa gắn Player Body reference!");

        if (touchArea == null)
            Debug.LogWarning("PlayerCamera: Chưa có Touch Area UI. Dùng touch toàn màn hình / chuột trái kéo trong Editor.");
    }

    void Update()
    {
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            // QUAN TRỌNG: Chỉ accept touch bên PHẢI màn hình
            // Bên TRÁI dành cho joystick (di chuyển)
            // Bên PHẢI dành cho camera (xoay nhìn)
            float screenMiddle = Screen.width / 2f;
            
            // Ignore touch bên trái (joystick area)
            if (touch.position.x < screenMiddle)
            {
                isTouching = false;
                return;
            }
            
            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                Vector2 delta = touch.position - lastTouchPosition;
                lastTouchPosition = touch.position;
                RotateCamera(delta);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
        
        // FALLBACK: Dùng mouse cho testing trong Unity Editor
        #if UNITY_EDITOR
        // Click-trái và kéo để xoay camera (không cần cursor lock)
        if (Input.GetMouseButton(0)) // Giữ chuột trái
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            if (mouseDelta.sqrMagnitude > 0.001f)
            {
                RotateCamera(mouseDelta * 5f); // Giảm sensitivity từ 15f xuống 5f
            }
        }
        #endif
    }

    void RotateCamera(Vector2 delta)
    {
        if (playerBody == null)
            return;

        playerBody.Rotate(Vector3.up * (delta.x * touchSensitivity));

        verticalRotation -= delta.y * touchSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
