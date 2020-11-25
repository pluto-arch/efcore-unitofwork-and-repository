using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace PlutoData.Extensions
{
	/// <summary>
	/// 实体定义字段说明
	/// </summary>
	public static class EntityDefinitionExtensions
	{
		private static ConcurrentDictionary<string, List<PropertyInfo>> _cacheSubmeter = new ConcurrentDictionary<string, List<PropertyInfo>>();


		/// <summary>
		/// 获取某个特性
		/// </summary>
		/// <typeparam name="TAttribute">属性</typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		internal static TAttribute GetAttribute<TAttribute>(this Type entity) where TAttribute : Attribute
		{
			var attrs = entity.GetCustomAttributes(typeof(TAttribute));
			if (attrs?.Count() > 0)
				return (TAttribute)attrs.First();
			return null;
		}


		/// <summary>
		/// 获取某个特性
		/// </summary>
		/// <typeparam name="TAttribute">属性</typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		internal static TAttribute GetAttribute<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
		{
			var obs = property.GetCustomAttributes(typeof(TAttribute), false);
			if (obs?.Count() > 0)
				return (TAttribute)obs.First();
			return null;
		}


		public static PropertyInfo GetIdentityField<TEntity>(this TEntity entity)
		{
			var t = typeof(TEntity);
			var mTableName = t.GetMainTableName();
			var propertyInfos = t.GetProperties<DatabaseGeneratedAttribute>();
			if ((propertyInfos?.Count ?? 0) <= 0) // 代表没有主键
				return null;

			foreach (var pi in propertyInfos)
			{
				var attribute = pi.GetAttribute<DatabaseGeneratedAttribute>();
				if (attribute != null && attribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
				{
					return pi;
				}
			}
			return null;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		internal static List<PropertyInfo> GetProperties<TAttribute>(this Type entity) where TAttribute : Attribute
		{
			var propertyInfos = _cacheSubmeter.GetOrAdd($"{entity.Name}_{typeof(TAttribute).Name}",
			                                            key => entity.GetPropertyByAttribute<TAttribute>());
			return propertyInfos;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TAttribute">属性</typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		internal static List<PropertyInfo> GetPropertyByAttribute<TAttribute>(this Type entity) where TAttribute : Attribute
		{
			var list = new List<PropertyInfo>();
			var pis = entity.GetProperties();
			foreach (var item in pis)
			{
				var obs = item.GetCustomAttributes(typeof(TAttribute), false);
				if (obs?.Count() > 0)
					list.Add(item);
			}
			return list;
		}

	}
}