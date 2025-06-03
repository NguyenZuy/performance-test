using ZuyZuy.Workspace;
using ZuyZuy.PT.Constants;

namespace ZuyZuy.PT.Manager
{
    public partial class GameManager
    {
        private UIPopupController _popupController;
        private UIDialogController _dialogController;
        private LoadingController _loadingController;

        void InitializeUI()
        {
            _popupController = FindFirstObjectByType<UIPopupController>();
            _dialogController = FindFirstObjectByType<UIDialogController>();
            _loadingController = FindFirstObjectByType<LoadingController>();
        }

        #region Popup Methods
        public UIPopup ShowPopup(UIPopupName popupName)
        {
            if (_popupController != null)
            {
                return _popupController.ShowPopup(popupName.ToString());
            }
            return null;
        }

        public void HidePopup(UIPopupName popupName)
        {
            if (_popupController != null)
            {
                _popupController.HidePopup(popupName.ToString());
            }
        }

        public void HideAllPopups()
        {
            if (_popupController != null)
            {
                _popupController.HideAllPopups();
            }
        }

        public bool IsPopupActive(UIPopupName popupName)
        {
            return _popupController != null && _popupController.IsPopupActive(popupName.ToString());
        }

        public UIPopup GetCurrentPopup()
        {
            return _popupController != null ? _popupController.GetCurrentPopup() : null;
        }
        #endregion

        #region Dialog Methods
        public void ShowAlert(string title, string message, System.Action onConfirm = null, string confirmText = "OK")
        {
            if (_dialogController != null)
            {
                _dialogController.ShowAlert(title, message, onConfirm, confirmText);
            }
        }

        public void ShowConfirm(string title, string message, System.Action onConfirm = null, System.Action onCancel = null,
            string confirmText = "OK", string cancelText = "Cancel")
        {
            if (_dialogController != null)
            {
                _dialogController.ShowConfirm(title, message, onConfirm, onCancel, confirmText, cancelText);
            }
        }

        public void ShowInput(string title, string message, System.Action<string> onConfirm = null, System.Action onCancel = null,
            string confirmText = "OK", string cancelText = "Cancel")
        {
            if (_dialogController != null)
            {
                _dialogController.ShowInput(title, message, onConfirm, onCancel, confirmText, cancelText);
            }
        }
        #endregion

        #region Loading Methods
        public void ShowLoadingPage(string loadingText = "Loading...", System.Action onLoadSuccess = null)
        {
            if (_loadingController != null)
            {
                _loadingController.ShowLoadingPage(loadingText, onLoadSuccess);
            }
        }

        public void ShowLoadingCircle(string loadingText = "Loading...")
        {
            if (_loadingController != null)
            {
                _loadingController.ShowLoadingCircle(loadingText);
            }
        }

        public void HideLoading()
        {
            if (_loadingController != null)
            {
                _loadingController.HideAll();
            }
        }
        #endregion
    }
}