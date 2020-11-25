using System;

namespace PlutoData.Attrs
{
	[AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
	public class DapperIgnoreAttribute:Attribute
	{
	}
}