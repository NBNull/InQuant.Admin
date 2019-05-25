﻿using InQuant.Framework.Data.Core.Configuration;
using InQuant.Framework.Data.Core.Enums;
using InQuant.Framework.Data.Core.Mapper;
using InQuant.Framework.Data.Core.Predicates;
using InQuant.Framework.Data.Core.Sql;
using InQuant.Framework.Data.Predicates;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using Xunit;

namespace InQuant.Framework.Tests.Data
{
    public class PredicatesTest
    {
        public abstract class PredicatesFixtureBase
        {
            protected Mock<ISqlDialect> @SqlDialect;
            protected Mock<ISqlGenerator> Generator;
            protected Mock<IDapperConfiguration> Configuration;

            public PredicatesFixtureBase()
            {
                @SqlDialect = new Mock<ISqlDialect>();
                Generator = new Mock<ISqlGenerator>();
                Configuration = new Mock<IDapperConfiguration>();

                @SqlDialect.SetupGet(c => c.ParameterPrefix).Returns('@');
                Configuration.SetupGet(c => c.Dialect).Returns(@SqlDialect.Object);
                Generator.SetupGet(c => c.Configuration).Returns(Configuration.Object);
            }
        }

        public class PredicatesTests : PredicatesFixtureBase
        {
            [Fact]
            public void Field_ReturnsSetupPredicate()
            {
                var predicate = Predicates.Field<PredicateTestEntity>(f => f.Name, Operator.Like, "Lead", true);
                Assert.Equal("Name", predicate.PropertyName);
                Assert.Equal(Operator.Like, predicate.Operator);
                Assert.Equal("Lead", predicate.Value);
                Assert.True(predicate.Not);
            }

            [Fact]
            public void Property_ReturnsSetupPredicate()
            {
                var predicate = Predicates.Property<PredicateTestEntity, PredicateTestEntity2>(f => f.Name, Operator.Le, f => f.Value, true);
                Assert.Equal("Name", predicate.PropertyName);
                Assert.Equal(Operator.Le, predicate.Operator);
                Assert.Equal("Value", predicate.PropertyName2);
                Assert.True(predicate.Not);
            }

            [Fact]
            public void Group_ReturnsSetupPredicate()
            {
                Mock<IPredicate> subPredicate = new Mock<IPredicate>();
                var predicate = Predicates.Group(GroupOperator.Or, subPredicate.Object);
                Assert.Equal(GroupOperator.Or, predicate.Operator);
                Assert.Equal(1, predicate.Predicates.Count);
                Assert.Equal(subPredicate.Object, predicate.Predicates[0]);
            }

            [Fact]
            public void Exists_ReturnsSetupPredicate()
            {
                Mock<IPredicate> subPredicate = new Mock<IPredicate>();
                var predicate = Predicates.Exists<PredicateTestEntity2>(subPredicate.Object, true);
                Assert.Equal(subPredicate.Object, predicate.Predicate);
                Assert.True(predicate.Not);
            }

            [Fact]
            public void Between_ReturnsSetupPredicate()
            {
                BetweenValues values = new BetweenValues();
                var predicate = Predicates.Between<PredicateTestEntity>(f => f.Name, values, true);
                Assert.Equal("Name", predicate.PropertyName);
                Assert.Equal(values, predicate.Value);
                Assert.True(predicate.Not);
            }

            [Fact]
            public void Sort__ReturnsSetupPredicate()
            {
                var predicate = Predicates.Sort<PredicateTestEntity>(f => f.Name, false);
                Assert.Equal("Name", predicate.PropertyName);
                Assert.False(predicate.Ascending);
            }
        }

        public class BasePredicateTests : PredicatesFixtureBase
        {
            [Fact]
            public void GetColumnName_WhenMapNotFound_ThrowsException()
            {
                Mock<BasePredicate> predicate = new Mock<BasePredicate>();
                predicate.CallBase = true;
                Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity))).Returns(() => null).Verifiable();

                var ex = Assert.Throws<NullReferenceException>(() => predicate.Object.TestProtected().RunMethod<string>("GetColumnName", typeof(PredicateTestEntity), Generator.Object, "Name"));

                Configuration.Verify();

                Assert.StartsWith("Map was not found", ex.Message);
            }

            [Fact]
            public void GetColumnName_WhenPropertyNotFound_ThrowsException()
            {
                Mock<IClassMapper> classMapper = new Mock<IClassMapper>();
                Mock<BasePredicate> predicate = new Mock<BasePredicate>();
                List<IPropertyMap> propertyMaps = new List<IPropertyMap>();
                predicate.CallBase = true;

                Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity))).Returns(classMapper.Object).Verifiable();
                classMapper.SetupGet(c => c.Properties).Returns(propertyMaps).Verifiable();

                var ex = Assert.Throws<NullReferenceException>(() => predicate.Object.TestProtected().RunMethod<string>("GetColumnName", typeof(PredicateTestEntity), Generator.Object, "Name"));

                Configuration.Verify();
                classMapper.Verify();

                Assert.StartsWith("Name was not found", ex.Message);
            }

            [Fact]
            public void GetColumnName_GetsColumnName()
            {
                Mock<IClassMapper> classMapper = new Mock<IClassMapper>();
                Mock<BasePredicate> predicate = new Mock<BasePredicate>();
                Mock<IPropertyMap> propertyMap = new Mock<IPropertyMap>();
                List<IPropertyMap> propertyMaps = new List<IPropertyMap> { propertyMap.Object };
                predicate.CallBase = true;

                Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity))).Returns(classMapper.Object).Verifiable();
                classMapper.SetupGet(c => c.Properties).Returns(propertyMaps).Verifiable();
                propertyMap.SetupGet(p => p.Name).Returns("Name").Verifiable();
                Generator.Setup(g => g.GetColumnName(classMapper.Object, propertyMap.Object, false)).Returns("foo").Verifiable();

                var result = predicate.Object.TestProtected().RunMethod<string>("GetColumnName", typeof(PredicateTestEntity), Generator.Object, "Name");

                Configuration.Verify();
                classMapper.Verify();
                propertyMap.Verify();
                Generator.Verify();

                Assert.Equal("foo", result);
            }
        }

        public class ComparePredicateTests : PredicatesFixtureBase
        {
            [Fact]
            public void GetOperatorString_ReturnsOperatorStrings()
            {
                Assert.Equal("=", Setup(Operator.Eq, false).Object.GetOperatorString());
                Assert.Equal("<>", Setup(Operator.Eq, true).Object.GetOperatorString());
                Assert.Equal(">", Setup(Operator.Gt, false).Object.GetOperatorString());
                Assert.Equal("<=", Setup(Operator.Gt, true).Object.GetOperatorString());
                Assert.Equal(">=", Setup(Operator.Ge, false).Object.GetOperatorString());
                Assert.Equal("<", Setup(Operator.Ge, true).Object.GetOperatorString());
                Assert.Equal("<", Setup(Operator.Lt, false).Object.GetOperatorString());
                Assert.Equal(">=", Setup(Operator.Lt, true).Object.GetOperatorString());
                Assert.Equal("<=", Setup(Operator.Le, false).Object.GetOperatorString());
                Assert.Equal(">", Setup(Operator.Le, true).Object.GetOperatorString());
                Assert.Equal("LIKE", Setup(Operator.Like, false).Object.GetOperatorString());
                Assert.Equal("NOT LIKE", Setup(Operator.Like, true).Object.GetOperatorString());
            }

            protected Mock<ComparePredicate> Setup(Operator op, bool not)
            {
                Mock<ComparePredicate> predicate = new Mock<ComparePredicate>();
                predicate.Object.Operator = op;
                predicate.Object.Not = not;
                predicate.CallBase = true;
                return predicate;
            }
        }

        public class FieldPredicateTests : PredicatesFixtureBase
        {
            [Fact]
            public void GetSql_NullValue_ReturnsProperSql()
            {
                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, null, false);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();

                Assert.Empty(parameters);
                Assert.Equal("(fooCol IS NULL)", sql);
            }

            [Fact]
            public void GetSql_NullValue_Not_ReturnsProperSql()
            {
                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, null, true);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();

                Assert.Empty(parameters);
                Assert.Equal("(fooCol IS NOT NULL)", sql);
            }

            [Fact]
            public void GetSql_Enumerable_NotEqOperator_ReturnsProperSql()
            {
                var predicate = Setup<PredicateTestEntity>("Name", Operator.Le, new[] { "foo", "bar" }, false);
                var parameters = new Dictionary<string, object>();

                var ex = Assert.Throws<ArgumentException>(() => predicate.Object.GetSql(Generator.Object, parameters));

                predicate.Verify();

                Assert.Equal("Operator must be set to Eq for Enumerable types", ex.Message);
            }

            [Fact]
            public void GetSql_Enumerable_ReturnsProperSql()
            {
                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, new[] { "foo", "bar" }, false);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();

                Assert.Equal(2, parameters.Count);
                Assert.Equal("foo", parameters["@Name_0"]);
                Assert.Equal("bar", parameters["@Name_1"]);
                Assert.Equal("(fooCol IN (@Name_0, @Name_1))", sql);
            }

            [Fact]
            public void GetSql_Enumerable_Not_ReturnsProperSql()
            {
                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, new[] { "foo", "bar" }, true);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();

                Assert.Equal(2, parameters.Count);
                Assert.Equal("foo", parameters["@Name_0"]);
                Assert.Equal("bar", parameters["@Name_1"]);
                Assert.Equal("(fooCol NOT IN (@Name_0, @Name_1))", sql);
            }

            [Fact]
            public void GetSql_ReturnsProperSql()
            {
                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, 12, true);
                predicate.Setup(p => p.GetOperatorString()).Returns("**").Verifiable();
                var parameters = new Dictionary<string, object>();

                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();

                Assert.Single(parameters);
                Assert.Equal(12, parameters["@Name_0"]);
                Assert.Equal("(fooCol ** @Name_0)", sql);
            }

            protected Mock<FieldPredicate<T>> Setup<T>(string propertyName, Operator op, object value, bool not) where T : class
            {
                Mock<FieldPredicate<T>> predicate = new Mock<FieldPredicate<T>>();
                predicate.Object.PropertyName = propertyName;
                predicate.Object.Operator = op;
                predicate.Object.Not = not;
                predicate.Object.Value = value;
                predicate.CallBase = true;
                predicate.Protected().Setup<string>("GetColumnName", typeof(T), Generator.Object, propertyName).Returns("fooCol").Verifiable();
                return predicate;
            }
        }

        public class PropertyPredicateTests : PredicatesFixtureBase
        {
            [Fact]
            public void GetSql_ReturnsProperSql()
            {
                var predicate = Setup<PredicateTestEntity, PredicateTestEntity2>("Name", Operator.Eq, "Value", false);
                predicate.Setup(p => p.GetOperatorString()).Returns("**").Verifiable();
                var parameters = new Dictionary<string, object>();

                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();

                Assert.Empty(parameters);
                Assert.Equal("(fooCol ** fooCol2)", sql);
            }

            protected Mock<PropertyPredicate<T, T2>> Setup<T, T2>(string propertyName, Operator op, string propertyName2, bool not)
                where T : class
                where T2 : class
            {
                Mock<PropertyPredicate<T, T2>> predicate = new Mock<PropertyPredicate<T, T2>>();
                predicate.Object.PropertyName = propertyName;
                predicate.Object.PropertyName2 = propertyName2;
                predicate.Object.Operator = op;
                predicate.Object.Not = not;
                predicate.CallBase = true;
                predicate.Protected().Setup<string>("GetColumnName", typeof(T), Generator.Object, propertyName).Returns("fooCol").Verifiable();
                predicate.Protected().Setup<string>("GetColumnName", typeof(T2), Generator.Object, propertyName2).Returns("fooCol2").Verifiable();
                return predicate;
            }
        }

        public class BetweenPredicateTests : PredicatesFixtureBase
        {
            [Fact]
            public void GetSql_ReturnsProperSql()
            {
                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, 12, 20, false);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();

                Assert.Equal(2, parameters.Count);
                Assert.Equal(12, parameters["@Name_0"]);
                Assert.Equal(20, parameters["@Name_1"]);
                Assert.Equal("(fooCol BETWEEN @Name_0 AND @Name_1)", sql);
            }

            [Fact]
            public void GetSql_Not_ReturnsProperSql()
            {
                var predicate = Setup<PredicateTestEntity>("Name", Operator.Eq, 12, 20, true);
                var parameters = new Dictionary<string, object>();

                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();

                Assert.Equal(2, parameters.Count);
                Assert.Equal(12, parameters["@Name_0"]);
                Assert.Equal(20, parameters["@Name_1"]);
                Assert.Equal("(fooCol NOT BETWEEN @Name_0 AND @Name_1)", sql);
            }

            protected Mock<BetweenPredicate<T>> Setup<T>(string propertyName, Operator op, object value1, object value2, bool not)
                where T : class
            {
                Mock<BetweenPredicate<T>> predicate = new Mock<BetweenPredicate<T>>();
                predicate.Object.PropertyName = propertyName;
                predicate.Object.Value = new BetweenValues { Value1 = value1, Value2 = value2 };
                predicate.Object.Not = not;
                predicate.CallBase = true;
                predicate.Protected().Setup<string>("GetColumnName", typeof(T), Generator.Object, propertyName).Returns("fooCol").Verifiable();
                return predicate;
            }
        }

        public class PredicateGroupTests : PredicatesFixtureBase
        {
            [Fact]
            public void EmptyPredicate__CreatesNoOp_And_ReturnsProperSql()
            {
                Mock<IPredicate> subPredicate1 = new Mock<IPredicate>();
                @SqlDialect.SetupGet(s => s.EmptyExpression).Returns("1=1").Verifiable();

                var subPredicates = new List<IPredicate> { subPredicate1.Object, subPredicate1.Object };
                var predicate = Setup(GroupOperator.And, subPredicates);
                var parameters = new Dictionary<string, object>();

                subPredicate1.Setup(s => s.GetSql(Generator.Object, parameters)).Returns("").Verifiable();
                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();
                @SqlDialect.Verify();
                subPredicate1.Verify(s => s.GetSql(Generator.Object, parameters), Times.AtMost(2));

                Assert.Empty(parameters);
                Assert.Equal("(1=1)", sql);
            }

            [Fact]
            public void GetSql_And_ReturnsProperSql()
            {
                Mock<IPredicate> subPredicate1 = new Mock<IPredicate>();
                var subPredicates = new List<IPredicate> { subPredicate1.Object, subPredicate1.Object };
                var predicate = Setup(GroupOperator.And, subPredicates);
                var parameters = new Dictionary<string, object>();

                subPredicate1.Setup(s => s.GetSql(Generator.Object, parameters)).Returns("subSql").Verifiable();
                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();
                subPredicate1.Verify(s => s.GetSql(Generator.Object, parameters), Times.AtMost(2));

                Assert.Empty(parameters);
                Assert.Equal("(subSql AND subSql)", sql);
            }

            [Fact]
            public void GetSql_Or_ReturnsProperSql()
            {
                Mock<IPredicate> subPredicate1 = new Mock<IPredicate>();
                var subPredicates = new List<IPredicate> { subPredicate1.Object, subPredicate1.Object };
                var predicate = Setup(GroupOperator.Or, subPredicates);
                var parameters = new Dictionary<string, object>();

                subPredicate1.Setup(s => s.GetSql(Generator.Object, parameters)).Returns("subSql").Verifiable();
                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();
                subPredicate1.Verify(s => s.GetSql(Generator.Object, parameters), Times.AtMost(2));

                Assert.Empty(parameters);
                Assert.Equal("(subSql OR subSql)", sql);
            }

            protected Mock<PredicateGroup> Setup(GroupOperator op, IList<IPredicate> predicates)
            {
                Mock<PredicateGroup> predicate = new Mock<PredicateGroup>();
                predicate.Object.Operator = op;
                predicate.Object.Predicates = predicates;
                predicate.CallBase = true;
                return predicate;
            }
        }

        public class ExistsPredicateTests : PredicatesFixtureBase
        {
            [Fact]
            public void GetSql_WithoutNot_ReturnsProperSql()
            {
                Mock<IPredicate> subPredicate = new Mock<IPredicate>();
                Mock<IClassMapper> subMap = new Mock<IClassMapper>();
                var predicate = Setup<PredicateTestEntity2>(subPredicate.Object, subMap.Object, false);
                Generator.Setup(g => g.GetTableName(subMap.Object)).Returns("subTable").Verifiable();

                var parameters = new Dictionary<string, object>();

                subPredicate.Setup(s => s.GetSql(Generator.Object, parameters)).Returns("subSql").Verifiable();
                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();
                subPredicate.Verify();
                Generator.Verify();

                Assert.Empty(parameters);
                Assert.Equal("(EXISTS (SELECT 1 FROM subTable WHERE subSql))", sql);
            }

            [Fact]
            public void GetSql_WithNot_ReturnsProperSql()
            {
                Mock<IPredicate> subPredicate = new Mock<IPredicate>();
                Mock<IClassMapper> subMap = new Mock<IClassMapper>();
                var predicate = Setup<PredicateTestEntity2>(subPredicate.Object, subMap.Object, true);
                Generator.Setup(g => g.GetTableName(subMap.Object)).Returns("subTable").Verifiable();

                var parameters = new Dictionary<string, object>();

                subPredicate.Setup(s => s.GetSql(Generator.Object, parameters)).Returns("subSql").Verifiable();
                var sql = predicate.Object.GetSql(Generator.Object, parameters);

                predicate.Verify();
                subPredicate.Verify();
                Generator.Verify();

                Assert.Empty(parameters);
                Assert.Equal("(NOT EXISTS (SELECT 1 FROM subTable WHERE subSql))", sql);
            }

            [Fact]
            public void GetClassMapper_NoMapFound_ThrowsException()
            {
                var predicate = new Mock<ExistsPredicate<PredicateTestEntity>>();
                predicate.CallBase = true;

                Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity2))).Returns(() => null).Verifiable();

                var ex = Assert.ThrowsAny<NullReferenceException>(() => predicate.Object.TestProtected().RunMethod<IClassMapper>("GetClassMapper", typeof(PredicateTestEntity2), Configuration.Object));

                Configuration.Verify();
                Assert.StartsWith("Map was not found", ex.Message);
            }

            [Fact]
            public void GetClassMapper_ReturnsMap()
            {
                Mock<IClassMapper> classMap = new Mock<IClassMapper>();
                var predicate = new Mock<ExistsPredicate<PredicateTestEntity>>();
                predicate.CallBase = true;

                Configuration.Setup(c => c.GetMap(typeof(PredicateTestEntity2))).Returns(classMap.Object).Verifiable();

                var result = predicate.Object.TestProtected().RunMethod<IClassMapper>("GetClassMapper", typeof(PredicateTestEntity2), Configuration.Object);

                Configuration.Verify();

                Assert.Equal(classMap.Object, result);
            }

            protected Mock<ExistsPredicate<T>> Setup<T>(IPredicate predicate, IClassMapper classMap, bool not) where T : class
            {
                Mock<ExistsPredicate<T>> result = new Mock<ExistsPredicate<T>>();
                result.Object.Predicate = predicate;
                result.Object.Not = not;
                result.Protected().Setup<IClassMapper>("GetClassMapper", typeof(T), Configuration.Object).Returns(classMap).Verifiable();
                result.CallBase = true;
                return result;
            }
        }

        public class PredicateTestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class PredicateTestEntity2
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }
    }
}