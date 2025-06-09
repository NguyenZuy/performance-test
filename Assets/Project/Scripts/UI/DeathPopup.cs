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

        public void TapClick()
        {
            GameManager.Instance.ShowPopup(UIPopupName.MenuPopup);
        }
    }
}