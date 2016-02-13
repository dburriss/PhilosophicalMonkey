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
