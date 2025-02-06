// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Alethic.AspNetCore.EcmaScript.Node.Hosting;

/// <summary>
/// Extension methods that help with populating a <see cref="NodeServicesOptions"/> object.
/// </summary>
public static class NodeServicesOptionsExtensions
{

	/// <summary>
	/// Configures the <see cref="INodeService"/> service so that it will use out-of-process
	/// Node.js instances and perform RPC calls over HTTP.
	/// </summary>
	public static void UseHttpHosting(this NodeServicesOptions options)
	{
		options.NodeInstanceFactory = () =>
		{
			return new HttpNodeInstance(options);
		};
	}

}
