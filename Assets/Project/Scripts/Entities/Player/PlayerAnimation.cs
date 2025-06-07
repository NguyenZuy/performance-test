using UnityEngine;
using UnityEngine.Animations.Rigging;
using LitMotion;
using LitMotion.Extensions;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Rig _rigLayerGunShoot;
        [SerializeField] private float _blendSpeed = 5f; // Speed of transition between states
        [SerializeField] private float _shootTransitionDuration = 0.2f; // Duration for gun shoot transition

        private static readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
        private float _currentMoveSpeed;
        private MotionHandle _shootMotionHandle;

        public void SetMotion(Vector3 moveDirection)
        {
            float targetSpeed = moveDirection.magnitude;
            _currentMoveSpeed = Mathf.Lerp(_currentMoveSpeed, targetSpeed, Time.deltaTime * _blendSpeed);
            _animator.SetFloat(_moveSpeedHash, _currentMoveSpeed);
        }

        public void SetGunShoot(bool isShoot)
        {
            // Cancel any existing motion
            if (_shootMotionHandle != null)
                _shootMotionHandle.TryCancel();

            // Create new motion for smooth transition
            _shootMotionHandle = LMotion.Create(_rigLayerGunShoot.weight, isShoot ? 1f : 0f, _shootTransitionDuration)
                .WithEase(Ease.OutQuad)
                .Bind((weight) =>
                {
                    _rigLayerGunShoot.weight = weight;
                })
                .AddTo(gameObject);
        }
    }
}
