using System;

namespace StructureAssertions.Test.TestTypes
{
	public class ContainsStringPropertyWithLazyInitializerCall
	{
		readonly Lazy<string> _property = new Lazy<string>(() => StringInitializer.AString());
		public string Property
		{
			get { return _property.Value; }
		}
	}
}