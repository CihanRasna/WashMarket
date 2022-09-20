using System;
using UnityEngine;

namespace RSNManagers
{

    [Serializable]
    public abstract class RsnObject : IEquatable<RsnObject>
    {
        
        [SerializeField, HideInInspector] protected string _id = Guid.NewGuid().ToString();
        public virtual string id => _id;


        #region Equality

        protected virtual string GetUniqueId()
        {
            return id;
        }

        public override bool Equals(object other)
        {
            return Equals(other as RsnObject);
        }

        public bool Equals(RsnObject other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return GetUniqueId() == other.GetUniqueId();
        }

        public override int GetHashCode()
        {
            return GetUniqueId().GetHashCode();
        }

        public static bool operator ==(RsnObject lhs, RsnObject rhs)
        {
            if (lhs is null)
            {
                return rhs is null;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(RsnObject lhs, RsnObject rhs)
        {
            return !(lhs == rhs);
        }
        
    #endregion
    
    }

}