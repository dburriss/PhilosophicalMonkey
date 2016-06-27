using System;
using System.Collections.Generic;
using TestModels;
using Xunit;

namespace PhilosophicalMonkey.Tests
{
    public class OnMappingsTests
    {

        [Fact]
        public void MapDynamicToDictionary_OnCorrectlyShapedDynamic_MapsToDictionary()
        {
            dynamic d = new { Nr = 1, Name = "Devon" };
            var dictionary = Reflect.OnMappings.TurnObjectIntoDictionary(d);

            Assert.Equal(2, dictionary.Keys.Count);
        }

        [Fact]
        public void MapComplexTypeToDictionary_OnAnyComplexType_MapsToDictionary()
        {
            var complexPerson = new Person() {
                Name = "Complex",
                DOB = DateTime.Now,
                Address = new Address()
                {
                    Street ="Complex Street",
                    StreetNr = 1
                }
            };
            var dictionary = Reflect.OnMappings.TurnObjectIntoDictionary(complexPerson);
            Assert.IsType<Dictionary<string,object>>(dictionary);
        }

        [Fact]
        public void MapComplexTypeToDictionary_OnAnyComplexType_MapsSubTypesToDictionary()
        {
            var complexPerson = new Person()
            {
                Name = "Complex",
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
        public void MapComplexTypeToDictionary_OnAnyComplexType_SubDictionaryKeyCountEqualsTwo()
        {
            var complexPerson = new Person()
            {
                Name = "Complex",
                DOB = DateTime.Now,
                Address = new Address()
                {
                    Street = "Complex Street",
                    StreetNr = 1
                }
            };
            var dictionary = Reflect.OnMappings.TurnObjectIntoDictionary(complexPerson);
            Assert.Equal(((Dictionary<string,object>)dictionary["Address"]).Keys.Count, 2);
        }

        [Fact]
        public void Map_FromDictionaryToFlatType_MapsValues()
        {
            dynamic d = new { StreetNr = 1, Street = "Main Rd" };
            var dictionary = new Dictionary<string, object>() { { "StreetNr", 1 }, { "Street", "Main Rd" } };
            var instance = new Address();
            Reflect.OnMappings.Map(dictionary, instance);
            Assert.Equal(instance.StreetNr, 1);
            Assert.Equal(instance.Street, "Main Rd");
        }

    }
}
