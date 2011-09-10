namespace StructureAssertions.Test.TestTypes
{
	public class ContainsStringPropertyCallingInitializer
	{
		string _property;
		public string Property
		{
			get { return _property ?? (_property = StringInitializer.AString()); }
		}
	}
}