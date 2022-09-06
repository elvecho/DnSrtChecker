using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnSrtChecker.Persistence
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
        void UpdateAsync(Object obj);
        void UpdateRangeAsync(IEnumerable<Object> obj);

    }
}
