using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace StructureAssertions
{
	public class ForbiddenDependencyException : Exception
	{
		const string MessageFormat = "Assembly {0} contains forbidden dependencies:\n{1}";

		internal ForbiddenDependencyException(AssemblyDefinition assembly, IEnumerable<string> dependencies)
			: base(string.Format(MessageFormat, assembly.MainModule.Name, string.Join("\n", dependencies)))
		{
		}
	}
}