using ZuyZuy.PT.Constants;
using ZuyZuy.Workspace;
using ZuyZuy.PT.Entities.Player;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;
using ZuyZuy.PT.Manager;

namespace ZuyZuy.PT.UI
{
    public class MainView : UIView
    {
        [Title("Main View")]
        [SerializeField] private Button[] _btnSwitchGun;
        [SerializeField] private float _activeButtonScale = 1.2f;
        [SerializeField] private float _animationDuration = 0.3f;
        [SerializeField] private Slider _playerHPSlider;

        private MotionHandle[] _scaleHandles;

        protected override void Init()
        {
            m_ViewName = UIViewName.Main.ToString();
            _scaleHandles = new MotionHandle[_btnSwitchGun.Length];
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerHPChanged += HandlePlayerHPChanged;
                GameManager.Instance.OnLevelStart += SetPlayerHP;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerHPChanged -= HandlePlayerHPChanged;
                GameManager.Instance.OnLevelStart -= SetPlayerHP;
            }
        }

        void SetPlayerHP()
        {
            _playerHPSlider.maxValue = GameManager.Instance.MaxPlayerHP;
            _playerHPSlider.value = GameManager.Instance.CurPlayerHP;
        }

        private void HandlePlayerHPChanged(int currentHP)
        {
            if (_playerHPSlider != null)
            {
                _playerHPSlider.value = (float)currentHP / GameManager.Instance.MaxPlayerHP;
            }
        }

        public void ChangeGun(int index)
        {
            PlayerController.Instance.SwitchGun(index);
            UpdateGunButtonsInteractability(index);
        }

        private void UpdateGunButtonsInteractability(int activeGunIndex)
        {
            if (_btnSwitchGun == null) return;

            for (int i = 0; i < _btnSwitchGun.Length; i++)
            {
                if (_btnSwitchGun[i] != null)
                {
                    _btnSwitchGun[i].interactable = i != activeGunIndex;

                    // Cancel previous animation if exists
                    if (_scaleHandles[i].IsActive())
                    {
                        _scaleHandles[i].Cancel();
                    }

                    // Animate scale
                    float targetScale = i == activeGunIndex ? _activeButtonScale : 1f;
                    _scaleHandles[i] = LMotion.Create(_btnSwitchGun[i].transform.localScale, new Vector3(targetScale, targetScale, targetScale), _animationDuration)
                        .WithEase(Ease.OutBack)
                        .BindToLocalScale(_btnSwitchGun[i].transform);
                }
            }
        }

        protected void OnDestroy()
        {
            // Cancel all animations when view is destroyed
            if (_scaleHandles != null)
            {
                foreach (var handle in _scaleHandles)
                {
                    if (handle.IsActive())
                    {
                        handle.TryCancel();
                    }
                }
            }
        }
    }
}
