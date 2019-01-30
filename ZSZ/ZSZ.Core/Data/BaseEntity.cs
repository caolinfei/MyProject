using System;

namespace ZSZ.Core.Data
{
    [Serializable]
    public abstract class BaseEntity
    {
        public long? Id { get; set; }
    }
}
