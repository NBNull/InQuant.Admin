using System;
using System.Collections.Generic;
using InQuant.Framework.Data.Core;
using InQuant.Framework.Data.Core.Attributes;

namespace InQuant.Framework.Tests.Data.Entities
{
    public class Person : IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Active { get; set; }
        public int? ProfileId { get; set; }

        [Ignore]
        public IEnumerable<Phone> Phones { get; private set; }
    }
}