/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 31/05/2017
 * Time: 01:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using OpenWeatherAPI;

namespace appCore.Toolbox
{
	/// <summary>
	/// Description of QueryFactory.
	/// </summary>
	public class QueryFactory
	{
		public class FooFactory : FactoryConverter<Query>
		{
			public FooFactory(Query bar)
			{
				this.Query = bar;
			}

			public Query Query { get; private set; }
//
//			public override Foo Create(Type objectType, Dictionary<string, string> arguments)
//			{
//				return new Foo(Bar, arguments["X"], arguments["Y"]);
//			}
		}
	}
}
