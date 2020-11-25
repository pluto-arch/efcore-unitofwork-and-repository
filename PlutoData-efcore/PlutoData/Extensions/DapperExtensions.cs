using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using PlutoData.Attrs;
using PlutoData.Enums;

namespace PlutoData.Extensions
{
	/// <summary>
	/// dapper 扩展
	/// </summary>
	public static class DapperExtensions
	{
		/// <summary>
		/// 添加
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="connection"></param>
		/// <param name="tableName"></param>
		/// <param name="entity"></param>
		/// <param name="returnLastIdentity"></param>
		/// <param name="outSqlAction"></param>
		/// <returns></returns>
		public static async Task<int> InsertAsync<TEntity>(this IDbConnection connection,
		                                                   string tableName,
		                                                   TEntity entity,
		                                                   bool returnLastIdentity = false,
		                                                   Action<string> outSqlAction = null)
				where TEntity : class, new()
		{
			if (string.IsNullOrEmpty(tableName))
				throw new ArgumentNullException(nameof(tableName));
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));
			var addFields = new List<string>();
			var atFields = new List<string>();
			var dbType = connection.GetDbType();
			var pis = typeof(TEntity).GetProperties();
			var identityPropertyInfo = entity.GetIdentityField();
			foreach (var pi in pis)
			{
				if (identityPropertyInfo?.Name == pi.Name)
					continue;
				if (pi.GetAttribute<DapperIgnoreAttribute>()!=null)
					continue;
				addFields.Add($"{pi.Name.ParamSql(dbType)}");
				atFields.Add($"@{pi.Name}");
			}
			var sql = $"insert into {tableName.ParamSql(dbType)}({string.Join(", ", addFields)}) values({string.Join(", ", atFields)});";
			outSqlAction?.Invoke(sql);
			var task = 0;
			if (identityPropertyInfo != null && returnLastIdentity)
			{
				sql += dbType.SelectLastIdentity();
				task = await connection.ExecuteScalarAsync<int>(sql, entity);
				if (task > 0)
					identityPropertyInfo.SetValue(entity, task);
			}
			else
			{
				task = await connection.ExecuteAsync(sql, entity);
			}
			return task;
		}

		/// <summary>
		/// 插入多个
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="connection"></param>
		/// <param name="tableName"></param>
		/// <param name="entities"></param>
		/// <param name="outSqlAction"></param>
		/// <returns></returns>
		public static async Task<int> InsertAsync<TEntity>(this IDbConnection connection,
		                                                   string tableName,
		                                                   IEnumerable<TEntity> entities,
		                                                   Action<string> outSqlAction = null)
				where TEntity : class, new()
		{
			if (string.IsNullOrEmpty(tableName))
				throw new ArgumentNullException(nameof(tableName));
			if ((entities?.Count() ?? 0) <= 0)
				throw new ArgumentNullException(nameof(entities));

			var addFields = new List<string>();
			var atFields = new List<string>();
			var dbType = connection.GetDbType();

			var pis = typeof(TEntity).GetProperties();
			var identityPropertyInfo = entities.First().GetIdentityField();

			foreach (var pi in pis)
			{
				if (identityPropertyInfo?.Name == pi.Name)
					continue;
				if (pi.GetAttribute<DapperIgnoreAttribute>()!=null)
					continue;
				addFields.Add($"{pi.Name.ParamSql(dbType)}");
				atFields.Add($"@{pi.Name}");
			}

			var sql = $"insert into {tableName.ParamSql(dbType)}({string.Join(", ", addFields)}) values({string.Join(", ", atFields)});";
			outSqlAction?.Invoke(sql);
			var task = await connection.ExecuteAsync(sql, entities);
			return task;
		}


		#region common
		static ConcurrentDictionary<string, EnumDatabaseType> MSSqlDbType = new ConcurrentDictionary<string, EnumDatabaseType>();
		internal static EnumDatabaseType GetDbType(this IDbConnection connection)
		{
			if (connection is SqlConnection)
			{
				return MSSqlDbType.GetOrAdd(connection.ConnectionString, (connectionString) =>EnumDatabaseType.SqlServer);
			}
			return EnumDatabaseType.SqlServer;
		}

		#endregion
	}
}