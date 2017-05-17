using System;

using Composite.Data;

namespace CompositeC1Contrib.Security.Data
{
    public class CompoundIDataPermissionsKey : IEquatable<CompoundIDataPermissionsKey>
    {
        public Guid DataTypeId { get; }
        public string DataId { get; }

        public CompoundIDataPermissionsKey(IData data) : this(data.GetImmutableTypeId(), data.GetUniqueKey().ToString()) { }

        public CompoundIDataPermissionsKey(Guid dataTypeId, string dataId)
        {
            DataTypeId = dataTypeId;
            DataId = dataId;
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
