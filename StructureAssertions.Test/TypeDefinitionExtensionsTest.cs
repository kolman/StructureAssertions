using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mono.Cecil;
using NUnit.Framework;
using StructureAssertions.Test.TestTypes;

namespace StructureAssertions.Test
{
	[TestFixture]
	public class TypeDefinitionExtensionsTest
	{
		readonly AssemblyDefinition _thisAssembly =
			AssemblyDefinition.ReadAssembly(typeof(TypeDefinitionExtensionsTest).Assembly.Location);

		IEnumerable<TypeReference> _dependencies;

		[Test]
		public void EmptyTypeHasNoDependencies()
		{
			ReadDependenciesOfType<Empty>();
			AssertDependencies(new Type[0]);
		}

		[Test]
		public void ReturnsDependenciesFromFieldType()
		{
			ReadDependenciesOfType<ContainsStringField>();
			AssertDependencies(typeof(string));
		}

		[Test]
		public void ReturnsDependenciesFromPropertyType()
		{
			ReadDependenciesOfType<ContainsStringProperty>();
			AssertDependencies(typeof(string));
		}

		[Test]
		public void ReturnsDependenciesFromPropertyMethod()
		{
			ReadDependenciesOfType<ContainsStringPropertyCallingInitializer>();
			AssertDependencies(typeof(string), typeof(StringInitializer));
		}

		[Test]
		public void ReturnsDependenciesFromLazyProperty()
		{
			ReadDependenciesOfType<ContainsStringPropertyWithLazyInitializerCall>();
			_dependencies
				.Select(t => t.FullName)
				// There are too many references because of using Lazy<T>, we care only about
				// StringInitializer, which is used in lazy initialization method
				.Should().Contain(new[] {typeof (string).FullName, typeof (StringInitializer).FullName});
		}

		[Test]
		public void ReturnsDependenciesFromFieldInitializer()
		{
			ReadDependenciesOfType<ContainsStringFieldWithInitializer>();
			AssertDependencies(typeof(string), typeof(StringInitializer));
		}

		[Test]
		public void ReturnsDependenciesFromMethodReturnType()
		{
			ReadDependenciesOfType<ContainsMethodReturningBool>();
			AssertDependencies(typeof(bool));
		}

		[Test]
		public void ReturnsDependenciesFromMethodParameters()
		{
			ReadDependenciesOfType<ContainsMethodWithInt32Argument>();
			AssertDependencies(typeof(int));
		}

		[Test]
		public void ReturnsDependenciesFromMethodVariables()
		{
			ReadDependenciesOfType<ContainsMethodWithDecimalVariable>();
			// Compiler optimization in Release build omits variables which are
			// not used. We must therefore call Consumer to do something with the variable.
			AssertDependencies(typeof(decimal), typeof(Consumer));
		}

		[Test]
		public void ReturnsDependenciesFromMethodInstructions()
		{
			ReadDependenciesOfType<ContainsMethodWithCallToStringInitializer>();
			AssertDependencies(typeof(StringInitializer));
		}

		[Test]
		public void ReturnsDependenciesFromBaseClass()
		{
			ReadDependenciesOfType<ClassWithBaseClassUsingString>();
			AssertDependencies(typeof(ABaseClass), typeof(string));
		}

		[Test]
		public void ReturnsDependenciesFromNestedClass()
		{
			ReadDependenciesOfType<ClassWithNestedClassUsingString>();
			AssertDependencies(typeof(string));
		}

		#region Helper Methods
		void ReadDependenciesOfType<T>()
		{
			_dependencies = GetTypeDefinition<T>().GetDependencies();
		}

		TypeDefinition GetTypeDefinition<T>()
		{
			return _thisAssembly.MainModule.GetTypes().First(t => t.FullName == typeof(T).FullName);
		}

		void AssertDependencies(params Type[] expectedDependencies)
		{
			_dependencies
				.Select(t => t.FullName)
				.Where(d => d != typeof(object).FullName) // every object depends on System.Object
				.Should().BeEquivalentTo(expectedDependencies.Select(t => t.FullName));
		}
		#endregion
	}
}