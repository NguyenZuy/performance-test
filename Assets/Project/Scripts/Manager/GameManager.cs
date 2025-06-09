using ZuyZuy.PT.Constants;
using ZuyZuy.Workspace;

namespace ZuyZuy.PT.Manager
{
    public partial class GameManager : BaseSingleton<GameManager>
    {
        void Start()
        {
            InitializeUI();
            InitializePlayerHP();
            InitializeCamera();
            //LaunchLevel(1);


            ShowPopup(UIPopupName.MenuPopup);
        }
    }
}
