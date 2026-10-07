# WORLD 2: ASSET REQUIREMENTS & INTEGRATION LIST

Bảng tổng hợp toàn bộ Model, Prefab, Material phục vụ cho World 2 (Nhà Gương):

| ID | Asset Name | Purpose | Source / Type | License | Status in Project |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **ENV_FLOOR** | Mansion Wood Floor | Mặt sàn cho 6 phòng và hành lang | Unity Primitive / Custom Mat | CC0 / Built-in | **Ready (Blockout + Material)** |
| **ENV_WALL** | Mansion Dark Walls | Tường bao quanh và vách ngăn phòng | Unity Primitive / Custom Mat | CC0 / Built-in | **Ready (Blockout + Material)** |
| **ENV_DOOR_01** | Interior Wood Door | Cửa ngăn cách các phòng | Unity Mesh / Prefab | CC0 / Built-in | **Ready (Prefab & Lockable)** |
| **PROP_MIRROR_01**| Grand Gateway Mirror | Cổng thoát hiểm chính tại R05 | Custom Prefab + Glass Mat | CC0 / Built-in | **Ready (Lockable Code)** |
| **PROP_MIRROR_02**| Wall Mirrors (Decor)| Gương treo tường trang trí | Custom Mesh + Glass Mat | CC0 / Built-in | **Ready (Placed in Hall)** |
| **PROP_SAFE_01** | Heavy Iron Safe | Két sắt chứa chìa khóa phòng ngủ | Custom Box Mesh + Lockable | CC0 / Built-in | **Ready (Lockable Code: 8991)** |
| **PROP_KEY_01** | Brass Key (Study) | Chìa mở phòng đọc sách | KeyItem Prefab + Gold Mat | CC0 / Built-in | **Ready (KEY_STUDY)** |
| **PROP_KEY_02** | Silver Key (Bedroom)| Chìa mở phòng ngủ | KeyItem Prefab + Silver Mat| CC0 / Built-in | **Ready (KEY_BEDROOM)** |
| **PROP_KEY_03** | Ritual Key | Chìa mở phòng nghi lễ | KeyItem Prefab + Gold Mat | CC0 / Built-in | **Ready (KEY_RITUAL)** |
| **PROP_NOTE_01** | Parchment Note 01 | Manh mối mở đầu tại Foyer | CodeClue Prefab + Paper Mat| CC0 / Built-in | **Ready (CLUE_01_NOTE)** |
| **PROP_NOTE_02** | Study Book Note | Gợi ý năm sinh đảo ngược | CodeClue Prefab + Paper Mat| CC0 / Built-in | **Ready (CLUE_02_BOOK)** |
| **PROP_NOTE_03** | Ritual Diary | Ghi chép mã tế lễ 4072 | CodeClue Prefab + Paper Mat| CC0 / Built-in | **Ready (CLUE_03_DIARY)** |
| **PROP_ALTAR** | Ritual Stone Altar | Bệ đá tế lễ trước gương cổng | Custom Mesh + Stone Mat | CC0 / Built-in | **Ready (Placed in R05)** |
| **AI_DEMON** | Mirror Demon (Quái) | Kẻ thù tuần tra săn lùng Player | NavMeshAgent + AIController | CC0 / Built-in | **Ready (5 Patrol Waypoints)** |
| **LIGHT_CANDLE** | Candle Point Light | Đèn nến tạo không khí kinh dị | Unity Point Light (Warm) | Built-in | **Ready (Configured)** |

---

### Gợi ý cho thành viên B (Khi thay thế 3D Model chi tiết):
1. **Victorian Interior Assets**: Có thể lấy gói  Victorian Interior - Free Sample trên Unity Asset Store.
2. **Horror Props**: Lấy trên Unity Asset Store (Keywords: Horror Props Pack Free, Old Furniture Free).
3. **Quái vật AI**: Thay thế Mesh của AI_Demon bằng model Humanoid rigged (Zombie / Ghost / Demon) và gán Animator.
