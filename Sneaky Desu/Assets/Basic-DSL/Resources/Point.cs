using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSL.Core
{
	/// <summary>
	/// Specify a jumping point when certain events occur in the dialogue set.
	/// </summary>
	public class Point : MonoBehaviour
	{
		public int Location { get; private set; }

		public Point(int _location)
		{
			Location = _location;
		}
	} 
}
