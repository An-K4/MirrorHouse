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
    private int cameraFingerId = -1;

    void Start()
    {
        if (playerBody == null && transform.parent != null)
            playerBody = transform.parent;

        if (playerBody == null)
            Debug.LogError("PlayerCamera: Chưa gắn Player Body reference!");

        if (touchArea == null)
            Debug.LogWarning("PlayerCamera: Chưa có Touch Area UI. Dùng touch toàn màn hình / chuột trái kéo trong Editor.");

        // Load saved sensitivity with safe default
        touchSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", 1.2f);
        if (touchSensitivity > 3.5f)
        {
            touchSensitivity = 1.2f;
            PlayerPrefs.SetFloat("CameraSensitivity", 1.2f);
            PlayerPrefs.Save();
        }
    }

    void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        // Don't rotate if interacting with UI
        bool isPointerOverUI = UnityEngine.EventSystems.EventSystem.current != null &&
                               UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        float screenMiddle = Screen.width / 2f;
        bool foundCameraTouch = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Bỏ qua nếu ngón tay này chạm vào UI (ví dụ bấm nút E, bấm Pause, bấm Sprint)
            bool touchOverUI = UnityEngine.EventSystems.EventSystem.current != null &&
                               UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId);

            if (touchOverUI)
            {
                if (touch.fingerId == cameraFingerId)
                {
                    cameraFingerId = -1;
                    isTouching = false;
                }
                continue;
            }

            // Nếu ngón tay đang là camera finger đã lock
            if (cameraFingerId != -1 && touch.fingerId == cameraFingerId)
            {
                foundCameraTouch = true;
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 pixelDelta = touch.position - lastTouchPosition;
                    lastTouchPosition = touch.position;
                    Vector2 rotDegrees = pixelDelta * (touchSensitivity * 0.065f);
                    RotateCamera(rotDegrees);
                }
                else if (touch.phase == TouchPhase.Stationary)
                {
                    lastTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    cameraFingerId = -1;
                    isTouching = false;
                }
                break;
            }
            // Nếu chưa có camera finger và chạm vào nửa PHẢI màn hình
            else if (cameraFingerId == -1 && touch.position.x >= screenMiddle)
            {
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    cameraFingerId = touch.fingerId;
                    lastTouchPosition = touch.position;
                    isTouching = true;
                    foundCameraTouch = true;
                    break;
                }
            }
        }

        if (!foundCameraTouch && cameraFingerId != -1)
        {
            cameraFingerId = -1;
            isTouching = false;
        }

        // FALLBACK: Dùng mouse cho testing trong Unity Editor (hỗ trợ cả chuột trái và chuột phải)
        #if UNITY_EDITOR
        if (!isPointerOverUI && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            if (mouseDelta.sqrMagnitude > 0.0001f)
            {
                // Chuẩn hóa chuột trên PC: tốc độ mượt mà, không bị giật hay xoay quá nhanh
                Vector2 rotDegrees = mouseDelta * (touchSensitivity * 1.4f);
                RotateCamera(rotDegrees);
            }
        }
        #endif
    }

    void RotateCamera(Vector2 deltaDegrees)
    {
        if (playerBody == null)
            return;

        playerBody.Rotate(Vector3.up * deltaDegrees.x);

        verticalRotation -= deltaDegrees.y;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    public void SetSensitivity(float newSensitivity)
    {
        touchSensitivity = Mathf.Clamp(newSensitivity, 0.2f, 3.5f);
        PlayerPrefs.SetFloat("CameraSensitivity", touchSensitivity);
        PlayerPrefs.Save();
    }

    public float GetSensitivity() => touchSensitivity;
}
