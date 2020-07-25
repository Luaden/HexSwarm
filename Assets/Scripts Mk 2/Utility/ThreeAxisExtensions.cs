using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ThreeAxisExtensions
{
    public static class ThreeAxisExtensions
    {
        public static IReadOnlyDictionary<int, Quaternion> Transforms
            = new Dictionary<int, Quaternion>()
         {
                    {(int)Direction.Sixty,       Quaternion.AngleAxis(60,new Vector3(0.6f,0.6f,0.6f))},
                    {(int)Direction.One20,       Quaternion.AngleAxis(120,new Vector3(0.6f,0.6f,0.6f))},
                    {(int)Direction.One80,       Quaternion.AngleAxis(180,new Vector3(0.6f,0.6f,0.6f))},
                    {(int)Direction.Two40,       Quaternion.AngleAxis(240,new Vector3(0.6f,0.6f,0.6f))},
                    {(int)Direction.Threehundred,Quaternion.AngleAxis(300,new Vector3(0.6f,0.6f,0.6f))},
         };

        public static IEnumerable<Vector3Int> Rotate(this IRange<Vector3Int> range, Direction direction)
        {
            if (!range.Rotates || direction == Direction.Zero)
                return range;

            Quaternion transform;
            if (Transforms.TryGetValue((int)direction, out transform))
                return range.Select(X => Vector3Int.RoundToInt(transform * X));
            return range;
        }

    }
}
