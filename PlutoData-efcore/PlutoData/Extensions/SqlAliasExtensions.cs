using System;
using PlutoData.Enums;

namespace PlutoData.Extensions
{
	public static class SqlAliasExtensions
	{
		/// <summary>
		/// 参数前缀
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static string ParamPrefix(this EnumDatabaseType dbType)
		{
			switch (dbType)
			{
				case EnumDatabaseType.SqlServer:
					return "@";
				case EnumDatabaseType.MySql:
					return "?";
				case EnumDatabaseType.SQLite:
					return "@";
				default:
					return string.Empty;
			}
		}


		/// <summary>
		/// 获取添加左右标记 防止有关键字作为字段名/表名
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static string ParamSql(this string columnName, EnumDatabaseType? dbType)
		{
			switch (dbType)
			{
				case EnumDatabaseType.SqlServer:
					if (columnName.StartsWith("["))
						return columnName;
					return $"[{columnName}]";
				case EnumDatabaseType.MySql:
					if (columnName.StartsWith("`"))
						return columnName;
					return $"`{columnName}`";
				case EnumDatabaseType.SQLite:
					if (columnName.StartsWith("`"))
						return columnName;
					return $"`{columnName}`";
				default:
					return columnName;
			}
		}

		/// <summary>
		/// 获取最后一次Insert
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static string SelectLastIdentity(this EnumDatabaseType dbType)
		{
			switch (dbType)
			{
				case EnumDatabaseType.SqlServer:
					return " select @@Identity";
				case EnumDatabaseType.MySql:
					return " select LAST_INSERT_ID();";
				case EnumDatabaseType.SQLite:
					return " select last_insert_rowid();";
				default:
					return string.Empty;
			}
		}
	}
}