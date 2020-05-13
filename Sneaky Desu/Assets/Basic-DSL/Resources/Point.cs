namespace DSL.Core
{
	/// <summary>
	/// Specify a jumping point when certain events occur in the dialogue set.
	/// </summary>
	public class Point
	{
		public int Location { get; private set; }

		public Point(int _location)
		{
			Location = _location;
		}
	} 
}
