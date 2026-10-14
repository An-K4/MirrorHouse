using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// DOOR INTERACTION SYSTEM
/// Cửa 4 bức tường kết nối các phòng. Click chuột hoặc nhấn E để mở/đóng cửa.
/// Có hỗ trợ khóa bằng chìa khóa (KeyId). Hỗ trợ quái vật (Mirror_Demon_Monster) mở cửa đuổi bắt player.
/// </summary>
public class DoorInteractive : Interactable
{
    [Header("Door Settings")]
    [Tooltip("Góc mở của cánh cửa (thường là 90 độ)")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private bool startsOpen = false;

    [Header("Lock Settings")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string requiredKeyId = "";
    [SerializeField] private string lockedMessage = "Cửa đang bị khóa! Cần chìa khóa phù hợp.";
    [Tooltip("Nếu là cửa thoát chính (thoát khỏi nhà) -> Thắng game")]
    [SerializeField] private bool isExitDoor = false;

    [Header("Hinge / Pivot")]
    [Tooltip("Transform trục xoay của cánh cửa (nếu để trống, bản lề sẽ được tính từ bounds của model)")]
    [SerializeField] private Transform doorHinge;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine rotateCoroutine;
    private List<Collider> blockingColliders = new List<Collider>();
    private NavMeshObstacle navObstacle;
    private OffMeshLink doorOffMeshLink;

    // Public Getters cho AIController và các hệ thống khác
    public bool IsOpen => isOpen;
    public bool IsLocked => isLocked;
    public bool IsExitDoor => isExitDoor;
    public string RequiredKeyId => requiredKeyId;
    public Transform DoorHinge => doorHinge;

    protected override void Start()
    {
        base.Start(); // ← FIX: Gọi base.Start() để khởi tạo player reference!
        
        navObstacle = GetComponentInChildren<NavMeshObstacle>();

        if (doorHinge == null)
            doorHinge = CreateRuntimeHingeFromModelBounds();

        // Thu thập toàn bộ các blocking collider (kể cả inactive) để quản lý xuyên phòng
        blockingColliders.Clear();
        foreach (Collider col in GetComponentsInChildren<Collider>(true))
        {
            if (!col.isTrigger)
            {
                blockingColliders.Add(col);
            }
        }

        closedRotation = doorHinge.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (startsOpen)
        {
            isOpen = true;
            doorHinge.localRotation = openRotation;
            SetBlockingCollidersState(true);
        }

        SetupDoorNavMeshLink();
        UpdatePrompt();
    }

    /// <summary>
    /// Tự động thiết lập OffMeshLink bắc cầu NavMesh giữa 2 phòng qua khung cửa
    /// Giúp NavMeshAgent của quái vật di chuyển mượt mà vào phòng ngay khi mở cửa
    /// </summary>
    public void SetupDoorNavMeshLink()
    {
        if (doorOffMeshLink != null) return;

        // Thử cả 2 hướng forward và right để tìm NavMesh ở 2 bên cánh cửa
        Vector3[] checkDirs = new Vector3[] { transform.forward, transform.right };
        NavMeshHit hitA = default;
        NavMeshHit hitB = default;
        bool found = false;

        foreach (Vector3 dir in checkDirs)
        {
            Vector3 posA = transform.position + dir * 1.3f;
            Vector3 posB = transform.position - dir * 1.3f;

            if (NavMesh.SamplePosition(posA, out hitA, 3.0f, NavMesh.AllAreas) &&
                NavMesh.SamplePosition(posB, out hitB, 3.0f, NavMesh.AllAreas))
            {
                if (Vector3.Distance(hitA.position, hitB.position) > 0.8f)
                {
                    found = true;
                    break;
                }
            }
        }

        if (found)
        {
            GameObject linkStart = new GameObject("NavMeshLink_A");
            linkStart.transform.SetParent(transform, true);
            linkStart.transform.position = hitA.position;

            GameObject linkEnd = new GameObject("NavMeshLink_B");
            linkEnd.transform.SetParent(transform, true);
            linkEnd.transform.position = hitB.position;

            doorOffMeshLink = gameObject.AddComponent<OffMeshLink>();
            doorOffMeshLink.startTransform = linkStart.transform;
            doorOffMeshLink.endTransform = linkEnd.transform;
            doorOffMeshLink.biDirectional = true;
            doorOffMeshLink.activated = isOpen;
            doorOffMeshLink.costOverride = -1;
            doorOffMeshLink.autoUpdatePositions = false;
        }
    }

    public void IgnoreCollisionWithMonster(Collider monsterCollider)
    {
        if (monsterCollider == null) return;
        for (int i = 0; i < blockingColliders.Count; i++)
        {
            if (blockingColliders[i] != null)
            {
                Physics.IgnoreCollision(monsterCollider, blockingColliders[i], true);
            }
        }
    }

    private void SetBlockingCollidersState(bool doorIsOpen)
    {
        for (int i = 0; i < blockingColliders.Count; i++)
        {
            if (blockingColliders[i] != null)
            {
                // Khi cửa mở: isTrigger = true để người chơi và quái vật đi xuyên qua, không bị cản trở
                // Khi cửa đóng: isTrigger = false để trở thành cửa cản vật lý
                blockingColliders[i].isTrigger = doorIsOpen;
            }
        }

        if (navObstacle != null)
        {
            navObstacle.enabled = !doorIsOpen;
        }

        if (doorOffMeshLink != null)
        {
            doorOffMeshLink.activated = doorIsOpen;
        }
    }

    private Transform CreateRuntimeHingeFromModelBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return transform;

        Bounds localBounds = new Bounds();
        bool initialized = false;
        foreach (Renderer renderer in renderers)
        {
            Bounds rendererBounds = renderer.localBounds;
            Vector3 min = rendererBounds.min;
            Vector3 max = rendererBounds.max;
            for (int x = 0; x < 2; x++)
            for (int y = 0; y < 2; y++)
            for (int z = 0; z < 2; z++)
            {
                Vector3 corner = transform.InverseTransformPoint(renderer.transform.TransformPoint(new Vector3(
                    x == 0 ? min.x : max.x,
                    y == 0 ? min.y : max.y,
                    z == 0 ? min.z : max.z)));
                if (!initialized)
                {
                    localBounds = new Bounds(corner, Vector3.zero);
                    initialized = true;
                }
                else
                {
                    localBounds.Encapsulate(corner);
                }
            }
        }

        bool widthIsX = localBounds.size.x >= localBounds.size.z;
        float halfWidth = (widthIsX ? localBounds.size.x : localBounds.size.z) * 0.5f;

        // Never move the door root here. Scene doors are already aligned to their wall
        // openings; only a child pivot may be repositioned for rotation.

        GameObject hingeObject = new GameObject("DoorHinge_Runtime");
        Transform hinge = hingeObject.transform;
        hinge.SetParent(transform, false);
        hinge.localPosition = widthIsX
            ? new Vector3(localBounds.center.x - halfWidth, localBounds.min.y, localBounds.center.z)
            : new Vector3(localBounds.center.x, localBounds.min.y, localBounds.center.z - halfWidth);

        // Move visible model roots under the hinge without changing their world pose.
        List<Transform> modelRoots = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child != hinge)
                modelRoots.Add(child);
        }
        foreach (Transform child in modelRoots)
            child.SetParent(hinge, true);

        // The blocking collider must swing with the mesh. Convert a root box collider
        // into an equivalent collider on the hinge child.
        BoxCollider rootBox = GetComponent<BoxCollider>();
        if (rootBox != null)
        {
            bool isTrigger = rootBox.isTrigger;
            PhysicsMaterial material = rootBox.sharedMaterial;
            rootBox.enabled = false;
            Destroy(rootBox);

            BoxCollider hingeBox = hingeObject.AddComponent<BoxCollider>();
            hingeBox.size = new Vector3(
                Mathf.Max(0.01f, localBounds.size.x),
                Mathf.Max(0.01f, localBounds.size.y),
                Mathf.Max(0.01f, localBounds.size.z));
            hingeBox.center = localBounds.center - hinge.localPosition;
            hingeBox.isTrigger = isTrigger;
            hingeBox.sharedMaterial = material;
            if (!hingeBox.isTrigger && !blockingColliders.Contains(hingeBox))
            {
                blockingColliders.Add(hingeBox);
            }
        }

        return hinge;
    }

    void UpdatePrompt()
    {
        if (isLocked)
        {
            interactionPrompt = "Cửa đã khóa (Click / E để thử mở)";
        }
        else
        {
            interactionPrompt = isOpen ? "Click / E để đóng cửa" : "Click / E để mở cửa";
        }
    }

    protected override void Interact()
    {
        if (isLocked)
        {
            if (!string.IsNullOrEmpty(requiredKeyId) && Lockable.HasKey(requiredKeyId))
            {
                isLocked = false;
                Debug.Log($"<color=green>[DOOR] Đã dùng chìa khóa '{requiredKeyId}' mở khóa cửa thành công!</color>");
                GameEvents.TriggerLockOpened(requiredKeyId);
                ToggleDoor();
            }
            else
            {
                Debug.Log($"<color=yellow>[DOOR] {lockedMessage}</color>");
            }
            UpdatePrompt();
            return;
        }

        ToggleDoor();
    }

    public void OpenDoor(bool force = false, float speedMultiplier = 1.0f)
    {
        if (isOpen) return;

        // Cửa thoát hiểm chính không thể bị phá mở bởi quái vật
        if (isExitDoor && force) return;

        if (isLocked)
        {
            if (force)
            {
                isLocked = false;
            }
            else
            {
                return;
            }
        }

        isOpen = true;
        UpdatePrompt();

        if (isOpen && isExitDoor)
        {
            GameEvents.TriggerGameWin();
        }

        // Khi cửa mở: cho phép người chơi và AI đi qua thông suốt ngay lập tức
        SetBlockingCollidersState(true);

        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        Debug.Log($"[DoorInteractive] {gameObject.name} → OPENING (force={force}, speedMult={speedMultiplier})");
        rotateCoroutine = StartCoroutine(AnimateDoor(openRotation, speedMultiplier));
    }

    public void CloseDoor(float speedMultiplier = 1.0f)
    {
        if (!isOpen) return;

        isOpen = false;
        UpdatePrompt();

        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        Debug.Log($"[DoorInteractive] {gameObject.name} → CLOSING");
        rotateCoroutine = StartCoroutine(AnimateDoor(closedRotation, speedMultiplier));
    }

    /// <summary>
    /// Cho phép quái vật (Mirror_Demon_Monster) mở cánh cửa để truy lùng hoặc đuổi bắt player.
    /// Quái vật tuyệt đối không thể mở cửa thoát hiểm (isExitDoor).
    /// </summary>
    /// <param name="isChasing">Nếu đang đuổi bắt (Chase), quái vật sẽ phá cửa mở toang kể cả cửa phòng đang khóa thường.</param>
    /// <param name="monsterCollider">Collider của quái vật để bỏ qua va chạm vật lý với cửa.</param>
    /// <returns>True nếu cửa được mở thành công hoặc đã mở sẵn, False nếu không thể mở (ví dụ cửa thoát hiểm).</returns>
    public bool TryOpenByMonster(bool isChasing = true, Collider monsterCollider = null)
    {
        // Quái vật không bao giờ được phép mở cửa thoát hiểm chính
        if (isExitDoor)
            return false;

        // Bỏ qua va chạm vật lý giữa quái vật và cánh cửa để quái bước qua ngay lập tức
        if (monsterCollider != null)
        {
            IgnoreCollisionWithMonster(monsterCollider);
        }

        // Cửa đã mở sẵn -> đảm bảo OffMeshLink được bật
        if (isOpen)
        {
            if (doorOffMeshLink == null) SetupDoorNavMeshLink();
            if (doorOffMeshLink != null) doorOffMeshLink.activated = true;
            return true;
        }

        // Nếu cửa đang bị khóa:
        // - Khi tuần tra (Patrol): quái vật không mở cửa khóa bí mật
        // - Khi rượt đuổi (Chase): quái vật phá khóa xông vào bắt người chơi
        if (isLocked && !isChasing)
            return false;

        // Khi đang rượt đuổi, mở nhanh hơn (tốc độ x1.5) để tạo cảm giác rùng rợn, dồn dập
        float speedMult = isChasing ? 1.5f : 1.0f;
        OpenDoor(force: isChasing, speedMultiplier: speedMult);
        return true;
    }

    public void ToggleDoor()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor(force: false);
        }
    }

    private IEnumerator AnimateDoor(Quaternion targetRot, float speedMultiplier = 1.0f)
    {
        // FIX: Dùng RotateTowards thay vì Slerp (theo Unity best practices cho door rotation)
        // openSpeed được interpret là multiplier, nhân 30 để ra degrees/second
        float degreesPerSecond = (openSpeed * speedMultiplier) * 30f; // openSpeed=3 → 90°/s → mở 90° trong 1 giây
        
        while (Quaternion.Angle(doorHinge.localRotation, targetRot) > 0.1f)
        {
            doorHinge.localRotation = Quaternion.RotateTowards(
                doorHinge.localRotation,
                targetRot,
                degreesPerSecond * Time.deltaTime
            );
            yield return null;
        }
        
        // Snap to exact target
        doorHinge.localRotation = targetRot;

        // Khi cửa đã đóng kín hoàn toàn: bật lại cản vật lý
        if (!isOpen)
        {
            SetBlockingCollidersState(false);
        }

        Debug.Log($"[DoorInteractive] {gameObject.name} → {(isOpen ? "OPENED" : "CLOSED")}");
    }

    public void Unlock()
    {
        isLocked = false;
        UpdatePrompt();
    }

    public void SetLocked(bool locked, string keyId = "")
    {
        isLocked = locked;
        requiredKeyId = keyId;
        UpdatePrompt();
    }
}
