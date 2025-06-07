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
        [SerializeField] protected ParticleSystem m_MuzzleFlash;

        protected float m_NextFireTime;

        public void TryShoot()
        {
            if (Time.time >= m_NextFireTime)
            {
                Shoot();
                m_NextFireTime = Time.time + (1f / m_FireRate);
            }
        }

        protected void PlayMuzzleFlash()
        {
            m_MuzzleFlash.Play();
        }

        protected abstract void AffectTarget();

        protected virtual void Shoot()
        {
            PlayMuzzleFlash();
            AffectTarget();
        }
    }
}
