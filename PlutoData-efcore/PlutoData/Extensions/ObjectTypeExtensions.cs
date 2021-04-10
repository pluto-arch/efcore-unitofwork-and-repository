using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace PlutoData.Extensions
{
	/// <summary>
	/// 对象类型扩展
	/// </summary>
	public static class ObjectTypeExtensions
	{
		/// <summary>
		/// 获取主表名称
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		internal static string GetMainTableName(this Type entity)
		{
			var attribute = entity.GetAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>();
            var attributeDapper = entity.GetAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>();
            if (attribute == null&&attributeDapper==null)
            {
                return entity.Name;
            }
            else
            {
                return attribute==null?attributeDapper.Name:attribute.Name;
            }
		}
	}
}