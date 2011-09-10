using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace StructureAssertions
{
	public class AssertAssemblyStructure
	{
		readonly AssemblyDefinition _assembly;

		internal AssertAssemblyStructure(Assembly assembly)
		{
			if (assembly == null) throw new ArgumentNullException("assembly");
			_assembly = AssemblyDefinition.ReadAssembly(assembly.Location);
		}

		public AssertTypesStructure Types(Func<TypeDefinition, bool> predicate)
		{
			return new AssertTypesStructure(_assembly, _assembly.MainModule.GetTypes().Where(predicate));
		}
	}
}