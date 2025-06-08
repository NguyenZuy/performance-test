using UnityEngine;
using ZuyZuy.PT.UI;
using ZuyZuy.PT.Constants;
using System;

namespace ZuyZuy.PT.Manager
{
    public partial class GameManager
    {
        private int _curPlayerHP;
        private int _maxPlayerHP = 100;

        public Action<int> OnPlayerHPChanged;

        public void InitializePlayerHP()
        {
            _curPlayerHP = _maxPlayerHP;
            OnPlayerHPChanged?.Invoke(_curPlayerHP);
        }

        public void TakeDamage(int damage)
        {
            _curPlayerHP = Mathf.Max(0, _curPlayerHP - damage);
            OnPlayerHPChanged?.Invoke(_curPlayerHP);

            if (_curPlayerHP <= 0)
            {
                // Handle player death
                Debug.Log("Player died!");
            }
        }
    }
}
