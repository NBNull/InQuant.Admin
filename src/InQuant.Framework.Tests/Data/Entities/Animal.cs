using System;
using InQuant.Framework.Data.Core;

namespace InQuant.Framework.Tests.Data.Entities
{
    public class Animal : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
