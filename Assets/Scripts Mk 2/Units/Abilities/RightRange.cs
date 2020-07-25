using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RightRange : IRange<Vector3Int>
{
    protected List<Vector3Int> myRanges;
    public bool Rotates => true;

    public RightRange(int min, int max, bool fill = false)
    {
        myRanges = new List<Vector3Int>();
        for (int i = min; i <= max; i++)
        {
            myRanges.Add(new Vector3Int(i, 0, -i));
            if (fill)
                for(int delta = max-i; delta > 0; delta--)
                    myRanges.Add(new Vector3Int(i, delta, -i - delta));
        }
    }


    public IEnumerator<Vector3Int> GetEnumerator()
    {
        return myRanges.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return myRanges.GetEnumerator();
    }
}
