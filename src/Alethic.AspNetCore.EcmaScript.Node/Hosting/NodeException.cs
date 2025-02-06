using System;

namespace Alethic.AspNetCore.EcmaScript.Node.Hosting
{

	internal class NodeException : Exception
	{

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="message"></param>
		public NodeException(string? message) :
			base(message)
		{

		}

	}

}
