using UnityEngine;
using System.Collections.Generic;

namespace Old
{
    public class AbilityList : MonoBehaviour
    {
        private static AbilityList _Instance;
        private static AbilityList Instance
        {
            get
            {
                if (_Instance == default)
                    _Instance = new AbilityList();
                return _Instance;
            }
        }

        protected void Awake()
        {
            // if the singleton hasn't been initialized yet
            if (_Instance != null && _Instance != this)
            {
                Destroy(this);
                return;//Avoid doing anything else
            }

            _Instance = this;
            DontDestroyOnLoad(this.gameObject);
            lambskin = new Move1();
        }

        [SerializeField] protected Move1 lambskin;
        public static Move1 MoveOne => Instance.lambskin;
    }
}
