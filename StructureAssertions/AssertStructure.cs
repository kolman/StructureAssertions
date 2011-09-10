using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace StructureAssertions
{
	public static class AssertStructure
	{
		public static TypeSelector InAssembly(Assembly assembly)
		{
			return new TypeSelector(assembly);
		}

		public static TypeSelector InAssemblyContaining<T>()
		{
			return InAssembly(typeof(T).Assembly);
		}

		public class TypeSelector
		{
			readonly AssemblyDefinition _assembly;

			public TypeSelector(Assembly assembly)
			{
				if (assembly == null) throw new ArgumentNullException("assembly");
				_assembly = AssemblyDefinition.ReadAssembly(assembly.Location);
			}

			public DependencyChecker Types(Func<TypeDefinition, bool> predicate)
			{
				return new DependencyChecker(_assembly, _assembly.MainModule.GetTypes().Where(predicate));
			}
		}

		public class DependencyChecker
		{
			readonly AssemblyDefinition _assembly;
			readonly IEnumerable<TypeDefinition> _inputTypes;

			public DependencyChecker(AssemblyDefinition assembly, IEnumerable<TypeDefinition> inputTypes)
			{
				if (assembly == null) throw new ArgumentNullException("assembly");
				if (inputTypes == null) throw new ArgumentNullException("inputTypes");
				_assembly = assembly;
				_inputTypes = inputTypes;
			}

			public void MustNotReference(Func<TypeReference, bool> filter)
			{
				var violations = _inputTypes.SelectMany(t => GetViolations(t, filter)).ToArray();
				if (violations.Length > 0)
				{
					throw new ForbiddenDependencyException(_assembly, violations);
				}
			}

			IEnumerable<string> GetViolations(TypeDefinition type, Func<TypeReference, bool> filter)
			{
				const string messageFormat = "  {0} references {1}";
				return type.GetDependencies()
					.Where(filter)
					.Select(r => string.Format(messageFormat, GetName(type), GetName(r)));
			}

			string GetName(TypeReference reference)
			{
				return reference.FullName.Replace("/", "+");
			}
		}
	}
}