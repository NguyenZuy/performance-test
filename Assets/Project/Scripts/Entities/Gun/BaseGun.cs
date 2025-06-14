using TriInspector;
using UnityEngine;
using ZuyZuy.PT.Manager;

namespace ZuyZuy.PT.Entities.Gun
{
    public abstract class BaseGun : MonoBehaviour
    {
        [Title("Gun Settings")]
        [SerializeField] protected int m_Damage;
        [SerializeField] protected float m_FireRate;
        [SerializeField] protected ParticleSystem m_MuzzleFlash;
        [SerializeField] protected GameObject m_HitEffectPrefab;
        [SerializeField] protected Transform m_LeftHandIKTarget;
        [SerializeField] protected Transform m_RightHandIKTarget;
        [SerializeField] protected AudioClip m_ShootSound;

        [Title("Recoil Settings")]
        [SerializeField] protected float m_RecoilForce = 2f;
        [SerializeField] protected float m_RecoilRecoverySpeed = 5f;
        [SerializeField] protected float m_MaxRecoilAngle = 30f;
        [SerializeField] protected Transform m_GunModel;

        protected float m_NextFireTime;
        protected float m_CurrentRecoil;
        protected Vector3 m_OriginalRotation;

        public Transform LeftHandIKTarget => m_LeftHandIKTarget;
        public Transform RightHandIKTarget => m_RightHandIKTarget;

        protected virtual void Start()
        {
            if (m_GunModel != null)
            {
                m_OriginalRotation = m_GunModel.localEulerAngles;
            }
        }

        protected virtual void Update()
        {
            if (m_GunModel != null)
            {
                // Recover from recoil
                m_CurrentRecoil = Mathf.Lerp(m_CurrentRecoil, 0f, Time.deltaTime * m_RecoilRecoverySpeed);
                ApplyRecoil();
            }
        }

        protected virtual void ApplyRecoil()
        {
            if (m_GunModel != null)
            {
                // Apply recoil rotation
                float recoilX = m_OriginalRotation.x - m_CurrentRecoil;
                m_GunModel.localEulerAngles = new Vector3(recoilX, m_OriginalRotation.y, m_OriginalRotation.z);
            }
        }

        protected virtual void AddRecoil()
        {
            m_CurrentRecoil = Mathf.Min(m_CurrentRecoil + m_RecoilForce, m_MaxRecoilAngle);
        }

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
            AddRecoil();
            if (m_ShootSound != null)
            {
                GameManager.Instance.PlaySFX(m_ShootSound);
            }
            AffectTarget();
        }
    }
}
