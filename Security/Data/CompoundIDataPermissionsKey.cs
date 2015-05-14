using System;

using Composite.Data;

namespace CompositeC1Contrib.Security.Data
{
    public class CompoundIDataPermissionsKey : IEquatable<CompoundIDataPermissionsKey>
    {
        public Guid DataTypeId { get; set; }
        public string DataId { get; set; }

        public CompoundIDataPermissionsKey() { }

        public CompoundIDataPermissionsKey(IData data)
        {
            DataTypeId = data.GetImmutableTypeId();
            DataId = data.GetUniqueKey().ToString();
        }

        public override int GetHashCode()
        {
            return DataTypeId.GetHashCode() ^ DataId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as CompoundIDataPermissionsKey;
            
            return other != null && Equals(other);
        }

        public bool Equals(CompoundIDataPermissionsKey other)
        {
            if (other == null)
            {
                return false;
            }

            return other.DataTypeId == DataTypeId && other.DataId == DataId;
        }
    }
}
