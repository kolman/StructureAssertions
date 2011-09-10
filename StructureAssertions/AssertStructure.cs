using System.Reflection;

namespace StructureAssertions
{
	public static class AssertStructure
	{
		public static AssertAssemblyStructure InAssembly(Assembly assembly)
		{
			return new AssertAssemblyStructure(assembly);
		}

		public static AssertAssemblyStructure InAssemblyContaining<T>()
		{
			return InAssembly(typeof(T).Assembly);
		}
	}
}