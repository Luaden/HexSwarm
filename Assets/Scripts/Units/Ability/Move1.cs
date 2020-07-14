using System;
using System.Collections.Generic;
using UnityEngine;

namespace Old
{
    [System.Serializable]
    public class Move1 : Ablity
    {
        [SerializeField] protected float X;

        protected override HashSet<Vector3Int> GenerateAttack()
        {
            return new HashSet<Vector3Int>();
        }

        protected override HashSet<Vector3Int> GenerateMoves()
        {
            HashSet<Vector3Int> returnVar = new HashSet<Vector3Int>();
            GenerateHexagon(1, returnVar);
            return returnVar;
        }

        public Move1() : base() { }
    }
}

