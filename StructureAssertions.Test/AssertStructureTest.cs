using NUnit.Framework;
using FluentAssertions;

namespace StructureAssertions.Test
{
	[TestFixture]
	public class AssertStructureTest
	{
		[Test]
		public void FailsWhenAssemblyContainsForbiddenDependencies()
		{
			Assert.Throws<ForbiddenDependencyException>(AssertForbiddenReferenceExists);
		}

		[Test]
		public void ExceptionContainsCorrectNestedClassNameWhenAssemblyContainsForbiddenDependencies()
		{
			ForbiddenDependencyException exception = null;
			try
			{
				AssertForbiddenReferenceExists();
			}
			catch (ForbiddenDependencyException e)
			{
				exception = e;
			}
			exception.Should().NotBeNull("Test assertion should throw correct exception");
			exception.Message.Should().Contain(typeof(ClassB).FullName);
		}

		static void AssertForbiddenReferenceExists()
		{
			AssertStructure
				.InAssemblyContaining<ClassA>()
				.Types(t => t.Name == "ClassB")
				.MustNotReference(r => r.Name == "ClassA");
		}

		[Test]
		public void SucceedsWhenAssemblyDoesNotContainForbiddenDependencies()
		{
			AssertStructure
				.InAssembly(typeof(ClassA).Assembly)
				.Types(t => true)
				.MustNotReference(r => r.Name == "ClassC");
		}

		class ClassA
		{
		}

		class ClassB
		{
			public ClassA A { get; set; }
		}

		class ClassC
		{

		}
	}

}