# MirrorHouse

**Game 3D Kinh Dị Sinh Tồn Góc Nhìn Thứ Nhất - Đồ án môn học Lập trình Game Android**

---

## 1. THÔNG TIN CHUNG

### Thông tin dự án
- **Tên game:** MirrorHouse (Ngôi Nhà Gương)
- **Mục tiêu:** Đồ án môn học Lập trình Game Android
- **Phiên bản hiện tại:** 0.4.0 (URP Shader Fix, Authentic 3D Mansion Furniture & AI Monster Door-Breaching Pursuit)
- **Engine:** Unity 6 (6000.6.0f1)
- **Render Pipeline:** Universal Render Pipeline (URP 17.6.0)
- **Nền tảng đích:** Android (Min SDK: API Level 26 - Android 8.0 Oreo, CPU Architecture: ARM64)
- **Định dạng màn hình:** Khóa cố định Landscape Only (16:9 / 19.5:9 / 20:9)

### Thành viên và phân công vai trò

| Thành viên | Vai trò | Trách nhiệm chính |
|------------|---------|-------------------|
| **Vũ Quốc An** | Role A - Lead Programmer | Lập trình Player Movement & Camera FPS, AI FSM (Patrol/Chase/Catch), AI Door-Breaching xuyên phòng, tương tác generic (Lockable/KeyItem/Drawer/MirrorInteractive), Game Loop Manager & UI Mobile |
| **Hứa Đức Sáng** | Role B - Asset & Environment Integrator | Tìm kiếm và tích hợp Asset 3D (Vintage Living Room, Old Wooden Cabinet, Furniture FREE Pack), chuyển đổi Shader Built-in/HDRP sang URP Lit, thiết lập ánh sáng nến và 5 Reflection Probes |
| **Phạm Bá Tú** | Role C - Level & Puzzle Designer | Thiết kế kiến trúc dinh thự, mở rộng cửa thông phòng (Bedroom <-> Library, Dining <-> Storage), thiết kế chuỗi giải đố liên phòng (3 két sắt, 3 manh mối, 20 tấm gương Gothic), cân bằng độ khó |
| **Phạm Hoa Nhi** | Role E - Product Owner & Documentation | Quản lý backlog, ưu tiên tính năng, viết tài liệu Game Design Document (GDD), cập nhật README và báo cáo tiến độ đồ án |
| **Nguyễn Tuấn Dũng** | *(Chuyển hướng)* | Ban đầu tham gia Role D (UI/Audio), sau đó chuyển sang đồ án 2D riêng |

---

## 2. MÔ TẢ GAMEPLAY & CỐT TRUYỆN

### Cốt truyện
Bạn tỉnh dậy trong một dinh thự cổ kiểu Victoria hoang phế giữa đêm giông bão. Không khí ẩm mốc đặc quánh tử khí, và điều kỳ dị nhất là: khắp các bức tường dinh thự bị bao phủ bởi **20 tấm gương cổ u ám**. Những tấm gương này dường như không chỉ phản chiếu hình ảnh mà còn phản chiếu linh hồn và tiếng thì thầm ma quái của cõi bóng tối. 

Một thực thể tà ác mang tên **Mirror Demon** đang rình rập và tuần tra khắp hành lang. Bạn phải giải mã bí mật trong nhật ký của cô bé Emily, mở các ngăn kéo bí mật, tìm mật mã mở 3 két sắt cổ để thu thập các chìa khóa phòng và cổ vật **Dao tế Cõi Gương (Ritual Dagger)**. Chỉ khi đưa được Dao tế đến phá giải phong ấn của **Đại Gương Thoát Hiểm (Grand Gateway Mirror)** ở cuối hành lang, bạn mới có thể thoát khỏi cơn ác mộng này.

### Thể loại & Cơ chế chơi
- **Thể loại:** 3D First-Person Psychological Survival Horror kết hợp Escape Room (phong cách *Granny* & *Amnesia*).
- **Điều kiện Thắng (Victory):** Giải mã toàn bộ chuỗi câu đố liên phòng, lấy được Dao tế Cõi Gương, mở phong ấn Đại Gương Thoát Hiểm (`GrandGatewayMirror_Exit`) và bước qua cổng giải thoát.
- **Điều kiện Thua (Game Over):** Bị quái vật Mirror Demon bắt kịp trong cự ly tấn công -> Kích hoạt jumpscare và màn hình kết thúc.

---

## 3. HƯỚNG DẪN ĐIỀU KHIỂN

### 📱 Điều khiển trên Mobile (Android)
Hệ thống điều khiển được thiết kế theo chuẩn công thái học ngón tay cái, an toàn tuyệt đối trước phần khuyết camera (notch / tai thỏ) và mép cong màn hình:

| Thao tác | Thành phần điều khiển | Tọa độ & Kích thước (Độ phân giải chuẩn 1920x1080) | Ghi chú công thái học |
|----------|-----------------------|-----------------------------------------------------|------------------------|
| **Di chuyển** | Virtual Fixed Joystick | Tọa độ `(190, 190)`, Kích thước `200x200` | Cách mép trái và đáy 90px; tự nhiên dưới ngón cái trái |
| **Xoay nhìn góc nhìn FPS** | Vuốt nửa phải màn hình | Vùng cảm ứng tự do (Touch Surface) | Tự động loại trừ các nút bấm UI, di chuyển góc nhìn mượt mà |
| **Tương tác (Interact)** | Nút "E" Gothic tròn | Tọa độ `(-190, 190)`, Kích thước `120x120` | Đối xứng hoàn hảo với Joystick ($Y = 190$), cách mép phải 130px |
| **Chạy nhanh (Sprint)** | Nút "CHẠY" (Sprint Toggle) | Tọa độ `(-190, 320)`, Kích thước `100x80` | Nằm thẳng hàng ngay phía trên nút E; chạm ngón cái phải tiện lợi |
| **Tạm dừng (Pause)** | Nút "❚❚" góc trên phải | Tọa độ `(-60, -50)`, Kích thước `70x70` | An toàn góc trên màn hình |
| **Nhập mã số két sắt** | Hộp thoại Code Input | Hiển thị giữa màn hình | Phím số lớn, nút Xác nhận & Đóng |
| **Khóa xoay màn hình** | Landscape Only | Thiết lập trong `ProjectSettings.asset` | Loại bỏ hoàn toàn tình trạng lật khung hình khi cầm điện thoại |

### 💻 Điều khiển trên PC (Unity Editor Testing)
| Thao tác | Phím bấm |
|----------|----------|
| **Di chuyển** | W, A, S, D |
| **Xoay camera** | Chuột (Mouse Look mượt mà) |
| **Tương tác** | Phím **E** |
| **Chạy nhanh** | Giữ phím **Shift Trái** |
| **Tạm dừng Menu** | Phím **Escape** |

---

## 4. TÍNH NĂNG VÀ CÔNG NGHỆ HOÀN THIỆN ✅

### 🎨 Khắc Phục Triệt Để Lỗi Vật Liệu Màu Tím (URP Shader Fix Pipeline)
- [x] **Nguyên nhân được xử lý triệt để:** Các gói asset nhập vào (`Assets/Furniture/`, `Assets/Scenes/ZNS3D/`, `Assets/Models/Furniture/`) vốn được tác giả xây dựng cho Built-in Standard Shader hoặc HDRP, khi đưa vào dự án Unity 6 URP 17.6.0 bị xung đột và Unity hiển thị màu tím lỗi (`#FF00FF`).
- [x] **Chuyển đổi 35 Material sang URP Lit:** Hệ thống tự động nâng cấp toàn bộ 35 material sang shader chuẩn `Universal Render Pipeline/Lit`.
- [x] **Ánh xạ đầy đủ Texture PBR gốc:**
  - Kết nối chính xác texture Albedo/Diffuse vào `_BaseMap`.
  - Kết nối Normal Map vào `_BumpMap` và kích hoạt keyword `_NORMALMAP`.
  - Kết nối Metallic/Mask map vào `_MetallicGlossMap` và kích hoạt keyword `_METALLICSPECGLOSSMAP`.
  - Kết nối Ambient Occlusion vào `_OcclusionMap`.
  - Thiết lập độ mịn (`_Smoothness: 0.5`) và độ kim loại (`_Metallic: 0.1 - 0.2`) giúp đồ gỗ, vải nhung và kim loại phản chiếu ánh sáng tự nhiên.
  - Sửa các material đặc biệt: Kính trong suốt (`Glass`), máy hát đĩa cổ (`Mat_Gramophone`), đèn cổ (`Mat-Antique_Light`), sàn gỗ (`Mat_Floors`), tường hoa văn (`Mat_Walls`).

---

### 🛋️ Hệ Thống 55 Món Nội Thất 3D Chân Thực Phân Bổ 5 Khu Vực
Không còn các khối Cube mô phỏng, toàn bộ 5 khu vực của dinh thự được bố trí bằng các mô hình 3D cổ điển sắc nét, chuẩn kích thước mét thực tế:

1. **Phòng ngủ Emily (Tây Bắc):**
   - **Giường ngủ hoàng gia Victoria** thật (`Victorian_Bed.prefab`) với khung gỗ chạm trổ và nệm bọc ga.
   - Hai tủ đầu giường gỗ kèm **chân nến / đèn ngủ cổ kính** (`antique_light.prefab`).
   - **Tủ quần áo cổ kính** lớn (`SM_cabinet.prefab` - Old Cabinet).
   - **Bàn trang điểm có gương** (`MirrorComode.prefab`) kèm ghế tựa nhung (`chair.prefab`).
   - **Ghế bành quý tộc bọc nhung** (`Fotel2.prefab`) kèm bàn trà tròn (`RoundTable.prefab`).
   - Rương gỗ phong ấn chân giường (`StorageCrate.obj`).
   - **Thảm nhung đỏ lớn** trải sàn phòng ngủ (`carpet_2.prefab`).

2. **Thư viện cổ (Tây Nam):**
   - Dãy **kệ sách đồ sộ** (`bookshelf.prefab`) dọc vách tường Tây và vách Đông.
   - **Bàn làm việc nghiên cứu cổ** (`Vintage_Desk.prefab`) đặt tại trung tâm phòng kèm ghế gỗ học giả.
   - Các chồng sách cổ (`book_1.prefab`, `book_2.prefab`) nằm trên bàn học.
   - **Tủ cấm thư Grimoire** (`SM_cabinet.prefab`) và **Tủ tài liệu hồ sơ** (`Closet3.prefab`) áp sát vách Nam.
   - Góc đọc sách thư viện với **ghế bành thư viện bọc nhung** (`Fotel3.prefab`), bàn trà và **máy hát đĩa than cổ** (`gramophone.prefab`).
   - **Đồng hồ treo tường cổ** (`clock_wall.prefab`) gắn trang trọng trên vách tường.
   - **Thảm hoa văn thư viện** (`carpet_3.prefab`).

3. **Phòng khách & Bàn ăn (Đông Bắc):**
   - **Bàn tiệc dài quý tộc** (`Dining_Table_Set.prefab`) với 4 ghế ăn xếp ngay ngắn xung quanh.
   - **Bộ Sofa phòng khách lớn** (`sofa_big.prefab`) và **Sofa đơn bọc nỉ** (`sofa_small.prefab`).
   - Bàn trà phòng khách (`tea_table 1.prefab`) đặt kèm **bình hoa cổ** (`vase_1.prefab`).
   - **Tủ ly kính phòng ăn** (`SM_cabinet.prefab` - Old Cabinet với cửa kính phản chiếu).
   - Tủ búp phê phòng ăn (`BigCloset.prefab`) sát vách Bắc.
   - **Đèn cây đứng cao** (`light_tall.prefab`) tại góc phòng.
   - Bàn console đón khách sảnh vào kèm bình hoa trang trí (`vase_2.prefab`).
   - **Tranh sơn dầu phong cảnh cổ điển** (`painting_1`, `painting_2`) treo trên các vách tường.
   - **Thảm phòng khách phong cách Victoria** (`carpet_1.prefab`).

4. **Nhà kho & Phòng tế lễ (Đông Nam):**
   - **Bàn Tế Đàn Hắc Ám** (`Ritual_Altar.prefab`) đặt uy nghi tại trung tâm vòng tròn ma pháp.
   - Hai chân nến tế lễ cổ cao tỏa ánh sáng ma quái hai bên tế đàn.
   - Vòng phong ấn tế lễ rùng rợn in trên sàn gỗ (`Mat_Mirror_Tainted.mat`).
   - **Két sắt kim loại bí mật** (`Safe.prefab`) giấu tại góc tối Đông Nam.
   - **Tủ chứa cổ vật tế lễ** (`SM_cabinet.prefab`) sát vách Nam.
   - Các cụm **thùng gỗ tích trữ** (`Barrel.prefab`) và hòm gỗ cũ xếp tầng.
   - Kệ chứa đồ cũ kim loại (`bookshelf.prefab`).

5. **Hành lang chính (Hall of Mirrors):**
   - Giữ thông thoáng 100% trục giữa rộng > 3.5m cho người chơi và quái vật di chuyển.
   - Hai bệ tượng trang trí cổ áp sát mặt tường hành lang.

> [!IMPORTANT]
> **Vùng đệm an toàn quanh các cánh cửa (Clearance Buffer > 7.0m):**
> Toàn bộ nội thất được tính toán khoảng cách cách xa hai cửa thông phòng (`Door_Bedroom_Library`, `Door_Dining_Storage`) và các cửa hành lang trên **7.0 mét**. Tuyệt đối không có đồ vật nào chắn cửa, che khuất hay cản trở việc mở cửa và đi xuyên qua lại.

---

### 👹 Quái Vật AI Demon Monster & Cơ Chế Mở Cửa Đuổi Bắt Xuyên Phòng
- [x] **Finite State Machine (FSM):** Quái vật hoạt động với 3 trạng thái rõ ràng:
  - **Patrol:** Tuần tra tuần tự qua các Waypoint định sẵn khắp hành lang và các phòng.
  - **Chase:** Khi phát hiện người chơi (tầm nhìn Vision Cone + thính giác âm thanh bước chân), quái vật gầm rú và tăng tốc truy đuổi ráo riết.
  - **Catch:** Bắt giữ người chơi trong cự ly tấn công, kích hoạt jumpscare và Game Over.
- [x] **Cơ chế Phá Cửa Đuổi Bắt (Door-Breaking AI Room Entry Pursuit):**
  - Khi người chơi chạy vào phòng và đóng cửa lại, quái vật trong trạng thái Chase sẽ phát hiện cánh cửa cản đường.
  - Quái vật tự động kích hoạt logic mở phá cửa với tốc độ mở nhanh gấp **x1.5 lần**, phát ra âm thanh cửa va đập rùng rợn.
  - **Tự động tạo OffMeshLink bắc cầu NavMesh:** Kết nối liền mạch lưới điều hướng giữa hành lang và sàn bên trong các phòng.
  - **Chuẩn hóa thể hình AI:** Bán kính NavMeshAgent được tối ưu ở mức `0.35m`, chiều cao `1.8m`, `stoppingDistance: 0.4m` kết hợp thuật toán `TraverseThroughObstacleOrDoor`.
  - Quái vật sải bước mượt mà vượt qua khe cửa hẹp và tiến thẳng vào tận trong phòng để truy lùng người chơi, loại bỏ hoàn toàn tình trạng kẹt ngoài cửa hay đứng bất động.
  - Cửa thoát hiểm cuối cùng (`isExitDoor = true`) được bảo vệ riêng biệt, quái vật không thể tự ý mở để đảm bảo tính giải đố của người chơi.

---

### 🪞 Hệ Thống 20 Gương Gothic Treo Tường (The Mirror System)
- [x] **20 Tấm gương được căn chỉnh sát tường từng centimet:**
  - **8 Gương Hành lang chính (Hall of Mirrors):** 4 cặp gương đối xứng dọc hành lang tạo hiệu ứng thị giác phản chiếu vô tận (*Infinite Reflection*).
  - **3 Gương Phòng ngủ Emily:** Gương bàn trang điểm (vệt bàn tay mờ), gương soi toàn thân Gothic đứng, gương đầu giường.
  - **3 Gương Thư viện cổ:** Gương đại sảnh chạm khắc hoa văn huyền bí, gương vòm giữa hai giá sách, gương góc đọc sách tĩnh lặng.
  - **3 Gương Phòng khách & Bàn ăn:** Gương đại tiệc mạ vàng, gương lò sưởi Victoria, gương sảnh đón tiếp.
  - **3 Gương Nhà kho & Tế đàn:** Gương đen tế đàn phong ấn hắc khí, gương nứt dính vết cào máu khô, gương ẩm mốc phát ra tiếng gõ.
- [x] **MirrorInteractive Script:** Cho phép người chơi nhấn E (hoặc chạm nút trên Mobile) để nhìn sâu vào gương, kích hoạt câu thoại tâm lý rùng rợn độc bản cho từng tấm gương kèm hiệu ứng âm thanh ma quái.
- [x] **5 Reflection Probes:** Bao phủ 5 khu vực (Hành lang, Phòng ngủ, Thư viện, Phòng khách/ăn, Nhà kho) giúp mặt kính phản chiếu đồ nội thất và ánh nến lung linh trong URP.

---

### 🚪 Kiến Trúc Dinh Thự & Cửa Thông Phòng (Mansion Architecture)
- [x] **Hai Cửa Thông Phòng Mới (Connecting Doors):**
  - Cửa kết nối Phòng ngủ Emily <-> Thư viện (`Door_Bedroom_Library` tại $X = -9.5, Z = 0$).
  - Cửa kết nối Phòng ăn <-> Nhà kho (`Door_Dining_Storage` tại $X = 9.5, Z = 0$).
  - Vách tường được đục lanh-tô chuẩn xác, cánh cửa gỗ mở xoay 90 độ mượt mà, người chơi và quái vật đi xuyên qua dễ dàng.
- [x] **Hàn kín triệt để khe hở vách tường 0.5m:** Tái tính toán kích thước vách ngăn `Wall_East_Divider_Inner` ($7.05\text{m}$) và `Wall_West_Divider_Inner` ($6.25\text{m}$) ăn khớp hoàn toàn với tường hành lang lệch tâm ($X = 1.92\text{m}$ và $X = -2.70\text{m}$), loại bỏ hoàn toàn khoảng hở nhìn xuyên ra khoảng không gian đen.

---

### 🔐 Chuỗi Câu Đố & Manh Mối Liên Phòng (Puzzle Progression)
- [x] **3 Két sắt mật mã (Safes):**
  - **Két sắt phòng ngủ (`Safe_Bedroom`):** Nhập mã **1894** -> Mở khóa nhận chìa khóa Thư viện (`key_library`).
  - **Két sắt thư viện (`Safe_Library`):** Nhập mã **4219** -> Mở khóa nhận chìa khóa Nhà kho (`key_storage`).
  - **Két sắt nhà kho (`Safe_Storage_Dagger`):** Nhập mã **5731** -> Mở khóa nhận Cổ vật Dao tế Cõi Gương (`key_ritual_dagger`).
- [x] **3 Mẩu giấy manh mối kinh dị (Clue Notes):**
  - **Nhật ký Emily (`Clue_Note_Emily`):** Giấu kín bên trong ngăn kéo trượt bàn học phòng ngủ Emily (gợi ý năm 1894).
  - **Cổ thư Thư viện (`Clue_Note_Library`):** Nằm trên bàn đọc sách thư viện, ghi chép nghi thức 4 bước cõi gương (gợi ý mã 4219).
  - **Mẩu giấy dính máu Nhà kho (`Clue_Note_Storage`):** Nằm dưới chân bàn ăn, ghi lại số vết cào trên vách gỗ (gợi ý mã 5731).
- [x] **DrawerInteractive:** Ngăn kéo bàn học trượt ra mượt mà trong 0.35 giây, vật phẩm manh mối bên trong di chuyển đồng bộ cùng ngăn kéo.

---

### 🩸 Giao Diện & Trải Nghiệm Kinh Dị (Horror UI & HUD)
- [x] **MirrorHouseHorrorHUD:**
  - Viền mờ vignette đỏ thẫm và hiệu ứng sương mù ma mị quanh viền màn hình khi quái vật lại gần.
  - Tâm ngắm con ngươi đổi màu (Dynamic Reticle Text) hiển thị chính xác tên hành động: *"Nhìn vào gương"*, *"Mở cửa"*, *"Kéo ngăn kéo"*, *"Nhập mã két sắt"*, *"Đọc manh mối"*.
  - Bảng thông điệp kinh dị `Horror_Clue_Panel` phong cách Gothic hiển thị nội dung thư tịch và câu thoại gương phản chiếu.
- [x] **CodeInputUI:** Giao diện bàn phím số mở két sắt, kiểm tra đúng/sai trực quan, tự động mở khóa chuột và đóng hộp thoại khi hoàn thành.
- [x] **Pause Menu & Settings:** Tạm dừng thời gian thực, tùy chỉnh độ nhạy camera và âm lượng lưu vào `PlayerPrefs`.

---

## 5. CẤU TRÚC CODE & THƯ MỤC DỰ ÁN

```
Assets/
├── Furniture/                          # Asset pack đồ nội thất (Sofa, ghế, tủ, bàn, bàn trang điểm)
│   ├── Prefabs/                        # 3Seat, chair, BigCloset, closet2, Closet3, Couch, Fotel, MetalTable...
│   └── Source/Materials/               # 17 Materials đã nâng cấp chuẩn URP Lit
├── Old Cabinet/                        # Asset pack tủ gỗ cổ kính kinh dị
│   ├── Prefabs/                        # SM_cabinet.prefab (Tủ gỗ cổ với cửa kính)
│   └── Materials/                      # M_cabinet.mat, M_cabinet_glass.mat (URP Lit)
├── Scenes/
│   ├── Level_Main.unity                # Scene gameplay chính của dinh thự Nhà Gương
│   ├── MainMenu.unity                  # Scene menu khởi đầu game
│   └── ZNS3D/                          # Vintage Living Room Game Pack (URP Materials & Prefabs)
├── Prefabs/
│   ├── Characters/                     # Demon_Monster.prefab (Quái vật AI)
│   └── Furniture/                      # Victorian_Bed, Dining_Table_Set, Safe, Ritual_Altar...
├── Materials/                          # Toàn bộ vật liệu chuẩn URP của dự án
│   ├── Mat_Gothic_Mirror.mat           # Mặt kính gương phản chiếu cao
│   ├── Mat_Mirror_Tainted.mat          # Kính gương ố máu phong ấn
│   ├── Mat_Floor_Wood.mat              # Sàn gỗ dinh thự
│   ├── Mat_Wall_Dark.mat               # Vách tường hoa văn tối màu
│   └── Imported/                       # Mat_Imported_Desk, Bookshelf, BedWood, Altar, Safe...
└── Scripts/
    ├── AI/
    │   ├── AIController.cs             # FSM: Patrol, Chase, Catch, Door-Breaching & OffMeshLink
    │   └── AIState.cs                  # Enum trạng thái tuần tra/đuổi bắt
    ├── Audio/
    │   └── AudioManager.cs             # Singleton quản lý SFX, BGM & Procedural Audio
    ├── Events/
    │   └── GameEvents.cs               # Event bus truyền tín hiệu toàn game
    ├── Interaction/
    │   ├── Interactable.cs             # Base class tương tác
    │   ├── Lockable.cs                 # Hệ thống khóa (Key, Code, Safe, Win Condition)
    │   ├── DoorInteractive.cs         # Cửa xoay 90 độ, kiểm tra chìa khóa
    │   ├── DrawerInteractive.cs       # Ngăn kéo trượt vật lý
    │   ├── MirrorInteractive.cs       # Tương tác 20 tấm gương kinh dị
    │   ├── KeyItem.cs                  # Chìa khóa & cổ vật nhiệm vụ
    │   ├── CodeClue.cs                 # Manh mối giấy & mật mã két sắt
    │   ├── EscapeTrigger.cs           # Vùng trigger chiến thắng tại Đại Gương
    │   └── CameraRaycastInteraction.cs # Bắn tia Raycast tâm camera tương tác
    ├── Managers/
    │   └── MirrorHouseGameManager.cs   # Quản lý trạng thái màn chơi, thời gian, Win/Lose
    ├── Player/
    │   ├── PlayerController.cs         # Di chuyển CharacterController, Gravity, Sprint Toggle
    │   └── PlayerCamera.cs             # FPS Camera, nhạy cảm ứng nửa phải, bỏ qua chạm UI
    ├── UI/
    │   ├── GameUIManager.cs            # Điều phối toàn bộ HUD, Pause, Settings, GameOver, Victory
    │   ├── MainMenuUI.cs               # Điều khiển Menu chính
    │   ├── CodeInputUI.cs              # Hộp thoại nhập mã số mở két
    │   ├── MobileInteractButton.cs     # Nút bấm tương tác trên Mobile
    │   ├── CrosshairUI.cs              # Tâm ngắm đổi màu và hiển thị nhãn hành động
    │   └── MirrorHouseHorrorHUD.cs     # HUD kinh dị, viền sương mù & bảng tin cõi gương
    └── Editor/
        ├── DirectAssetDecorator.cs     # Tự động sửa Shader URP & Bố trí 55 đồ nội thất thật
        ├── MansionDoorsAndPuzzlesEditor.cs # Menu 1-click tạo cửa thông phòng, hàn vách, đặt két sắt
        ├── MansionDecoratorAndMirrorsEditor.cs # Bố trí 20 gương Gothic & Reflection Probes
        └── UISetupBuilder.cs           # Menu 1-click tự động dựng Canvas UI & Managers
```

---

## 6. HƯỚNG DẪN CÀI ĐẶT, THIẾT LẬP VÀ BUILD APK

### 1. Mở dự án trong Unity
- Mở Unity Hub, thêm thư mục dự án `MirrorHouse`.
- Khởi chạy bằng **Unity 6 (6000.6.0f1)**.

### 2. Các công cụ Menu tự động hóa (1-Click Editor Tools)
Dự án đã được tích hợp sẵn các công cụ tự động hóa trên thanh menu Unity:
- `MirrorHouse` -> `Fix All Purple Materials & Decorate Mansion (Direct)`:
  * Nâng cấp toàn bộ 35 material từ Built-in/HDRP sang URP Lit chuẩn PBR.
  * Bố trí trực tiếp 55 đồ nội thất 3D sắc nét vào 5 phòng và lưu vĩnh viễn vào `Level_Main.unity`.
- `MirrorHouse` -> `Setup Connecting Doors & Horror Clues (Permanent)`:
  * Tạo 2 cửa thông phòng Victorian và hàn kín khe hở vách ngăn 0.5m.
  * Cấu hình 3 két sắt mật mã (1894, 4219, 5731) và 3 mẩu giấy manh mối kinh dị.
- `MirrorHouse` -> `Decorate Mansion & Place Wall Mirrors (Permanent)`:
  * Căn chỉnh 20 tấm gương Gothic áp sát tường các phòng và cấu hình 5 Reflection Probes.
- `MirrorHouse` -> `Setup UI for Level_Main`: Tự động tạo Canvas responsive, Joysticks, Mobile Buttons và HUD.

### 3. Chạy thử trên máy tính (Play Mode)
- Mở scene `Assets/Scenes/Level_Main.unity` (hoặc `MainMenu.unity`).
- Nhấn nút **Play** trên thanh công cụ Unity để trải nghiệm trực tiếp bằng bàn phím và chuột.

### 4. Build file APK cho điện thoại Android
1. Vào `File` -> `Build Profiles` (hoặc `Build Settings`).
2. Chọn nền tảng **Android**.
3. Đảm bảo danh sách scene trong bản build gồm:
   - `Assets/Scenes/MainMenu.unity` (Index 0)
   - `Assets/Scenes/Level_Main.unity` (Index 1)
4. Kiểm tra cấu hình `Player Settings`:
   - **Package Name:** `com.DefaultCompany.MirrorHouse`
   - **Minimum API Level:** Android 8.0 'Oreo' (API Level 26)
   - **Target API Level:** Automatic (highest installed)
   - **Scripting Backend:** IL2CPP
   - **Target Architectures:** ARM64
   - **Orientation:** Khóa Landscape (Landscape Right / Landscape Left)
5. Nhấn **Build** (hoặc **Build and Run**) để xuất file APK cài đặt lên điện thoại Android thực tế.
