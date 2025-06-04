using TriInspector;
using UnityEngine;

namespace ZuyZuy.PT.Entities.Gun
{
    public abstract class BaseGun : MonoBehaviour
    {
        [Title("Gun Settings")]
        [SerializeField] protected int m_Damage;
        [SerializeField] protected float m_FireRate;
        [SerializeField] protected float m_Range;

        protected float m_NextFireTime;

        public void TryShoot()
        {
            if (Time.time >= m_NextFireTime)
            {
                Shoot();
                m_NextFireTime = Time.time + (1f / m_FireRate);
            }
        }

        protected abstract void Shoot();
    }
}
