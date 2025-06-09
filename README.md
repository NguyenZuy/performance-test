# Game Bắn Zombie

## Mô tả dự án
Game hành động bắn zombie với góc nhìn từ trên xuống, người chơi điều khiển lính chiến đấu chống lại bầy zombie tấn công từ mọi phía.

## Công nghệ sử dụng
- Custom Workspace package: Quản lý UI, singleton pattern, và các hệ thống game
- LitMotion: Tween UI hiệu quả hơn DotTween
- UniTask: Tối ưu code chạy đa luồng
- TriInspector: Nâng cao hiệu quả làm việc với Inspector

## Tính năng chính

### Camera & Điều khiển
- Sử dụng Cinemachine để theo dõi nhân vật
- Góc nhìn từ trên xuống
- Virtual Joystick để điều khiển nhân vật

### Nhân vật (Player)
- Animation Rigging để rig cầm súng
- Chia layer animation: upper body (bắn) và lower body (di chuyển)
- Hiệu ứng mất máu và tương tác

### Zombie AI
- Sử dụng NavMesh để tìm đường đến người chơi
- Hiệu ứng trúng đạn và biến mất bằng shader
- AI thông minh tấn công từ nhiều hướng

### Vũ khí & Combat
- Nhiều loại súng khác nhau với khả năng chuyển đổi
- Hiệu ứng particle cho đạn nổ

### Level Design
- Level 1: Mặt đất bằng phẳng với vật cản
- Độ khó tăng dần theo thời gian

### Tối ưu & Visual
- Scriptable Objects để cấu hình game nhanh chóng
- Hỗ trợ đa độ phân giải
- Tích hợp âm thanh và particle effects
- UI thân thiện với người dùng

