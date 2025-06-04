using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ZuyZuy.PT.Entities.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Rig _rigLayerGunShoot;

        private static readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");

        public void SetMotion(Vector3 moveDirection)
        {
            _animator.SetFloat(_moveSpeedHash, moveDirection.magnitude);
        }

        public void SetGunShoot(bool isShoot)
        {
            _rigLayerGunShoot.weight = isShoot ? 1 : 0;
        }
    }
}
