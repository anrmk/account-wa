using System;
using System.Linq.Expressions;

namespace Core.Services.Business {
    public abstract class BaseBusinessManager {
        public static Expression<Func<TSource, string>> GetExpression<TSource>(string propertyName) {
            var param = Expression.Parameter(typeof(TSource), "x");
            var property = Expression.Property(param, propertyName);
            Expression conversion = Expression.Convert(property, typeof(string));
            return Expression.Lambda<Func<TSource, string>>(conversion, param);
        }
    }
}
