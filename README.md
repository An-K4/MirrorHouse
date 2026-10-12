# MirrorHouse

Game 3D kinh dị phong cách Granny - Đồ án môn học Lập trình Game Android

---

## 1. THÔNG TIN CHUNG

### Thông tin dự án
- **Tên game:** MirrorHouse
- **Tên nhóm:** *(Đang cập nhật)*
- **Mục tiêu:** Đồ án môn học Lập trình Game Android
- **Thời gian thực hiện:** *(Đang cập nhật)*
- **Phiên bản hiện tại:** 0.1.0

### Thành viên và vai trò

| Thành viên | Vai trò | Mô tả công việc |
|------------|---------|----------------|
| **Vũ Quốc An** | Role A - Programmer | Lập trình hệ thống chính: Player movement & camera, AI FSM (Patrol/Chase/Catch), hệ thống tương tác generic (Lockable/KeyItem), UI và Audio, Testing |
| **Hứa Đức Sáng** | Role B - Asset & Environment Integrator | Tìm kiếm và tích hợp assets 3D, xây dựng môi trường game, lighting, tương tác vật lý cơ bản |
| **Phạm Bá Tú** | Role C - Level & Puzzle Designer | Thiết kế bố cục không gian, thiết kế puzzle (khóa mật mã + chìa khóa), cân bằng độ khó, playtest |
| **Phạm Hoa Nhi** | Role E - Product Owner & Documentation | Quản lý backlog, ưu tiên công việc, viết tài liệu GDD và báo cáo |
| **Nguyễn Tuấn Dũng** | *(Chuyển sang dự án 2D)* | Ban đầu Role D (UI/Audio/Testing), sau đó tập trung làm game 2D riêng |

### Thông tin kỹ thuật
- **Unity Version:** 6000.6.0f1 (Unity 6)
- **Nền tảng đích:** Android
- **Render Pipeline:** Universal Render Pipeline (URP) 17.6.0
- **Android Min SDK:** API Level 26 (Android 8.0) *(Đang xác nhận lại)*
- **Kiến trúc CPU:** ARM64

---

## 2. MÔ TẢ GAME

### Thể loại
**Game 3D kinh dị góc nhìn thứ nhất (First-person Horror)**

Phong cách gameplay tương tự **Granny**: người chơi phải thoát khỏi ngôi nhà bị ma ám trong khi tránh né kẻ địch AI tuần tra và đuổi theo.

### Ý tưởng chính
Người chơi thức dậy trong một ngôi nhà bí ẩn và cần tìm cách thoát ra. Để mở cửa chính, người chơi phải giải các puzzle bằng cách tìm chìa khóa và giải mã các ổ khóa mật mã. Trong khi đó, một thực thể AI nguy hiểm đang tuần tra và sẽ đuổi theo nếu phát hiện người chơi.

### Cốt truyện và bối cảnh
*(Đang hoàn thiện bởi team tài liệu)*

Dự án hiện tập trung vào xây dựng mechanics và gameplay core, cốt truyện chi tiết sẽ được bổ sung trong giai đoạn hoàn thiện.

**Concept chính:** Người chơi (Emily) bị mắc kẹt trong ngôi nhà ma ám và cần tìm đường thoát. Có hai kết cục tiềm năng:
- **Kết cục 1:** Tìm được cửa chính và thoát khỏi nhà
- **Kết cục 2:** Phá giải lời nguyền tại nhà gương song song *(Chưa triển khai trong phiên bản hiện tại)*

### Luật chơi

#### Mục tiêu
Tìm cách thoát khỏi ngôi nhà mà không bị AI bắt được.

#### Điều kiện thắng
- Tìm được **cửa chính** (hoặc cửa thoát)
- Mở khóa cửa (có thể cần chìa khóa hoặc giải mã)
- Rời khỏi nhà thành công

#### Điều kiện thua
- Bị AI bắt được → Game Over

#### Cơ chế chính
1. **Khám phá:** Di chuyển qua các phòng, tìm kiếm vật phẩm và manh mối
2. **Giải puzzle:** 
   - **Khóa mật mã:** Tìm các mảnh giấy gợi ý để biết mã số đúng, sau đó nhập mã vào ổ khóa
   - **Khóa chìa vật lý:** Tìm chìa khóa phù hợp để mở cửa/tủ/hộp
3. **Lén lút:** Tránh né AI bằng cách quan sát hướng di chuyển và ẩn náu
4. **Tương tác:** Tương tác với cửa, ngăn kéo, ổ khóa, vật phẩm

### Cách điều khiển

#### Mobile Controls (Android)
| Thao tác | Điều khiển |
|----------|------------|
| **Di chuyển** | Virtual Joystick (góc trái dưới màn hình) |
| **Nhìn xung quanh** | Vuốt ngón tay trên nửa bên phải màn hình |
| **Tương tác** | Nút "Interact" trên màn hình (khi đứng gần vật thể có thể tương tác) |
| **Nhập mã khóa** | Bàn phím ảo xuất hiện khi tương tác với ổ khóa mật mã |
| **Chạy nhanh (Sprint)** | *(Chưa có trong phiên bản hiện tại)* |

#### PC Controls (Unity Editor - Testing)
| Thao tác | Phím |
|----------|------|
| **Di chuyển** | W, A, S, D |
| **Nhìn xung quanh** | Di chuyển chuột |
| **Tương tác** | E |

---

## 3. NỘI DUNG GAME

### Danh sách màn chơi

#### Level_Main
- **Trạng thái:** Đang phát triển
- **Mô tả:** Màn chơi chính với đầy đủ mechanics
- **Nội dung:** *(Đang cập nhật bởi Role C - Level Designer)*

#### Demo_Topic7_Interaction
- **Trạng thái:** Demo/Testing
- **Mô tả:** Scene demo hệ thống tương tác (cửa, két sắt, chìa khóa)
- **Mục đích:** Test và trình bày hệ thống Lockable/KeyItem

#### Level_Test_Interactive
- **Trạng thái:** Testing
- **Mô tả:** Scene thử nghiệm các tính năng tương tác
- **Mục đích:** Kiểm thử và debug

### Nhân vật

#### Người chơi (Player)
- **Tên:** Emily
- **Loại:** First-person character (góc nhìn thứ nhất)
- **Thông số:** *(Đang cập nhật bởi Role C)*
  - Tốc độ di chuyển: ~5 m/s (mặc định trong code)
  - Tốc độ sprint: Chưa có
  - Tầm tương tác: ~3m
  - Máu/Mạng sống: Chưa cài đặt (hiện tại bị bắt = thua ngay)

### Kẻ địch / AI

#### AI Controller
- **Tên:** *(Chưa có - có thể đặt bởi team tài liệu)*
- **Loại:** AI ma/quái vật tuần tra
- **Hành vi:** FSM (Finite State Machine) với 3 trạng thái:
  - **Patrol (Tuần tra):** Di chuyển giữa các waypoints theo đường định sẵn
  - **Chase (Đuổi theo):** Khi phát hiện người chơi, tăng tốc độ đuổi theo
  - **Catch (Bắt):** Khi đến gần người chơi, kích hoạt Game Over
- **Thông số:** *(Đang cập nhật bởi Role C)*
  - Tốc độ tuần tra: 0.25 (25% tốc độ gốc - đã giảm để dễ chơi)
  - Tốc độ đuổi: 0.5625 (56.25% tốc độ gốc)
  - Tầm phát hiện: ~12m
  - Góc nhìn (FOV): ~120°
  - Tầm bắt: ~2m
- **Số lượng:** *(Đang cập nhật bởi Role C)*

### Vật phẩm

*(Đang cập nhật bởi Role C - Level Designer)*

Các loại vật phẩm dự kiến:
- **Chìa khóa (Key Items):** Mở các ổ khóa vật lý (cửa, tủ, hộp)
- **Mảnh giấy gợi ý (Code Clues):** Chứa thông tin về mã số
- **Vật phẩm đặc biệt:** *(Chưa xác định)*

Hệ thống vật phẩm sử dụng **generic Lockable/KeyItem system** - Role C có thể dễ dàng thêm vật phẩm mới mà không cần lập trình viên sửa code.

### Kỹ năng
*(Chưa có trong phiên bản hiện tại)*

### Hệ thống điểm / Tiền / Phần thưởng
*(Chưa có trong phiên bản hiện tại)*

Game hiện tập trung vào mechanics trốn thoát và giải puzzle, chưa có hệ thống tính điểm hay thu thập tiền.

---

## 4. CHỨC NĂNG

### Các tính năng đã hoàn thành ✅

#### Core Gameplay
- [x] **Player Movement:** Di chuyển với Virtual Joystick (mobile) hoặc WASD (PC)
- [x] **Player Camera:** Camera góc nhìn thứ nhất với chuột/touch controls
- [x] **AI Patrol:** AI tuần tra giữa các waypoints
- [x] **AI Chase:** AI phát hiện và đuổi theo người chơi
- [x] **AI Catch:** AI bắt người chơi (trigger game over logic)

#### Hệ thống tương tác
- [x] **Lockable System (Generic):** Hệ thống khóa tổng quát, hỗ trợ 2 loại:
  - Key Lock: Cần chìa khóa vật lý
  - Code Lock: Cần nhập mã số đúng
- [x] **KeyItem System:** Vật phẩm chìa khóa có thể nhặt và lưu vào inventory
- [x] **Door Interaction:** Tương tác với cửa (mở/đóng, kiểm tra khóa)
- [x] **Drawer Interaction:** Tương tác với ngăn kéo (kéo ra/đẩy vào)

### Các tính năng đang phát triển 🚧

*(Đang cập nhật bởi Role C)*

Theo kế hoạch hiện tại:
- [ ] **UI Integration:** Hoàn thiện và kết nối các màn hình UI
- [ ] **Audio Integration:** Thêm âm thanh và hiệu ứng âm thanh
- [ ] **Animation System:** Hoạt hình cho cửa, ngăn kéo, AI, người chơi
- [ ] **Complete Game Loop:** Nối các hệ thống thành một vòng chơi hoàn chỉnh

### Các tính năng chưa làm ❌

- [ ] **UI: Mobile Interact Button** *(Đã có code, chưa tích hợp hoàn chỉnh)*
- [ ] **UI: Code Input Panel** *(Đã có code, chưa test đầy đủ)*
- [ ] **UI: Game Over Screen**
- [ ] **UI: Victory Screen**
- [ ] **UI: Main Menu**
- [ ] **UI: Settings Menu**
- [ ] **UI: Pause Menu**
- [ ] **Audio: Footsteps SFX**
- [ ] **Audio: Ambient Sounds**
- [ ] **Audio: Jumpscare Sounds**
- [ ] **Audio: UI SFX**
- [ ] **Save/Load System**
- [ ] **Multiple Levels**
- [ ] **Tutorial/Instructions**

### Giao diện (UI)

#### UI Components đã implement
1. **CodeInputUI:** Bàn phím ảo để nhập mã số
   - Trạng thái: Code đã có, chưa test đầy đủ
   - Chức năng: Hiện popup nhập mã khi tương tác với Code Lock

2. **MobileInteractButton:** Nút tương tác cho mobile
   - Trạng thái: Code đã có, chưa tích hợp hoàn chỉnh
   - Chức năng: Thay thế phím E trên mobile

3. **CrosshairUI:** Tâm ngắm ở giữa màn hình
   - Trạng thái: Code đã có
   - Chức năng: Hiển thị điểm tương tác

#### UI Components chưa có
- Main Menu
- Settings (âm lượng, độ nhạy camera, graphics)
- Pause Menu
- Game Over Screen
- Victory Screen
- Inventory Display (hiển thị chìa khóa đã nhặt)
- Health Bar / Status Display

### Âm thanh

**Trạng thái:** Chưa implement

**Kế hoạch:**
- Footsteps (bước chân người chơi và AI)
- Ambient (nhạc nền kinh dị)
- Door/Drawer sounds (âm thanh mở cửa, kéo ngăn)
- Jumpscare (âm thanh khi bị AI bắt)
- UI sounds (click button)

**Nguồn dự định:** Freesound.org và các nguồn free khác *(Đang cập nhật bởi Role B)*

### Lưu dữ liệu

**Trạng thái:** Chưa implement

**Kế hoạch:** Sử dụng PlayerPrefs để lưu:
- Tiến độ game
- Settings (âm lượng, độ nhạy)
- Unlocked levels

### Quảng cáo / Thanh toán
*(Không có - Đây là đồ án học tập, không phải game thương mại)*

---

## 5. CẤU TRÚC KỸ THUẬT

### Cây thư mục

```
MirrorHouse/
├── Assets/
│   ├── 3D model/              # Models 3D từ các nguồn
│   ├── Audio/                 # (Trống - chưa có âm thanh)
│   ├── Materials/             # Materials cho 3D objects
│   ├── Models/               # Organized 3D models
│   │   ├── Characters/       # Character models (DemonMonster, Player)
│   │   ├── Environment/      # Environment assets (walls, floors)
│   │   ├── Furniture/        # Furniture (Bookshelf, MasterBed, VintageDesk)
│   │   ├── Items/            # Interactive items (OrnateKey, SafeBox)
│   │   └── Props/            # Decorative props (Candelabra, GrandMirror, RitualAltar)
│   ├── Prefabs/              # Prefab objects
│   │   ├── AI/               # AI prefabs
│   │   ├── Characters/       # Player prefabs
│   │   ├── Furniture/        # Furniture prefabs
│   │   ├── Interactables/    # Interactive object prefabs
│   │   ├── Items/            # Item prefabs
│   │   ├── Player/           # Player-related prefabs
│   │   ├── Props/            # Prop prefabs
│   │   └── Waypoints/        # AI waypoint prefabs
│   ├── Scenes/               # Game scenes
│   │   ├── Level_Main.unity           # Màn chơi chính
│   │   ├── Demo_Topic7_Interaction.unity  # Demo tương tác
│   │   └── Level_Test_Interactive.unity   # Test scene
│   ├── Scripts/              # C# Scripts
│   │   ├── AI/               # AI-related scripts
│   │   ├── Editor/           # Unity Editor tools
│   │   ├── Events/           # Event system (nếu có)
│   │   ├── Interaction/      # Interaction system scripts
│   │   ├── Level/            # Level-specific scripts (nếu có)
│   │   ├── Managers/         # Game managers (nếu có)
│   │   ├── Player/           # Player scripts
│   │   └── UI/               # UI scripts
│   ├── Settings/             # Project settings
│   ├── UI/                   # UI assets
│   ├── Joystick Pack/        # Third-party joystick asset
│   └── TextMesh Pro/         # TextMesh Pro package
└── ProjectSettings/          # Unity project settings
```

### Danh sách Scenes và luồng chuyển

| Scene | Mục đích | Trạng thái | Luồng chuyển |
|-------|----------|------------|--------------|
| **Level_Main** | Màn chơi chính với đầy đủ mechanics | Đang phát triển | *(Chưa có Main Menu)* → Level_Main → *(Chưa có Game Over/Victory)* |
| **Demo_Topic7_Interaction** | Demo hệ thống tương tác cho báo cáo | Hoàn thành | Standalone demo |
| **Level_Test_Interactive** | Test và debug tính năng | Development | Không dùng trong build cuối |

**Luồng dự kiến (chưa implement):**
```
Main Menu → Level_Main → (Thắng: Victory Screen / Thua: Game Over Screen) → Main Menu
```

### Các script chính

#### AI System
| Script | Chức năng | Quan hệ |
|--------|-----------|---------|
| **AIController.cs** | FSM điều khiển AI (Patrol/Chase/Catch), sử dụng NavMeshAgent | Tìm và đuổi theo PlayerController |

**Thông số kỹ thuật:**
- States: Patrol, Chase, Catch
- NavMesh-based pathfinding
- Detection: Raycast + distance check + FOV check
- Tunable: patrol speed, chase speed, detection radius, FOV angle

#### Player System
| Script | Chức năng | Quan hệ |
|--------|-----------|---------|
| **PlayerController.cs** | Di chuyển player với CharacterController + Joystick/WASD | Đọc input từ Joystick component |
| **PlayerCamera.cs** | First-person camera với mouse/touch look controls | Gắn vào Player, xử lý rotation |

**Thông số kỹ thuật:**
- Movement: CharacterController-based (không dùng Rigidbody)
- Input: Virtual Joystick cho mobile, WASD cho PC
- Camera: Mouse delta / Touch delta → Rotation
- Gravity: Custom gravity application

#### Interaction System
| Script | Chức năng | Quan hệ |
|--------|-----------|---------|
| **Interactable.cs** | Base class cho tất cả objects có thể tương tác | Abstract class, các class khác kế thừa |
| **Lockable.cs** | Hệ thống khóa generic (Key hoặc Code) | Kế thừa Interactable, gọi CodeInputUI hoặc check inventory |
| **KeyItem.cs** | Vật phẩm chìa khóa có thể nhặt | Kế thừa Interactable, add key vào Lockable.Inventory |
| **DoorInteractive.cs** | Tương tác cửa (mở/đóng) | Kế thừa Interactable, có thể kết hợp với Lockable |
| **DrawerInteractive.cs** | Tương tác ngăn kéo (kéo/đẩy) | Kế thừa Interactable, animation world-space direction |
| **CameraRaycastInteraction.cs** | Raycast từ camera để detect objects có thể tương tác | Gọi Interact() trên Interactable khi nhấn E/tap button |
| **CodeClue.cs** | Mảnh giấy gợi ý mã số (nếu có) | Kế thừa Interactable |

**Design Pattern: Component-based với Inheritance**
```
Interactable (abstract)
  ├─ Lockable (khóa generic: Key/Code)
  ├─ KeyItem (chìa khóa nhặt được)
  ├─ DoorInteractive (cửa)
  ├─ DrawerInteractive (ngăn kéo)
  └─ CodeClue (gợi ý)

CameraRaycastInteraction → Raycast → Detect Interactable → Call Interact()
```

#### UI System
| Script | Chức năng | Quan hệ |
|--------|-----------|---------|
| **CodeInputUI.cs** | UI nhập mã số cho Code Lock | Được gọi bởi Lockable khi interact với Code Lock |
| **MobileInteractButton.cs** | Nút Interact cho mobile | Gọi CameraRaycastInteraction.TryInteract() |
| **CrosshairUI.cs** | Hiển thị tâm ngắm | Standalone UI element |

#### Managers và Events
| Script | Chức năng | Trạng thái |
|--------|-----------|------------|
| **World2GameManager.cs** | Game manager cho World 2 (nếu có) | *(Cần xác nhận chức năng)* |
| **GameEvents.cs** | Event system (OnCodeCorrect, OnKeyPickup, OnPlayerCaught...) | *(Có thể có trong Events/ folder)* |

#### Editor Tools
| Script | Chức năng | Mục đích |
|--------|-----------|----------|
| **FixNavMeshWalls.cs** | Fix NavMesh trên tường cho Unity 6 | Giải quyết bug Unity 6 với NavMesh |
| **SceneFurnitureAdjuster.cs** | Tool điều chỉnh vị trí furniture | Hỗ trợ level design |
| **DoorSetupTool.cs** | Tool setup cửa nhanh | Tăng tốc độ level design |
| **UISetupBuilder.cs** | Tool tạo UI structure | Tự động tạo Canvas và UI elements |
| **World2SceneBuilder.cs** | Tool xây scene World 2 | Automation cho scene building |

### Prefab quan trọng

*(Đang cập nhật bởi Role B và Role C)*

Các prefabs dự kiến:
- **Player Prefab:** Character + Camera + Controller scripts
- **AI Prefab:** AI model + NavMeshAgent + AIController + Waypoints
- **Door Prefabs:** Nhiều loại cửa với DoorInteractive + Lockable (optional)
- **Drawer Prefabs:** Ngăn kéo với DrawerInteractive
- **Key Prefabs:** Chìa khóa với KeyItem + unique keyId
- **Lock Prefabs:** Ổ khóa với Lockable (Code hoặc Key type)

**Ưu điểm của hệ thống Generic:**
Role C (Level Designer) có thể:
1. Kéo thả prefab vào scene
2. Chỉnh lockId/keyId trong Inspector
3. Không cần Role A (Programmer) viết thêm code

### Packages, Plugins và SDK

#### Unity Packages (từ Package Manager)
| Package | Version | Mục đích |
|---------|---------|----------|
| **AI Navigation** | 2.0.14 | NavMesh pathfinding cho AI |
| **Universal Render Pipeline (URP)** | 17.6.0 | Graphics rendering pipeline |
| **Input System** | 1.20.0 | New Input System (chưa dùng đầy đủ) |
| **TextMesh Pro** | 6.6.0 | UI text rendering |
| **Timeline** | 6.6.0 | Animation và cutscenes |
| **Visual Scripting** | 1.9.12 | Visual scripting (nếu dùng) |
| **Test Framework** | 1.8.0 | Unit testing |

#### Asset Store Packages
| Package | Tác giả | Giấy phép | Mục đích |
|---------|---------|-----------|----------|
| **Joystick Pack** | Fenerax Studios | Asset Store (Free hoặc Standard License) | Virtual joystick cho mobile controls |

#### SDK và Tools
- **Unity 6 (6000.6.0f1):** Engine chính
- **Android SDK:** Build target cho Android
- **NavMesh Components:** Baking và runtime NavMesh

### Cách xử lý các hệ thống chính

#### 1. Di chuyển (Movement)
**PlayerController.cs:**
- **Component:** CharacterController (không dùng Rigidbody)
- **Input:** 
  - Mobile: Đọc từ Virtual Joystick component (Horizontal/Vertical)
  - PC: Input.GetAxis("Horizontal"/"Vertical") hoặc Input System
- **Logic:**
  ```
  Vector3 move = (forward * vertical + right * horizontal) * speed * Time.deltaTime
  controller.Move(move)
  ```
- **Gravity:** Tự apply gravity vào velocity.y
- **Không có Sprint:** Tốc độ cố định

#### 2. Va chạm (Collision)
**CharacterController:**
- Tự động xử lý va chạm với colliders
- OnControllerColliderHit() callback nếu cần

**Interactables:**
- BoxCollider hoặc MeshCollider trên objects
- Raycast từ camera để detect interaction (không dùng OnTriggerEnter)

**AI:**
- NavMeshAgent tự động tránh obstacles
- NavMesh Obstacle trên dynamic objects (nếu cần)

**Layer Collision Matrix:**
- *(Đang cập nhật bởi Role C - cần kiểm tra Project Settings → Physics)*

#### 3. AI (Pathfinding và Behavior)
**AIController.cs + NavMeshAgent:**

**Bước 1: NavMesh Baking**
- Role C: Bake NavMesh trong scene (Navigation window)
- Đánh dấu walkable areas
- Đặt NavMesh Obstacles trên furniture/walls

**Bước 2: FSM Logic**
```
State: PATROL
- agent.destination = patrolPoints[currentIndex]
- Nếu đến waypoint → chuyển waypoint tiếp theo
- Liên tục check: Player trong tầm nhìn? → Chuyển CHASE

State: CHASE
- agent.destination = player.position (update mỗi frame)
- agent.speed = chaseSpeed
- Nếu mất dấu player quá lâu → quay PATROL
- Nếu distance < attackRadius → Chuyển CATCH

State: CATCH
- Trigger GameOver
- Disable player controls
- (Chưa có: Play animation/sound)
```

**Detection:**
- Distance check: Vector3.Distance(ai, player) < detectionRadius
- FOV check: Vector3.Angle(ai.forward, toPlayer) < fieldOfView/2
- Line of sight: Raycast (optional - chưa rõ có implement không)

#### 4. Spawn System
**Hiện tại:** Objects được đặt sẵn trong scene bởi Role C

**AI Spawn:**
- AI prefab đặt sẵn tại vị trí start
- Waypoints (empty GameObjects) đặt trong scene

**Item Spawn:**
- Key items và clues đặt sẵn trong scene
- Chưa có random spawn hoặc runtime spawn

#### 5. UI System
**Canvas Setup:**
- Canvas Render Mode: Screen Space - Overlay (cho mobile UI)
- Canvas Scaler: Scale With Screen Size (Reference Resolution: 1920x1080 - cần xác nhận)

**UI Flow (dự kiến):**
```
Main Menu (chưa có)
  ↓
Level_Main
  - HUD: Crosshair, Interact Button (mobile), Inventory icons (chưa có)
  - Popup: Code Input Panel (khi interact với Code Lock)
  ↓
Game Over (chưa có) / Victory (chưa có)
  ↓
Quay lại Main Menu
```

**Event System:**
- EventSystem required cho UI input (đã có script tự tạo nếu thiếu)

#### 6. Lưu dữ liệu (Save/Load)
**Trạng thái:** Chưa implement

**Kế hoạch:** PlayerPrefs
```csharp
// Save
PlayerPrefs.SetInt("LevelCompleted", levelIndex);
PlayerPrefs.SetFloat("BestTime", time);
PlayerPrefs.Save();

// Load
int level = PlayerPrefs.GetInt("LevelCompleted", 0);
```

**Dữ liệu cần lưu:**
- Tiến độ level
- Settings (volume, sensitivity)
- Unlocked content

**Lưu ý:** PlayerPrefs là giải pháp đơn giản cho đồ án, production game nên dùng JSON hoặc binary serialization.

---

## 6. TÀI NGUYÊN

### Nguồn Assets

#### 3D Models
**Trạng thái:** *(Đang cập nhật bởi Role B - Asset Integrator)*

Các models hiện có:
- **Characters:** DemonMonster (AI), Player character
- **Furniture:** Bookshelf, MasterBed, VintageDesk
- **Props:** Candelabra, GrandMirror, RitualAltar
- **Items:** OrnateKey, SafeBox
- **Doors:** GothicDoor

**Nguồn dự kiến:**
- Tổng hợp từ nhiều nguồn (theo nhận định)
- Có thể bao gồm:
  - Kenney.nl (CC0)
  - Sketchfab (CC0 hoặc CC-BY)
  - Unity Asset Store (Free assets)
  - Quaternius (CC0)
  - Tự làm (nếu có)

**Giấy phép:** *(Cần Role B xác nhận từng asset cụ thể)*

#### Audio
**Trạng thái:** Chưa có

**Nguồn dự kiến:**
- Freesound.org (CC0, CC-BY)
- Unity Asset Store (Free audio packs)

#### UI và Icons
**Joystick Pack:**
- **Nguồn:** Unity Asset Store
- **Tác giả:** Fenerax Studios
- **Giấy phép:** Asset Store Standard License (hoặc Free license - cần kiểm tra trong Asset Store)

**Fonts:**
- **TextMesh Pro Default Fonts:** SDF fonts đi kèm TextMesh Pro (Free)
- **LegacyRuntime.ttf:** Font thay thế cho Unity 6 (thay Arial.ttf)

### Giấy phép Assets bên thứ ba

| Asset | Nguồn | Giấy phép | Ghi chú |
|-------|-------|-----------|---------|
| Joystick Pack | Unity Asset Store - Fenerax Studios | Asset Store License | *(Cần kiểm tra Free/Paid)* |
| 3D Models | *(Nhiều nguồn)* | *(Đang xác nhận bởi Role B)* | Cần list đầy đủ |
| TextMesh Pro | Unity Technologies | Unity Companion License | Free, đi kèm Unity |
| Unity Packages | Unity Technologies | Unity Companion License | Free |

**Lưu ý quan trọng:** 
- Với đồ án học tập, cần đảm bảo tuân thủ giấy phép của từng asset
- Nếu publish public (GitHub, itch.io), cần kiểm tra kỹ giấy phép cho phép redistribution
- Asset Store Standard License thường KHÔNG cho phép redistribute source files

### Dung lượng

**Trạng thái:** *(Chưa đo - sẽ cập nhật sau khi Role C hoàn thiện và build)*

**Ước tính:**
- APK size: 50-150 MB (tùy thuộc số lượng assets 3D và audio)
- Uncompressed install size: 100-300 MB

**Lưu ý:** 
- Unity 6 với URP thường tạo APK lớn hơn Built-in RP
- 3D models với textures chiếm dung lượng chính
- Nén texture và audio có thể giảm đáng kể dung lượng

---

## 7. CÀI ĐẶT VÀ BUILD

### Cách mở dự án

#### Yêu cầu hệ thống
- **Unity Hub:** Phiên bản mới nhất
- **Unity Editor:** 6000.6.0f1 (Unity 6)
- **Android Build Support:** Module Android Build Support + Android SDK & NDK Tools + OpenJDK
- **Dung lượng:** ~15 GB (Unity + Android tools + project)

#### Các bước mở dự án
1. **Cài đặt Unity Hub:**
   - Tải từ https://unity.com/download
   - Cài đặt Unity Hub

2. **Cài đặt Unity 6000.6.0f1:**
   - Mở Unity Hub → Installs → Install Editor
   - Chọn phiên bản **6000.6.0f1**
   - Modules cần thiết:
     - ✅ Android Build Support
     - ✅ Android SDK & NDK Tools
     - ✅ OpenJDK
     - ✅ Windows Build Support (nếu build cho PC)

3. **Clone dự án:**
   ```bash
   git clone https://github.com/An-K4/MirrorHouse.git
   cd MirrorHouse
   ```

4. **Mở dự án trong Unity:**
   - Mở Unity Hub
   - Add → Chọn folder MirrorHouse
   - Click vào project để mở
   - Unity sẽ import assets (5-10 phút lần đầu)

5. **Mở scene:**
   - Assets/Scenes/Level_Main.unity (màn chơi chính)
   - Hoặc Demo_Topic7_Interaction.unity (demo tương tác)

### Cách build APK hoặc AAB

#### Chuẩn bị Build
1. **Chuyển nền tảng sang Android:**
   - File → Build Settings
   - Chọn Android → Switch Platform (chờ 5-15 phút)

2. **Thêm scenes vào build:**
   - Build Settings → Scenes In Build
   - Add Open Scenes (hoặc kéo thả scenes từ Project window)
   - ✅ Level_Main.unity
   - ❌ Demo scenes và Test scenes (bỏ tick)

3. **Cấu hình Player Settings:**
   - Build Settings → Player Settings
   - **Company Name:** DefaultCompany *(hoặc đổi tên nhóm)*
   - **Product Name:** MirrorHouse
   - **Package Name:** com.DefaultCompany.MirrorHouse *(hoặc custom)*
   - **Version:** 0.1.0
   - **Bundle Version Code:** 1

#### Build APK (cho testing)
1. **Build Settings:**
   - Build System: Gradle
   - Build App Bundle (Google Play): **Tắt** (OFF)
   - Compression Method: LZ4 (nhanh hơn, file lớn hơn)

2. **Player Settings → Other Settings:**
   - Minimum API Level: **Android 8.0 (API Level 26)** *(xác nhận lại)*
   - Target API Level: **Automatic (highest installed)**
   - Scripting Backend: IL2CPP (recommended cho ARM64)
   - Target Architectures: ✅ **ARM64** (bỏ ARMv7 để giảm dung lượng)

3. **Build:**
   - Build Settings → Build
   - Chọn nơi lưu APK (VD: Builds/MirrorHouse_v0.1.0.apk)
   - Chờ build (10-30 phút tùy máy)

4. **Install:**
   - Copy APK sang Android qua USB hoặc cloud
   - Enable "Install from Unknown Sources" trên Android
   - Tap APK để cài đặt

#### Build AAB (cho Google Play)
1. **Build Settings:**
   - Build App Bundle (Google Play): **Bật** (ON)

2. **Keystore (cho release build):**
   - Player Settings → Publishing Settings
   - Create new keystore (lần đầu)
   - Lưu keystore file an toàn (mất là không publish được)

3. **Build:**
   - Build Settings → Build
   - Chọn nơi lưu AAB
   - Upload lên Google Play Console

### Cấu hình Build quan trọng

| Setting | Giá trị | Lý do |
|---------|---------|-------|
| **Package Name** | com.DefaultCompany.MirrorHouse | Định danh duy nhất trên Android |
| **Minimum API Level** | 26 (Android 8.0) | Hỗ trợ đa số thiết bị (>90% thị phần) |
| **Target API Level** | Automatic (highest) | Tận dụng tính năng Android mới nhất |
| **Scripting Backend** | IL2CPP | Bắt buộc cho ARM64, performance tốt hơn Mono |
| **Target Architecture** | ARM64 | Google Play yêu cầu từ 2019, thiết bị hiện đại |
| **Compression Method** | LZ4 (testing) / LZ4HC (release) | LZ4: build nhanh, LZ4HC: file nhỏ hơn |
| **Stripping Level** | Low (testing) / Medium (release) | Giảm dung lượng bằng cách loại bỏ code không dùng |

**Lưu ý:**
- Build lần đầu mất 20-30 phút
- Build sau nhanh hơn (~5-10 phút) nếu ít thay đổi
- IL2CPP build lâu hơn Mono nhưng performance tốt hơn
- ARM64-only giảm ~30% dung lượng so với ARMv7+ARM64

---

## 8. KIỂM THỬ

### Thiết bị đã test

| Thiết bị | Hãng & Model | Android Version | Kết quả |
|----------|--------------|-----------------|---------|
| **Vsmart Live 4** | Vsmart | Android 11 | *(Đang cập nhật kết quả test chi tiết)* |

**Lưu ý:** Cần test thêm trên nhiều thiết bị với các specs khác nhau:
- Màn hình nhỏ (5-6 inch)
- Màn hình lớn (6.5+ inch)
- RAM thấp (3-4 GB)
- RAM cao (6-8+ GB)
- Android versions khác nhau (8.0, 9.0, 10, 11, 12, 13, 14)

### FPS (Frames Per Second)
**Trạng thái:** *(Chưa đo - sẽ cập nhật sau khi Role C hoàn thiện và test)*

**Mục tiêu:**
- 30 FPS: Acceptable cho mobile
- 60 FPS: Ideal (có thể khó đạt trên thiết bị cũ với URP)

**Cách đo:**
- Unity Profiler (khi chạy trên device qua USB)
- Hoặc hiển thị FPS counter trong game (thêm script nếu cần)

### Dung lượng bản build
**Trạng thái:** *(Chưa đo - sẽ cập nhật sau khi build)*

**Ước tính:**
- APK (ARM64 only): 50-100 MB
- Installed size: 100-200 MB

### Lỗi đã biết

#### 1. Chưa build đa dạng cho các kích cỡ màn hình
- **Mô tả:** UI có thể bị lỗi trên màn hình aspect ratio khác 16:9
- **Impact:** UI elements bị cắt hoặc sai vị trí
- **Workaround:** Chưa có
- **Plan:** Role C sẽ kiểm tra và fix Canvas Scaler settings

#### 2. *(Các lỗi khác sẽ được cập nhật bởi Role C sau khi test đầy đủ)*

### Phản hồi người chơi thử
**Trạng thái:** *(Chưa có phiên playtest chính thức)*

**Kế hoạch:**
- Internal playtest với team members
- Feedback về difficulty, controls, bugs
- Iterate based on feedback

---

## 9. ĐÁNH GIÁ

### Khó khăn chính và cách giải quyết

#### 1. Hiểu và cài đặt các component Unity cơ bản
**Khó khăn:**
- Phải học và hiểu rõ nhiều component phức tạp:
  - **CharacterController:** Cách hoạt động, khác biệt với Rigidbody
  - **Rigidbody:** Khi nào dùng, khi nào không dùng
  - **Colliders (Box/Mesh/Capsule):** Loại nào phù hợp cho từng object
  - **NavMesh:** Baking, NavMeshAgent, NavMesh Obstacles
  - **Camera controls:** First-person rotation, mobile touch input
- Kéo thả và cài đặt objects trong scene phức tạp, dễ sai

**Cách giải quyết:**
- Học qua tài liệu Unity documentation
- Xem tutorials và videos
- Thử nghiệm và debug trực tiếp
- Dần dần làm quen với workflow Unity
- Đã phần nào nắm vững các concepts cơ bản

#### 2. Unity 6 mới - Bugs và Breaking Changes
**Khó khăn:**
- Unity 6 mới release, có bugs (VD: NavMesh trên walls không hoạt động đúng)
- Font compatibility issues (Arial.ttf → LegacyRuntime.ttf)
- Ít tutorials và resources cho Unity 6

**Cách giải quyết:**
- Viết Editor scripts để fix (VD: FixNavMeshWalls.cs)
- Search Unity forums và GitHub issues
- Thử nhiều workarounds khác nhau
- Document các fixes để tái sử dụng

#### 3. Git merge conflicts với Unity scenes và prefabs
**Khó khăn:**
- Scene files (.unity) và prefab files là YAML text nhưng khó merge thủ công
- Một người chỉnh scene, merge conflict với người khác
- Dễ làm mất data hoặc references bị broken

**Cách giải quyết:**
- Sử dụng Unity Smart Merge (UnityYAMLMerge)
- Communication trong team về ai đang chỉnh scene nào
- Backup thường xuyên
- Tạo các scene riêng cho testing

#### 4. Mobile controls khác biệt với PC
**Khó khăn:**
- Test trên PC (mouse/keyboard) nhưng target là mobile (touch)
- Virtual Joystick phải setup và configure đúng
- Interact button chưa tích hợp hoàn chỉnh

**Cách giải quyết:**
- Dùng Joystick Pack từ Asset Store
- Viết code hỗ trợ cả PC và mobile input
- Test thường xuyên trên thiết bị thật qua USB debugging
- Sử dụng Unity Remote (optional)

### Hạn chế hiện tại

1. **Chưa biết cách build đa dạng cho các kích cỡ màn hình khác nhau**
   - UI chưa responsive tốt
   - Cần học về Canvas Scaler và Anchors

2. **Chưa có UI hoàn chỉnh**
   - Chưa có Main Menu, Game Over Screen, Victory Screen
   - Chưa có Settings menu
   - Interact button và Code Input Panel chưa polish

3. **Chưa có âm thanh**
   - Không có SFX (bước chân, cửa, jumpscare)
   - Không có nhạc nền ambient

4. **Chưa có animations đầy đủ**
   - Cửa và ngăn kéo chưa có animation mượt mà
   - AI chưa có attack animation
   - Player chưa có hand animations

5. **Chỉ có 1 level đang phát triển**
   - Chưa có nhiều màn chơi
   - Chưa có tutorial level

6. **Chưa có save/load system**
   - Người chơi không thể lưu tiến độ
   - Phải chơi lại từ đầu mỗi lần

7. **Performance chưa được tối ưu đầy đủ**
   - Chưa có LOD (Level of Detail) cho models
   - Chưa implement occlusion culling
   - Chưa đo và tối ưu draw calls

8. **Chưa có localization**
   - Chỉ hỗ trợ một ngôn ngữ
   - UI text hardcoded trong code

*(Role C sẽ cập nhật thêm các hạn chế khác sau khi test đầy đủ)*

### Hướng phát triển

#### Ngắn hạn (Hoàn thiện MVP - 1-2 tuần)
1. **Nối UI hoàn chỉnh:**
   - Main Menu với Start Game, Settings, Quit
   - Game Over Screen khi bị AI bắt
   - Victory Screen khi thoát được nhà
   - Pause Menu

2. **Thêm âm thanh:**
   - Footsteps cho player và AI
   - Ambient horror music
   - Door/drawer sounds
   - Jumpscare sound khi bị bắt
   - UI click sounds

3. **Thêm animations:**
   - Cửa mở/đóng smooth
   - Ngăn kéo kéo/đẩy
   - AI attack animation
   - Idle animations

4. **Polish và bug fixes:**
   - Sửa lỗi UI trên nhiều kích cỡ màn hình
   - Cân bằng độ khó (AI speed, detection range)
   - Improve controls feel

5. **Build và test trên nhiều thiết bị:**
   - Test performance (FPS)
   - Fix compatibility issues
   - Measure và optimize APK size

#### Trung hạn (Mở rộng nội dung - 2-4 tuần)
1. **Thêm levels:**
   - 2-3 màn chơi với bố cục khác nhau
   - Tăng độ khó dần dần (nhiều AI hơn, puzzle phức tạp hơn)

2. **Thêm mechanics mới:**
   - Hiding spots (tủ, giường) để trốn AI
   - Flashlight với battery limited
   - Health system thay vì one-hit death
   - Noise system (chạy = ồn, AI nghe được)

3. **Improve AI:**
   - Search behavior khi mất dấu player
   - Multiple AI với patrol routes khác nhau
   - Different AI types (fast/slow, high detection/low detection)

4. **Save/Load system:**
   - Lưu tiến độ, settings
   - Checkpoint system trong level

#### Dài hạn (Nếu có thời gian)
1. **Story mode:**
   - Hoàn thiện cốt truyện
   - Cutscenes và dialogue
   - Multiple endings

2. **Nhà gương song song (Mirror House concept):**
   - Mechanic chuyển đổi giữa 2 thế giới
   - Puzzle dựa trên việc thao tác ở một thế giới ảnh hưởng thế giới kia

3. **Polish và publish:**
   - Professional UI/UX
   - Marketing materials
   - Publish lên Google Play Store (nếu muốn)

---

## 10. MỤC RIÊNG CHO GAME 3D

### Nguồn mô hình 3D và công cụ tạo

**Trạng thái:** *(Đang cập nhật chi tiết bởi Role B - Asset Integrator)*

**Models hiện có:**
- DemonMonster, GothicDoor, OrnateKey, SafeBox, Bookshelf, MasterBed, VintageDesk, Candelabra, GrandMirror, RitualAltar, StorageCrate

**Nguồn gốc:**
- Tổng hợp từ nhiều nguồn free 3D assets
- Có thể bao gồm: Kenney.nl, Sketchfab CC0, Unity Asset Store (free), Quaternius
- *(Cần Role B xác nhận từng asset cụ thể và nguồn)*

**Công cụ tạo:**
- Asset có sẵn: Downloaded và imported trực tiếp
- Nếu có chỉnh sửa: Blender (free, open-source)
- Import vào Unity: FBX hoặc OBJ format

### Số đa giác trung bình

**Trạng thái:** *(Chưa đo - cần Role B kiểm tra từng model)*

**Ước tính:**
- **Nhân vật Player:** *(Chưa xác định - cần kiểm tra)*
- **AI (DemonMonster):** *(Chưa đo)*
- **Cảnh chính (Level_Main):** *(Chưa đo tổng polycount)*
- **Furniture average:** 500-2000 tris mỗi piece (ước tính cho low-poly mobile)

**Lưu ý:** 
- Mobile game nên giữ polycount thấp (low-poly style)
- Mục tiêu: <50k triangles cho toàn scene visible cùng lúc
- Cần kiểm tra với Unity Statistics window khi chơi

### Render Pipeline và ánh sáng

**Render Pipeline:**
- **Universal Render Pipeline (URP) 17.6.0**
- Lý do chọn URP: 
  - Tối ưu cho mobile
  - Balance giữa quality và performance
  - Hỗ trợ tốt cho Unity 6

**Ánh sáng (Lighting):**
- **Loại:** *(Đang cập nhật bởi Role B)*
- **Dự kiến:**
  - **Realtime lighting:** Directional Light cho ánh sáng môi trường chính
  - **Baked lighting:** Lightmaps cho static objects (walls, floors) để tăng performance
  - **Mixed lighting:** Kết hợp realtime và baked cho balance

**Lighting Settings:**
- *(Cần Role B kiểm tra Window → Rendering → Lighting)*
- Skybox: Default hoặc custom dark skybox cho horror atmosphere
- Ambient lighting: Low intensity cho horror mood
- Fog: Có thể dùng để tạo không khí kinh dị

### Cách làm hoạt hình (Animation)

**Trạng thái:** *(Đang phát triển)*

**Animator system:**
- Unity Animator với Mecanim system
- Animation Controller cho AI và interactive objects

**Loại animations:**
1. **AI Character:**
   - *(Chưa rõ: Humanoid hay Generic rig)*
   - Idle, Walk, Run, Attack animations
   - Dùng Animator Controller với blend trees cho smooth transitions

2. **Interactive Objects:**
   - **Cửa (Door):** Rotation animation (mở 90 degrees)
   - **Ngăn kéo (Drawer):** Translation animation (kéo ra theo local axis)
   - Có thể dùng code-driven animation hoặc Animation Clips

3. **Player:**
   - First-person nên không cần full body animation
   - Có thể có hand animations (nếu visible)
   - Head bob animation khi di chuyển (optional)

**IK (Inverse Kinematics):**
- *(Chưa sử dụng trong phiên bản hiện tại)*

### Loại camera và góc nhìn

**Camera Type:**
- **First-Person Camera (FPS)**

**Implementation:**
- Camera gắn trực tiếp vào Player GameObject
- **PlayerCamera.cs** xử lý rotation:
  - Mouse/Touch input → Camera rotation (pitch)
  - Player rotation (yaw)
- Sensitivity điều chỉnh được trong code

**Camera Settings:**
- Field of View (FOV): ~60-75 degrees (standard FPS)
- Near Clip Plane: 0.1
- Far Clip Plane: 1000 (có thể giảm để tăng performance)
- Culling Mask: Everything (hoặc exclude specific layers)

**Special Effects (nếu có):**
- Head bob: Nhẹ nhàng khi di chuyển
- Camera shake: Khi bị AI bắt hoặc events đặc biệt
- FOV change: Khi sprint (nếu có sprint feature)

### Các biện pháp tối ưu

**Trạng thái:** *(Đang cập nhật - chưa implement đầy đủ)*

**Đã làm / Đang có:**
1. **URP Pipeline:**
   - Sử dụng URP thay vì Built-in RP
   - Tối ưu tốt cho mobile

2. **ARM64 only:**
   - Chỉ build cho ARM64, không build ARMv7
   - Giảm ~30% dung lượng APK

**Chưa làm / Cần làm:**
1. **LOD (Level of Detail):**
   - Chưa có LOD cho models
   - Nên thêm LOD Groups: High detail (gần), Medium (trung bình), Low (xa)

2. **Occlusion Culling:**
   - Chưa bake Occlusion Culling
   - Nên bake để không render objects bị che khuất

3. **Draw Call Optimization:**
   - Chưa đo số draw calls
   - Nên:
     - Combine meshes (Static Batching)
     - Sử dụng GPU Instancing cho repeated objects
     - Atlas textures để giảm material count

4. **Texture Optimization:**
   - Chưa kiểm tra texture sizes
   - Nên:
     - Compress textures (ASTC cho Android)
     - Giảm texture resolution cho objects nhỏ
     - Sử dụng mipmaps

5. **Lighting Optimization:**
   - Nên bake static lighting (lightmaps)
   - Giảm số realtime lights
   - Disable shadows cho lights xa

**Số đo trước/sau tối ưu:**
- *(Chưa có measurements - cần đo sau khi implement optimizations)*
- Sẽ đo: FPS, Draw Calls, Triangles count, Memory usage

---

## KẾT LUẬN

**MirrorHouse** là dự án đồ án môn học Game Android với mục tiêu tạo ra một game 3D kinh dị phong cách Granny. Dự án đã hoàn thành các **core mechanics** chính bao gồm player movement, AI behavior (patrol/chase/catch), và hệ thống tương tác generic (Lockable/KeyItem).

Dự án hiện đang trong giai đoạn **hoàn thiện và polish**, cần thêm UI, âm thanh, animations và testing để tạo thành một game hoàn chỉnh có thể chơi được trên Android.

**README này sẽ được cập nhật liên tục** khi các thành viên (đặc biệt Role B và Role C) hoàn thiện các phần công việc của mình.

---

*Phiên bản README: 1.0*  
*Ngày cập nhật: 2026-10-12*  
*Người tổng hợp: Role A (Vũ Quốc An) với thông tin từ team members*
