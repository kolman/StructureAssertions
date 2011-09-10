namespace StructureAssertions.Test.TestTypes
{
	public class ContainsMethodWithDecimalVariable
	{
		public void DoSomething()
		{
			var dec = 123M;
			Consumer.Use(dec);
		}
	}
}