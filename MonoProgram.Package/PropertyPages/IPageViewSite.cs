namespace MonoProgram.Package.PropertyPages
{
	public interface IPageViewSite
	{
		void PropertyChanged(string propertyName, string propertyValue);
		string GetValueForProperty(string propertyName);
	}
}