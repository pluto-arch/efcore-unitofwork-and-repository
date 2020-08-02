using System;
using System.Linq;
using System.Linq.Expressions;

namespace PlutoData.Extensions
{
	/// <summary>
	/// 表达式扩展
	/// </summary>
	public static class ExpressionExtensions
	{
		/// <summary>
		/// default true Expresion
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Expression<Func<T, bool>> True<T>() { return f => true; }
		/// <summary>
		///  default false Expresion
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Expression<Func<T, bool>> False<T>() { return f => false; }
		
		
		/// <summary>
		/// Expression OR
		/// </summary>
		/// <param name="expr1"></param>
		/// <param name="expr2"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
		                                              Expression<Func<T, bool>> expr2)
		{
			var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
			return Expression.Lambda<Func<T, bool>>
				(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
		}

		/// <summary>
		/// Expression AND
		/// </summary>
		/// <param name="expr1"></param>
		/// <param name="expr2"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
		                                               Expression<Func<T, bool>> expr2)
		{
			var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
			return Expression.Lambda<Func<T, bool>>
				(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
		}
		
	}
}