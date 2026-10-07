# C HANDOFF — BAN GIAO THIET KE & LEVEL WORLD 2
Nguoi phu trach: Role C — Level & Puzzle Designer
Scene chinh thuc: Assets/Scenes/World2_Prototype.unity

---

## 1. NHUNG GI ROLE C DA HOAN THANH (COMPLETED)
- World 2 Layout & Blockout: Dung hoan chinh 6 phong (R01_Foyer, R02_CentralHall, R03_Study, R04_Bedroom, R05_RitualRoom, R06_Storage) kem hanh lang chinh va duong tat (SHORTCUT_01).
- Player Setup: Dat Player FPS voi PlayerController + CharacterController + PlayerCamera, Tag "Player" tai diem xuat phat R01_Foyer.
- AI Waypoint & Route: Thiet lap 5 diem tuan tra (AI_WP_01 den AI_WP_05) va gan vao AIController cua quai vat tuan tra quanh nha.
- He thong Puzzle Chain hoan chinh:
  * CLUE_01_NOTE tai Foyer -> dan den KEY_STUDY tai Storage.
  * LOCK_DOOR_STUDY mo bang KEY_STUDY -> vao Thu vien.
  * CLUE_02_BOOK goi y ma 8991 -> mo ket sat LOCK_SAFE_STUDY -> lay KEY_BEDROOM.
  * LOCK_DOOR_BEDROOM mo bang KEY_BEDROOM -> vao Phong ngu.
  * CLUE_03_DIARY cung cap ma 4072 + KEY_RITUAL sau tranh rach -> mo LOCK_DOOR_RITUAL.
  * LOCK_EXIT_MIRROR mo bang ma 4072 tai phong Nghi le -> Kich hoat Win.
- Bo Prefabs & Materials: Da tao cac Prefab mau chuan ID trong Assets/Prefabs/Interactables/ va bang mau URP trong Assets/Materials/.
- Tai lieu ban giao: Day du 3 file doc trong Assets/Documentation/.

---

## 2. HANG MUC DANH CHO THANH VIEN B (3D / ENVIRONMENT ARTIST)
1. Thay the Mesh Placeholder:
   - Thay cac khoi blockout tuong/san/cua bang 3D Model chi tiet (Victorian Mansion).
   - Thay cac khoi ban/ghe/ket sat bang Model noi that tuong ung.
   - Thay the model Guong Cong Lon tai R05_RitualRoom.
2. Luu y ky thuat khi thay model:
   - Giu nguyen cac component Lockable, KeyItem, CodeClue da duoc cau hinh san tren cac GameObject.
   - Dam bao cua mo co be rong toi thieu 1.5m de AI va Player di chuyen khong bi ket.
3. Bake lai NavMesh: Sau khi thay doi vat the Static, mo tab Navigation va bam Bake.

---

## 3. HANG MUC DANH CHO THANH VIEN A (CORE PROGRAMMING)
1. Kiem tra tuong tac Code Lock UI:
   - Hien tai phuong thuc Lockable.TryUnlockWithCode(string) da san sang. Khi Role D tao xong ban phim ao (Numpad UI), A chi can ket noi InputField gui ma so vao method nay.
2. Win Condition Event:
   - LOCK_EXIT_MIRROR khi mo thanh cong da tu dong goi GameEvents.TriggerLockOpened("LOCK_EXIT_MIRROR") va GameEvents.TriggerGameWin().

---

## 4. HANG MUC DANH CHO THANH VIEN D/E (UI / AUDIO / QA)
1. UI Subscriptions:
   - Subscribe GameEvents.OnCluePickup(clueId, clueText) de hien bang popup doc noi dung note/nhat ky.
   - Subscribe GameEvents.OnKeyPickup(keyId) de cap nhat icon chia khoa vao HUD.
   - Subscribe GameEvents.OnLockOpened(lockId) de phat am thanh tieng mo khoa cot ket.
   - Subscribe GameEvents.OnGameWin de hien man hinh VICTORY / CUTSCENE ket thuc.
2. Audio / SFX:
   - Dat nguon am thanh tieng gio hu, tieng nen no lach tach, tieng buoc chan va tieng tho gam cua Quai vat.

---

## 5. KET QUA KIEM TRA (TESTING SUMMARY)
- Mo Scene Assets/Scenes/World2_Prototype.unity tron tru, khong co loi script do.
- Player di chuyen muot ma bang WASD/Mouse (Editor) va nhan dien dung cac vung va cham (Colliders).
- Quai vat AI di chuyen tuan tra theo 5 waypoints khong bi lech route.
- Chuoi nhat chia khoa va mo cua theo dung thu tu logic, khong the pha vo trinh tu cot truyen.