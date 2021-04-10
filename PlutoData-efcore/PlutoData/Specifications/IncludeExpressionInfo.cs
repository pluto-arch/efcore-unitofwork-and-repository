using PlutoData.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PlutoData.Specifications
{
    /// <summary>
    /// 导航属性信息
    /// </summary>
    public class IncludeExpressionInfo
    {
        /// <summary>
        /// LambdaExpression
        /// </summary>
        public LambdaExpression LambdaExpression { get; }

        /// <summary>
        /// EntityType
        /// </summary>
        public Type EntityType { get; }
        /// <summary>
        /// PropertyType
        /// </summary>
        public Type PropertyType { get; }
        /// <summary>
        /// PreviousPropertyType
        /// </summary>
        public Type? PreviousPropertyType { get; }

        /// <summary>
        /// IncludeTypeEnum
        /// </summary>
        public IncludeTypeEnum Type { get; }

        private IncludeExpressionInfo(LambdaExpression expression,
                                      Type entityType,
                                      Type propertyType,
                                      Type? previousPropertyType,
                                      IncludeTypeEnum includeType)

        {
            _ = expression ?? throw new ArgumentNullException(nameof(expression));
            _ = entityType ?? throw new ArgumentNullException(nameof(entityType));
            _ = propertyType ?? throw new ArgumentNullException(nameof(propertyType));

            if (includeType == IncludeTypeEnum.ThenInclude)
            {
                _ = previousPropertyType ?? throw new ArgumentNullException(nameof(previousPropertyType));
            }

            this.LambdaExpression = expression;
            this.EntityType = entityType;
            this.PropertyType = propertyType;
            this.PreviousPropertyType = previousPropertyType;
            this.Type = includeType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="entityType"></param>
        /// <param name="propertyType"></param>
        public IncludeExpressionInfo(LambdaExpression expression,
                                     Type entityType,
                                     Type propertyType)
            : this(expression, entityType, propertyType, null, IncludeTypeEnum.Include)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="entityType"></param>
        /// <param name="propertyType"></param>
        /// <param name="previousPropertyType"></param>
        public IncludeExpressionInfo(LambdaExpression expression,
                                     Type entityType,
                                     Type propertyType,
                                     Type previousPropertyType)
            : this(expression, entityType, propertyType, previousPropertyType, IncludeTypeEnum.ThenInclude)
        {
        }
    }
}
