using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace StructureAssertions
{
	public class AssertTypesStructure
	{
		readonly AssemblyDefinition _assembly;
		readonly IEnumerable<TypeDefinition> _inputTypes;

		internal AssertTypesStructure(AssemblyDefinition assembly, IEnumerable<TypeDefinition> inputTypes)
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
				.Select(r => String.Format(messageFormat, GetName(type), GetName(r)));
		}

		string GetName(TypeReference reference)
		{
			return reference.FullName.Replace("/", "+");
		}
	}
}