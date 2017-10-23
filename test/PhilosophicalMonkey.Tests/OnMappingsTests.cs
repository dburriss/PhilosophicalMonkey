using System;
using System.Collections.Generic;
using TestModels;
using Xunit;

namespace PhilosophicalMonkey.Tests
{
    public class OnMappingsTests
    {

        [Fact]
        public void TurnObjectIntoDictionary_OnCorrectlyShapedDynamic_MapsToDictionary()
        {
            dynamic d = new {Nr = 1, Name = "Bob"};
            var dictionary = Reflect.OnMappings.TurnObjectIntoDictionary(d);

            Assert.Equal(2, dictionary.Keys.Count);
        }

        [Fact]
        public void TurnObjectIntoDictionary_OnAnyComplexType_MapsToDictionary()
        {
            var complexPerson = new Person()
            {
                Name = "Bob",
                DOB = DateTime.Now,
                Address = new Address()
                {
                    Street = "Complex Street",
                    StreetNr = 1
                }
            };
            var dictionary = Reflect.OnMappings.TurnObjectIntoDictionary(complexPerson);
            Assert.IsType<Dictionary<string, object>>(dictionary);
        }

        [Fact]
        public void TurnObjectIntoDictionary_OnAnyComplexType_MapsSubTypesToDictionary()
        {
            var complexPerson = new Person()
            {
                Name = "Bob",
                DOB = DateTime.Now,
                Address = new Address()
                {
                    Street = "Complex Street",
                    StreetNr = 1
                }
            };
            var dictionary = Reflect.OnMappings.TurnObjectIntoDictionary(complexPerson);
            Assert.IsType<Dictionary<string, object>>(dictionary["Address"]);
        }

        [Fact]
        public void TurnObjectIntoDictionary_OnAnyComplexType_SubDictionaryKeyCountEqualsTwo()
        {
            var complexPerson = new Person()
            {
                Name = "Bob",
                DOB = DateTime.Now,
                Address = new Address()
                {
                    Street = "Complex Street",
                    StreetNr = 1
                }
            };
            var dictionary = Reflect.OnMappings.TurnObjectIntoDictionary(complexPerson);
            Assert.Equal(2, ((Dictionary<string, object>) dictionary["Address"]).Keys.Count);
        }

        [Fact]
        public void TurnDictionaryIntoObject_OnAnyComplexType_FillsInNestedValues()
        {
            var dictionary = new Dictionary<string, object>()
            {
                {"Name", "Bob"},
                {"DOB", DateTime.UtcNow},
                {
                    "Address", new Dictionary<string, object>()
                    {
                        {"StreetNr", 1},
                        {"Street", "Main Rd"}
                    }
                },
            };
            var instance = (Person) Reflect.OnMappings.TurnDictionaryIntoObject(dictionary, typeof(Person));
            Assert.Equal("Bob", instance.Name);
            Assert.Equal("Main Rd", instance.Address.Street);
        }

        [Fact]
        public void TurnDictionaryIntoObjectT_OnAnyComplexType_FillsInNestedValues()
        {
            var dictionary = new Dictionary<string, object>()
            {
                {"Name", "Bob"},
                {"DOB", DateTime.UtcNow},
                {
                    "Address", new Dictionary<string, object>()
                    {
                        {"StreetNr", 1},
                        {"Street", "Main Rd"}
                    }
                },
            };
            var instance = Reflect.OnMappings.TurnDictionaryIntoObject<Person>(dictionary);
            Assert.Equal("Bob", instance.Name);
            Assert.Equal("Main Rd", instance.Address.Street);
        }

        [Fact]
        public void Map_FromDictionaryToFlatType_MapsValues()
        {
            var dictionary = new Dictionary<string, object>() {{"StreetNr", 1}, {"Street", "Main Rd"}};
            var instance = new Address();
            Reflect.OnMappings.Map(dictionary, instance);
            Assert.Equal(1, instance.StreetNr);
            Assert.Equal("Main Rd", instance.Street);
        }

        [Fact]
        public void Map_FromDictionariesToComplexType_MapsValues()
        {
            var dictionary = new Dictionary<string, object>()
            {
                {"Name", "Bob"},
                {"DOB", DateTime.UtcNow},
                {
                    "Address", new Dictionary<string, object>()
                    {
                        {"StreetNr", 1},
                        {"Street", "Main Rd"}
                    }
                },
            };
            var instance = new Person();
            Reflect.OnMappings.Map(dictionary, instance);
            Assert.Equal("Bob", instance.Name);
            Assert.Equal("Main Rd", instance.Address.Street);
        }

        [Fact]
        public void MapT_FromDictionariesToComplexType_CreatesInstanceWithValuesSet()
        {
            var dictionary = new Dictionary<string, object>()
            {
                {"Name", "Bob"},
                {"DOB", DateTime.UtcNow},
                {
                    "Address", new Dictionary<string, object>()
                    {
                        {"StreetNr", 1},
                        {"Street", "Main Rd"}
                    }
                },
            };
            var instance = Reflect.OnMappings.Map<Person>(dictionary);
            Assert.Equal("Bob", instance.Name);
            Assert.Equal("Main Rd", instance.Address.Street);
        }

        [Fact]
        public void Map_BetweenSameTypes_MapsAllValues()
        {
            var p1 = new Person()
            {
                Name = "Bob",
                DOB = DateTime.Now,
                Address = new Address()
                {
                    Street = "Complex Street",
                    StreetNr = 1
                }
            };

            var p2 = new Person();

            Reflect.OnMappings.Map(p1, p2);
            Assert.Equal(p1.Name, p2.Name);
            Assert.Equal(p1.DOB, p2.DOB);
            Assert.Equal(p1.Address.Street, p2.Address.Street);
            Assert.Equal(p1.Address.StreetNr, p2.Address.StreetNr);
        }

        [Fact]
        public void Map_BetweenSameTypesWithNull_MapsAllValues()
        {
            var p1 = new Person()
            {
                Name = null,
                DOB = DateTime.Now,
                Address = null
            };
            var p2 = new Person();

            Reflect.OnMappings.Map(p1, p2);

            Assert.Null(p2.Name);
            Assert.Null(p2.Address);
        }


        [Fact]
        public void Map_BetweenDifferentTypes_MapsAllValuesInSource()
        {
            var p1 = new Person()
            {
                Name = null,
                DOB = DateTime.Now,
                Address = null
            };
            var p2 = new Person();

            Reflect.OnMappings.Map(p1, p2);

            Assert.Null(p2.Name);
            Assert.Null(p2.Address);
        }

        [Fact]
        public void Map_BetweenDifferentTypes_MapsAllValuesShared()
        {
            var x = new X()
            {
                Num = 1,
                Name = null
            };
            var y = new Y();

            Reflect.OnMappings.Map(x, y);

            Assert.Null(y.Name);
            Assert.Null(y.JustInY);
            Assert.Equal(1, y.Num);
        }

        [Fact]
        public void Map_WhenPropertiesArePrivate_DoesMap()
        {
            var x = new X()
            {
                Num = 1,
                Name = null
            };
            var y = new Y();

            Reflect.OnMappings.Map(x, y);

            Assert.Null(y.Name);
            Assert.Null(y.JustInY);
            Assert.Equal(1, y.Num);
        }


        internal class X
        {
            public int Num { get; set; }
            public string Name { get; set; }
            public string JustInX { get; set; }
            internal string Xxx { get; private set; }

        }

        internal class Y
        {
            public int Num { get; set; }
            public string Name { get; set; }
            internal string JustInY { get; private set; }
        }
    }
}
