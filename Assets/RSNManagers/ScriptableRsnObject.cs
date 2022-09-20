using System;
using UnityEngine;

namespace RSNManagers
{
    public class ScriptableRsnObject : ScriptableObject
    {
        [SerializeField, HideInInspector] private string _id = Guid.NewGuid().ToString();
        public string id => _id;

        #region Equality

        public override bool Equals(object other)
        {
            if (base.Equals(other))
            {
                return true;
            }

            var otherSO = other as ScriptableRsnObject;
            if (otherSO == null)
            {
                return false;
            }

            return _id == otherSO._id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(ScriptableRsnObject lhs, ScriptableRsnObject rhs)
        {
            if (lhs is null)
            {
                return rhs is null;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(ScriptableRsnObject lhs, ScriptableRsnObject rhs)
        {
            return !(lhs == rhs);
        }

        #endregion
    }
}