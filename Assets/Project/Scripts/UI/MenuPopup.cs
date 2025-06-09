using UnityEngine;
using ZuyZuy.PT.Constants;
using ZuyZuy.PT.Manager;
using ZuyZuy.Workspace;

namespace ZuyZuy.PT.UI
{
    public class MenuPopup : UIPopup
    {
        protected override void Init()
        {
            m_PopupName = UIPopupName.MenuPopup.ToString();
        }

        public void OnPlayClick()
        {
            GameManager.Instance.LaunchLevel(1);
            GameManager.Instance.HidePopup(UIPopupName.MenuPopup);
            GameManager.Instance.ShowView(UIViewName.Main);
        }

        public void OnQuitClick()
        {
            Application.Quit();
        }
    }
}