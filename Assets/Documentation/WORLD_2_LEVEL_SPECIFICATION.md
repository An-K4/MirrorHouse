# WORLD 2: THE MIRROR REALM (NHÀ GƯƠNG)
## TÀI LIỆU THIẾT KẾ LEVEL & CÂU ĐỐ (LEVEL & PUZZLE SPECIFICATION)

---

### 1. TỔNG QUAN LEVEL (LEVEL OVERVIEW)
* **Tên màn chơi**: World 2 — The Mirror Realm (Nhà Gương)
* **Bối cảnh**: Không gian song song bị đảo ngược của căn biệt thự giữa rừng mà Emily thừa kế. Mọi đèn điện biến thành nến/đèn dầu, gương treo vỡ rải rác khắp nơi.
* **Nhân vật**: Emily (Người chơi) trốn chạy Quỷ Gương (AI Demon) trong lúc tìm cách phá giải phong ấn để thoát khỏi nhà gương.
* **Thời lượng hoàn thành mục tiêu**: 10 – 15 phút (cho lần chơi đầu tiên).
* **Quy mô kiến trúc**: 1 tầng, gồm 6 phòng chính kết nối vòng tròn khép kín (Loop Layout), tránh tối đa các ngõ cụt gây kẹt người chơi khi bị AI rượt đuổi.

---

### 2. SƠ ĐỒ MẶT BẰNG & BỐ CỤC KHU VỰC (ASCII LAYOUT)

`	ext
               +----------------------------------------+
               |        [R05] RITUAL ROOM (EXIT)        |
               |       (Tấm Gương Lớn Thoát Hiểm)       |
               +-------------------+--------------------+
                                   | [LOCK_DOOR_RITUAL]
                                   | (Cửa phòng nghi lễ)
               +-------------------+--------------------+
               |           [R02] CENTRAL HALL           |
               |       (Sảnh Gương - AI Thường Qua)     |
               +---------+--------------------+---------+
                         |                    |
        [LOCK_DOOR_LIB]  |                    | [LOCK_DOOR_BED]
                         |                    |
       +-----------------+---+            +---+-----------------+
       |     [R03] STUDY     |            |  [R04] BEDROOM      |
       |     & LIBRARY       |            |    (Phòng Bố)       |
       |  (Phòng đọc sách)   |            | (Tủ đồ & Két sắt)   |
       +----------+----------+            +----------+----------+
                  |                                  |
                  | [DOOR_SECRET_01]                 | [SHORTCUT_01]
                  | (Lối tắt phụ)                    | (Lối tắt phụ)
                  |                                  |
       +----------+----------+            +----------+----------+
       |   [R01] FOYER       +------------+   [R06] STORAGE     |
       |  (Player Spawn)     | Hành lang  |  (Kho nến & bùa)    |
       +---------------------+   chính    +---------------------+
`

---

### 3. DANH SÁCH PHÒNG (ROOM LIST)

| ID | Tên Phòng | Chức năng Level Design | Đồ vật / Puzzle chứa bên trong | Mức độ nguy hiểm |
| :--- | :--- | :--- | :--- | :---: |
| **R01** | **Foyer (Tiền sảnh)** | Điểm Player xuất phát (Spawn). Khu vực an toàn ban đầu để làm quen di chuyển. | Bàn kính vỡ, manh mối khởi đầu CLUE_01_NOTE. | Thấp |
| **R02** | **Central Hall (Đại sảnh)** | Trục giao thông chính kết nối 4 hướng, khu vực tuần tra trung tâm của AI. | Gương treo tường, ngã rẽ vào các phòng. | Rất cao |
| **R03** | **Study (Phòng đọc sách)** | Nơi chứa ghi chép của người cha và két sắt gia tộc. | Kệ sách, bàn làm việc, CLUE_02_BOOK, két sắt LOCK_SAFE_STUDY, chìa KEY_BEDROOM. | Trung bình |
| **R04** | **Bedroom (Phòng ngủ cha)**| Nơi thờ phụng riêng tư, chứa tranh quỷ và nhật ký giao ước. | Giường cổ, bàn trang điểm, CLUE_03_DIARY, tranh rách giấu KEY_RITUAL, lối tắt sang R06. | Cao |
| **R05** | **Ritual Room (Phòng Nghi lễ)** | Phòng tế lễ cuối cùng, nơi đặt Tấm Gương Khổng Lồ dẫn lối về thế giới thực. | Bệ đá tế lễ, Tấm Gương Lớn LOCK_EXIT_MIRROR (Exit Goal). | Cực cao |
| **R06** | **Storage (Kho tế lễ)** | Nhà kho chứa nến và đồ thờ cũ, đường nối Foyer và Bedroom. | Kệ gỗ, nến tế, chìa khóa thư viện KEY_STUDY. | Trung bình |

---

### 4. ĐƯỜNG DI CHUYỂN CHÍNH & ĐƯỜNG TẮT (MAIN ROUTE & SHORTCUTS)

#### A. Main Route (Tuyến chính)
1. **START (R01 Foyer)**: Đọc CLUE_01_NOTE -> Đi qua hành lang phụ vào **R06 Storage**.
2. Nhặt chìa khóa KEY_STUDY trên kệ nến tại **R06**.
3. Di chuyển ra **R02 Central Hall** -> Dùng KEY_STUDY mở cửa LOCK_DOOR_STUDY vào **R03 Study**.
4. Đọc sách CLUE_02_BOOK -> Giải mật mã đảo ngược năm sinh (8991) mở két LOCK_SAFE_STUDY -> Nhặt KEY_BEDROOM.
5. Băng qua **R02 Central Hall** -> Dùng KEY_BEDROOM mở cửa LOCK_DOOR_BEDROOM vào **R04 Bedroom**.
6. Đọc CLUE_03_DIARY lấy mã 4072 -> Lấy KEY_RITUAL sau bức tranh rách.
7. Tiến vào **R05 Ritual Room** qua cửa LOCK_DOOR_RITUAL -> Nhập mã 4072 tại LOCK_EXIT_MIRROR -> **CHIẾN THẮNG (WIN)**.

#### B. Shortcut & Stealth Route (Tuyến né AI)
* **SHORTCUT_01 (Storage <-> Bedroom)**: Cánh cửa bí mật kết nối giữa phòng kho R06 và phòng ngủ R04. 
* Cho phép người chơi khi bị AI rượt đuổi ở Central Hall có thể rút lui vào Bedroom và thoát thẳng sang Storage mà không bị chặn đầu.

---

### 5. DATABASE KHÓA / CHÌA / MÃ SỐ (UNIQUE IDS)

| ID Đối Tượng | Loại Component | Vị trí đặt | Mở / Tác động | Điều kiện mở | Phần thưởng |
| :--- | :--- | :--- | :--- | :--- | :--- |
| CLUE_01_NOTE | CodeClue | Mặt bàn R01 Foyer | Cung cấp gợi ý tìm chìa | Tương tác (E) | Gợi ý tìm KEY_STUDY ở Kho |
| KEY_STUDY | KeyItem | Kệ gỗ R06 Storage | Cửa thư viện R03 | Nhặt (E) | Cấp quyền mở LOCK_DOOR_STUDY |
| LOCK_DOOR_STUDY | Lockable (Key) | Cửa R02 -> R03 | Mở cửa vào R03 Study | Cần KEY_STUDY | Lối vào R03 Study |
| CLUE_02_BOOK | CodeClue | Bàn đọc R03 Study | Gợi ý mã két sắt | Tương tác (E) | Quy luật đảo ngược năm 1998 -> 8991 |
| LOCK_SAFE_STUDY | Lockable (Code)| Két sắt góc R03 Study| Cửa két sắt | Mã số 8991 | KEY_BEDROOM bên trong |
| KEY_BEDROOM | KeyItem | Trong két R03 Study | Cửa phòng ngủ R04 | Mở két để lấy | Cấp quyền mở LOCK_DOOR_BEDROOM |
| LOCK_DOOR_BEDROOM| Lockable (Key) | Cửa R02 -> R04 | Mở cửa vào R04 Bedroom| Cần KEY_BEDROOM| Lối vào R04 Bedroom |
| CLUE_03_DIARY | CodeClue | Bàn phấn R04 Bedroom| Gợi ý mã cổng thoát hiểm| Tương tác (E) | Cung cấp mã 4072 |
| KEY_RITUAL | KeyItem | Sau tranh tường R04 | Cửa phòng nghi lễ R05| Nhặt (E) | Cấp quyền mở LOCK_DOOR_RITUAL |
| LOCK_DOOR_RITUAL | Lockable (Key) | Cửa R02 -> R05 | Mở cửa vào R05 Ritual | Cần KEY_RITUAL | Lối vào R05 Ritual Room |
| LOCK_EXIT_MIRROR | Lockable (Code)| Bệ gương lớn R05 | Kích hoạt cổng Exit | Mã số 4072 | **THẮNG GAME (GameEvents.OnGameWin)** |

---

### 6. LỘ TRÌNH TUẦN TRA CỦA AI (AI PATROL ROUTE)
* **AI Spawn Point**: Xuất hiện tại trung tâm R02 Central Hall.
* **5 Điểm Waypoint liên tục**:
  * AI_WP_01 (x: 0, z: 2) — Trung tâm Đại sảnh R02.
  * AI_WP_02 (x: 7, z: 2) — Cửa Phòng ngủ R04.
  * AI_WP_03 (x: 7, z: -6) — Góc Kho tế lễ R06.
  * AI_WP_04 (x: -3, z: -6) — Cửa Tiền sảnh R01.
  * AI_WP_05 (x: -7, z: 2) — Cửa Phòng đọc sách R03.
* Lộ trình di chuyển theo vòng tròn kín: WP_01 -> WP_02 -> WP_03 -> WP_04 -> WP_05 -> WP_01.
