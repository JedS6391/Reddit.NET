using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Reddit.NET.Client.UnitTests.Shared
{
    public static class TestExtensions
    {
        public static void SetProperty<TSource, TProperty>(
            this TSource source,
            Expression<Func<TSource, TProperty>> propertyExpression,
            TProperty value)
        {
            var memberInfo = (propertyExpression.Body as MemberExpression).Member;
            var propertyInfo = memberInfo as PropertyInfo;

            propertyInfo.SetValue(source, value);
        }
    }
}