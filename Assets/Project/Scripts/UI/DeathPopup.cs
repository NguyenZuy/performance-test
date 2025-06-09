using UnityEngine.SceneManagement;
using ZuyZuy.PT.Constants;
using ZuyZuy.PT.Manager;
using ZuyZuy.Workspace;

namespace ZuyZuy.PT.UI
{
    public class DeathPopup : UIPopup
    {
        protected override void Init()
        {
            m_PopupName = UIPopupName.DeathPopup.ToString();
        }

        public void OnRestartClick()
        {
            //GameManager.Instance.RestartGame();
            GameManager.Instance.HidePopup(UIPopupName.DeathPopup);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnMenuClick()
        {
            GameManager.Instance.ShowPopup(UIPopupName.MenuPopup);
            GameManager.Instance.HidePopup(UIPopupName.DeathPopup);
        }
    }
}