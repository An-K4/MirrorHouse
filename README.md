# MirrorHouse

Game 3D kinh dị phong cách Granny - Đồ án môn học Lập trình Game Android

---

## 1. THÔNG TIN CHUNG

### Thông tin dự án
- **Tên game:** MirrorHouse
- **Mục tiêu:** Đồ án môn học Lập trình Game Android
- **Phiên bản hiện tại:** 0.2.0 (Core MVP & Complete Game Loop)

### Thành viên và vai trò

| Thành viên | Vai trò | Mô tả công việc |
|------------|---------|----------------|
| **Vũ Quốc An** | Role A - Programmer | Lập trình hệ thống chính: Player movement & camera, AI FSM (Patrol/Chase/Catch), hệ thống tương tác generic (Lockable/KeyItem/Drawer), UI, Audio & Game Loop |
| **Hứa Đức Sáng** | Role B - Asset & Environment Integrator | Tìm kiếm và tích hợp assets 3D, xây dựng môi trường game, lighting, vật lý |
| **Phạm Bá Tú** | Role C - Level & Puzzle Designer | Thiết kế bố cục không gian, thiết kế puzzle (khóa mật mã + chìa khóa), cân bằng độ khó, playtest |
| **Phạm Hoa Nhi** | Role E - Product Owner & Documentation | Quản lý backlog, ưu tiên công việc, viết tài liệu GDD và báo cáo |
| **Nguyễn Tuấn Dũng** | *(Chuyển sang dự án 2D)* | Ban đầu Role D (UI/Audio/Testing), sau đó tập trung làm game 2D riêng |

### Thông tin kỹ thuật
- **Unity Version:** 6000.6.0f1 (Unity 6)
- **Nền tảng đích:** Android
- **Render Pipeline:** Universal Render Pipeline (URP) 17.6.0
- **Android Min SDK:** API Level 26 (Android 8.0)
- **Kiến trúc CPU:** ARM64

---

## 2. MÔ TẢ GAME

### Thể loại
**Game 3D kinh dị góc nhìn thứ nhất (First-person Horror)**

Gameplay tương tự **Granny**: Người chơi phải tìm chìa khóa, giải mật mã và mở cửa thoát khỏi ngôi nhà ma ám trong khi lẩn tránh thực thể AI đang tuần tra truy lùng.

### Điều kiện thắng / thua
- **Thắng (Victory):** Tìm được chìa khóa hoặc mã số để mở cửa chính / Gương cổ thoát hiểm (Grand Mirror Gateway) và bước qua vùng trốn thoát.
- **Thua (Game Over):** Bị thực thể AI tiếp cận trong cự ly tấn công -> Trigger jumpscare và màn hình Game Over.

### Cách điều khiển

#### Mobile Controls (Android)
| Thao tác | Điều khiển |
|----------|------------|
| **Di chuyển** | Virtual Joystick (góc dưới bên trái) |
| **Xoay nhìn** | Vuốt ngón tay nửa bên phải màn hình (tự động nhận diện không xung đột UI) |
| **Tương tác** | Nút "E" (Interact) trên màn hình khi ngắm vào vật thể |
| **Chạy nhanh (Sprint)**| Nút "SPRINT" trên màn hình |
| **Tạm dừng (Pause)** | Nút "❚❚" ở góc trên bên phải |
| **Nhập mã khóa** | Bàn phím số / InputField với nút "Nhập mã" và "Đóng" |

#### PC Controls (Unity Editor / Windows Testing)
| Thao tác | Phím |
|----------|------|
| **Di chuyển** | W, A, S, D |
| **Nhìn xung quanh** | Di chuyển chuột |
| **Tương tác** | Phím E |
| **Chạy nhanh** | Giữ phím Shift trái |
| **Tạm dừng** | Phím Escape |

---

## 3. TÍNH NĂNG VÀ HỆ THỐNG ĐÃ HOÀN THIỆN ✅

### Core Gameplay & AI
- [x] **Player Movement:** Di chuyển đa hướng với CharacterController mượt mà, hỗ trợ cả Joystick và bàn phím WASD.
- [x] **Player Camera:** Góc nhìn thứ nhất (FPS), hỗ trợ xoay vuốt mượt mà, tự động bỏ qua chạm trên UI, hỗ trợ điều chỉnh độ nhạy từ Cài đặt.
- [x] **Sprint Mechanic:** Chạy nhanh tăng 1.5x tốc độ, đồng bộ tần số âm thanh bước chân.
- [x] **AI Patrol:** AI tuần tra giữa các Waypoint trên NavMesh.
- [x] **AI Chase:** AI phát hiện người chơi theo bán kính và góc nhìn (FOV) rồi tăng tốc đuổi theo.
- [x] **AI Catch:** Kích hoạt sự kiện người chơi bị bắt, ngưng chuyển động và phát jumpscare.

### Hệ thống tương tác (Generic Interaction System)
- [x] **Interactable (Base Class):** Raycast từ tâm camera (Crosshair) chính xác đối tượng người chơi đang nhìn vào.
- [x] **Lockable System:** Khóa vạn năng hỗ trợ 2 chế độ:
  - Khóa chìa (Key Lock): Yêu cầu chìa tương ứng trong túi đồ.
  - Khóa mật mã (Code Lock): Mở giao diện nhập mã CodeInputUI, báo lỗi khi sai và tự động mở khi đúng.
  - Hỗ trợ đánh dấu Win Condition (mở cửa thoát = Thắng).
- [x] **DoorInteractive:** Cửa xoay mở/đóng 90 độ mượt mà quanh bản lề trục xoay, hỗ trợ khóa chìa và gắn cờ cửa thoát hiểm.
- [x] **DrawerInteractive:** Đã sửa kế thừa Interactable, hỗ trợ kéo trượt ngăn kéo ra/vào, hỗ trợ khóa chìa.
- [x] **KeyItem:** Chìa khóa có thể nhặt, tự động lưu vào túi đồ tĩnh và thông báo UI Inventory.
- [x] **CodeClue:** Mảnh giấy ghi manh mối mật mã, nhặt sẽ mở giao diện đọc manh mối trên màn hình.
- [x] **EscapeTrigger:** Vùng trigger kiểm tra điều kiện chìa khóa và kích hoạt chiến thắng.

### Giao diện người dùng (UI System)
- [x] **Responsive Canvas:** Tự động co giãn theo tỷ lệ màn hình (1920x1080 Match 0.5f) không bị lệch trên mọi dòng máy Android.
- [x] **HUD:** Tâm ngắm (Crosshair), hiển thị túi đồ nhặt được (`Túi đồ: [🗝️ key_library]`), nút Pause, nút Sprint, nút Interact.
- [x] **CodeInputUI:** Hộp thoại nhập mật mã với nút Submit, nút Đóng/Cancel và thông báo kết quả trực quan.
- [x] **Clue Viewer:** Bảng đọc nội dung giấy gợi ý có nút đóng.
- [x] **Pause Menu:** Bảng tạm dừng với các nút Tiếp tục, Cài đặt, Chơi lại, Menu chính.
- [x] **Settings Menu:** Thanh trượt điều chỉnh Độ nhạy camera, Âm lượng Master, Âm lượng BGM, Âm lượng SFX (lưu tự động vào PlayerPrefs).
- [x] **Game Over Screen:** Màn hình đỏ kinh dị khi bị bắt với nút Chơi lại và Menu chính.
- [x] **Victory Screen:** Màn hình chiến thắng hiển thị thời gian hoàn thành màn chơi cùng nút chơi lại.
- [x] **Main Menu Screen:** Giao diện Menu chính hoàn chỉnh với Bắt đầu chơi, Cài đặt, Thoát game.

### Hệ thống âm thanh (Audio System)
- [x] **AudioManager Singleton:** Quản lý âm lượng tổng (Master, Music, SFX).
- [x] **Procedural Audio Synthesizer:** Tự động tổng hợp âm thanh tại runtime (không cần chờ import asset ngoài vẫn có âm thanh):
  - Âm bước chân (Footstep thuds) thay đổi theo nhịp đi bộ / chạy nhanh.
  - Tiếng nhạc nền kinh dị trầm u ám (Ambient horror drone).
  - Tiếng jumpscare chói gắt khi bị bắt.
  - Tiếng nhặt chìa khóa (chime metallic).
  - Tiếng mở khóa thành công (click/clack).
  - Tiếng lật giấy manh mối (rustle paper).
  - Tiếng click các nút UI.
  - Tiếng nhạc chiến thắng khi trốn thoát.

---

## 4. CẤU TRÚC CODE & CÁC SCRIPT CHÍNH

```
Assets/Scripts/
├── AI/
│   ├── AIController.cs             # FSM: Patrol -> Chase -> Catch
│   └── AIState.cs                  # Enum trạng thái AI
├── Audio/
│   └── AudioManager.cs             # Singleton quản lý BGM, SFX & tổng hợp âm thanh
├── Events/
│   └── GameEvents.cs               # Event bus kết nối toàn bộ hệ thống
├── Interaction/
│   ├── Interactable.cs             # Base class cho mọi vật thể tương tác
│   ├── Lockable.cs                 # Hệ thống khóa (Key/Code/Win condition)
│   ├── DoorInteractive.cs         # Cửa mở 90 độ, kiểm tra khóa
│   ├── DrawerInteractive.cs       # Ngăn kéo trượt, kiểm tra khóa
│   ├── KeyItem.cs                  # Vật phẩm chìa khóa nhặt được
│   ├── CodeClue.cs                 # Giấy manh mối mật mã
│   ├── EscapeTrigger.cs           # Vùng trigger thoát hiểm chiến thắng
│   └── CameraRaycastInteraction.cs # Bắn tia Raycast từ tâm camera tương tác
├── Managers/
│   └── MirrorHouseGameManager.cs   # Quản lý vòng lặp chơi game (Loop/State/Time)
├── Player/
│   ├── PlayerController.cs         # Di chuyển CharacterController, Gravity, Sprint
│   └── PlayerCamera.cs             # FPS Camera xoay nhìn, nhạy cảm ứng
├── UI/
│   ├── GameUIManager.cs            # Quản lý toàn bộ HUD, Pause, Settings, GameOver, Victory
│   ├── MainMenuUI.cs               # Điều khiển màn hình menu chính
│   ├── CodeInputUI.cs              # Hộp thoại nhập mã khóa
│   ├── MobileInteractButton.cs     # Nút bấm tương tác trên mobile
│   ├── CrosshairUI.cs              # Tâm ngắm đổi màu khi ngắm vật thể
│   └── MirrorHouseHorrorHUD.cs     # Hiệu ứng viền mắt và sương mù kinh dị
└── Editor/
    ├── UISetupBuilder.cs           # Menu 1-click tự động dựng toàn bộ UI & Managers
    ├── DoorSetupTool.cs            # Tự động gắn bản lề cho cửa trong scene
    ├── FixNavMeshWalls.cs          # Sửa lỗi NavMesh cho Unity 6
    ├── SceneFurnitureAdjuster.cs   # Căn chỉnh vị trí nội thất trong Level_Main
    └── World2SceneBuilder.cs       # Trình dựng scene thế giới 2 hoàn chỉnh
```

---

## 5. HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY THỬ

1. **Mở Unity:** Mở dự án bằng **Unity 6 (6000.6.0f1)**.
2. **Thiết lập tự động chỉ với 1 click:**
   - Trên thanh menu Unity, chọn:
     - `MirrorHouse` -> `Setup UI for Level_Main`: Tự động tạo Canvas responsive, Joysticks, Mobile Buttons, Code Input, Managers và HUD.
     - `MirrorHouse` -> `Create Main Menu Scene & Configure Build Settings`: Tự động tạo scene `MainMenu.unity` và thêm cả 2 scene vào Build Settings.
3. **Chạy game:**
   - Nhấn **Play** trong Unity Editor hoặc build APK cài đặt lên điện thoại Android.
