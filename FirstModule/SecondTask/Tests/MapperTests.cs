using NUnit.Framework;

namespace SecondTask.Tests
{
	[TestFixture]
	public class MapperTests
	{
		[Test]
		public void Map_MapsOneClassToAnother()
		{
			const int expectedAge = 20;
			const string expectedName = "Cool Foo";
			var mapGenerator = new MappingGenerator();
			var mapper = mapGenerator.Generate<Foo, Bar>();

			var result = mapper.Map(new Foo()
			{
				Age = expectedAge,
				IsCool = true,
				Name = expectedName
			});

			Assert.IsNotNull(result, "Result should not be null!");
			Assert.IsInstanceOf<Bar>(result, "Result should be a Bar!");
			Assert.AreEqual(expectedAge, result.Age, "Age should be 20!");
			Assert.AreEqual(expectedName, result.Name, string.Format("Name should be: '{0}'", expectedName));
		}
	}
}