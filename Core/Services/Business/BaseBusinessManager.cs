using System;
using System.Linq.Expressions;

namespace Core.Services.Business {
    public abstract class BaseBusinessManager {
        //public static Expression<Func<TSource, string>> GetExpression<TSource>(string propertyName) {
        //    var param = Expression.Parameter(typeof(TSource), "x");
        //    var property = Expression.Property(param, propertyName);
        //    Expression conversion = Expression.Convert(property, property.Type);
        //    return Expression.Lambda<Func<TSource, string>>(conversion, param);
        //}

        public static Expression<Func<TSource, dynamic>> GetExpression<TSource>(string propertyName) {
            var param = Expression.Parameter(typeof(TSource), "x");
            var property = Expression.Property(param, propertyName);
            Expression conversion = Expression.Convert(property, property.Type);
            return Expression.Lambda<Func<TSource, dynamic>>(conversion, param);
        }
    }
}
