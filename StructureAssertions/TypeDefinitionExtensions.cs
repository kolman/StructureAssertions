using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace StructureAssertions
{
	public static class TypeDefinitionExtensions
	{
		public static IEnumerable<TypeReference> GetDependencies(this TypeDefinition type)
		{
			return GetDependenciesOfClassHierarchy(type).Distinct();
		}

		static IEnumerable<TypeReference> GetDependenciesOfClassHierarchy(this TypeDefinition type)
		{
			while (type != null)
			{
				foreach (var reference in GetDependenciesOfClassIncludingNested(type))
				{
					yield return reference;
				}
				type = type.BaseType == null || type.BaseType.FullName == "System.Object"
				       	? null
				       	: type.BaseType.Resolve();
			}
		}

		static IEnumerable<TypeReference> GetDependenciesOfClassIncludingNested(this TypeDefinition type)
		{
			return GetDependenciesOfClass(type)
				.Union(type.NestedTypes.SelectMany(GetDependenciesOfClass));
		}

		static IEnumerable<TypeReference> GetDependenciesOfClass(this TypeDefinition type)
		{
			return type.Fields.SelectMany(GetDependencies)
				.Union(type.Methods.SelectMany(GetDependencies))
				.Where(t => t != type)
				.Distinct();
		}

		static IEnumerable<TypeReference> GetDependencies(FieldDefinition field)
		{
			yield return field.FieldType;
		}

		static IEnumerable<TypeReference> GetDependencies(MethodDefinition method)
		{
			var types = method.ReturnType.FullName != "System.Void"
							? new[] { method.ReturnType }
							: new TypeReference[0];
			return types
				.Union(method.Parameters.Select(p => p.ParameterType))
				.Union(GetDependencies(method.Body));
		}

		static IEnumerable<TypeReference> GetDependencies(MethodBody body)
		{
			if (body == null) return new TypeReference[0];
			return body.Variables.Select(v => v.VariableType)
				.Union(body.Instructions.SelectMany(GetDependencies));
		}

		static IEnumerable<TypeReference> GetDependencies(Instruction instruction)
		{
			var mr = instruction.Operand as MemberReference;
			if (mr != null && mr.DeclaringType != null)
				yield return mr.DeclaringType;

			var tr = instruction.Operand as TypeReference;
			if (tr != null)
				yield return tr;
		}
	}
}
