using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace InQuant.Authorization.Permissions
{
    public class Permission
    {
        public const string ClaimType = "Permission";

        public Permission() { }

        public Permission(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public Permission(string name, string description) : this(name)
        {
            Description = description;
        }

        public Permission(string name, string description, IEnumerable<Permission> impliedBy) : this(name, description)
        {
            ImpliedBy = impliedBy;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public IEnumerable<Permission> ImpliedBy { get; set; }

        public static implicit operator Claim(Permission p)
        {
            return new Claim(ClaimType, p.Name);
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                return Name == ((Permission)obj).Name;
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
