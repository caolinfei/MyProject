using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Core.Data
{
    public abstract class BaseQuery
    {
        public long? Id { get; set; }
    }

    public class BaseQuery<TEntity> : BaseQuery where TEntity:BaseEntity
    {

    }
}
