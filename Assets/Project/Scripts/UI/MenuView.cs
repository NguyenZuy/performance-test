using ZuyZuy.PT.Constants;
using ZuyZuy.PT.Manager;
using ZuyZuy.Workspace;

namespace ZuyZuy.PT.UI
{
    public class MenuView : UIView
    {
        protected override void Init()
        {
            m_viewName = UIViewName.Menu.ToString();
        }

        public void OnPlayClick()
        {
            GameManager.Instance.LaunchLevel(1);
        }
    }
}
