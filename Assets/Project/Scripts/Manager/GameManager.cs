using ZuyZuy.Workspace;

namespace ZuyZuy.PT.Manager
{
    public partial class GameManager : BaseSingleton<GameManager>
    {
        void Start()
        {
            InitializeUI();
            LaunchLevel(1);

        }
    }
}
