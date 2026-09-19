// AIState.cs
// Enum định nghĩa các trạng thái của AI
// Role A - Sprint 1

public enum AIState
{
    Patrol,   // Tuần tra theo waypoints
    Chase,    // Đuổi theo player khi phát hiện
    Catch     // Bắt được player (game over)
}
