using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Rig _rigLayerGunShoot;
        [SerializeField] private float _blendSpeed = 5f; // Speed of transition between states

        private static readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
        private float _currentMoveSpeed;

        public void SetMotion(Vector3 moveDirection)
        {
            float targetSpeed = moveDirection.magnitude;
            _currentMoveSpeed = Mathf.Lerp(_currentMoveSpeed, targetSpeed, Time.deltaTime * _blendSpeed);
            _animator.SetFloat(_moveSpeedHash, _currentMoveSpeed);
        }

        public void SetGunShoot(bool isShoot)
        {
            _rigLayerGunShoot.weight = isShoot ? 1 : 0;
        }
    }
}
