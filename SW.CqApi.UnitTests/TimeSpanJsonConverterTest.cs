using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;


namespace SW.CqApi.UnitTests
{
    [TestClass]
    public class TimeSpanJsonConverterTest
    {
        class Dto
        {
            public TimeSpan Data { get; set; }
            public TimeSpan? NullableData { get; set; }
            public TimeSpan EmptyData { get; set; }
            public TimeSpan? EmptyNullableData { get; set; }

        }
        [TestMethod]
        public void TestDeSerialization()
        {

            var js = new JsonSerializer();
            js.Converters.Add(new TimeSpanJsonConverter());

            var serialized = @"{ ""Data"": ""10:10:10"" , ""NullableData"": ""9:9:9"" }";


            using (var jtr = new JsonTextReader(new StringReader(serialized)))
            {
                var result = js.Deserialize<Dto>(jtr);

                Assert.AreEqual(result.Data.Minutes, 10);
                Assert.AreEqual(result.NullableData.Value.Minutes, 9);
                Assert.IsNull(result.EmptyNullableData);



            }


        }

        [TestMethod]
        public void TestInvalidDeSerialization()
        {

            var js = new JsonSerializer();
            js.Converters.Add(new TimeSpanJsonConverter());

            var serialized = @"{ ""Data"": ""abcd"" }";


            using (var jtr = new JsonTextReader(new StringReader(serialized)))
            {
                var exceptionThrown = false;

                try
                {

                    var result = js.Deserialize<Dto>(jtr);
                }
                catch (Exception ex)
                {

                    exceptionThrown = true;
                    var castException = (JsonSerializationException)ex;
                    Assert.AreEqual(castException.Message, "can not deserialize value abcd to TimeSpan");
                }

                Assert.IsTrue(exceptionThrown);


            }


        }

        class OneField
        {
            public TimeSpan Data { get; set; }
        }
        [TestMethod]
        public void TesSerialization()
        {

            var js = new JsonSerializer();
            js.Converters.Add(new TimeSpanJsonConverter());

            var data = new OneField
            {
                Data = TimeSpan.FromSeconds(1)
            };

            var result = JsonConvert.SerializeObject(data, new TimeSpanJsonConverter());

            Assert.AreEqual(result, @"{""Data"":""00:00:01""}");
        }

    }
}
