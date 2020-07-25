using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IScriptableCollection<T>
{
    void SetInfo(IEnumerable<T> collection);
}
