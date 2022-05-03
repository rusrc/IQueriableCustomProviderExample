using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CustomExpressionTrees.Lesson5
{
    /// <summary>
    ///  Custom Linq Provider
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            new MyQueryableCollection<User>(new MyProvider())
                .Where(u => u.Id == 1);

            Console.WriteLine("-----------");
        }
    }

    public class User { 
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    /// <summary>
    /// https://github.com/microsoft/referencesource/blob/master/System.Core/System/Linq/IQueryable.cs
    /// </summary>
    public static class ProgramExtension
    {
        public static IQueryable<TSource> Where2<TSource>(this IQueryable<TSource> sourceCollection, Expression<Func<TSource, int>> predicateExpression)
        {
            var regularMethod = ((MethodInfo)MethodBase.GetCurrentMethod());
            var genericMethod = regularMethod.MakeGenericMethod(typeof(TSource));
            // sourceCollection.Expression - always constant, usually collection
            var arguments = new Expression[] { sourceCollection.Expression, Expression.Quote(predicateExpression) };

            MethodCallExpression callExpression = Expression.Call(
                  instance: null,
                  method: genericMethod,
                  arguments: arguments
                  );

            // TODO this.provider.CreateQuery<TSource>(callExpression);
            return (IQueryable<TSource>)new List<TSource>(); // callExpression.;
        }
    }

    public class MyQueryableCollection<T> : IQueryable<T>
    {
        public MyQueryableCollection(IQueryProvider provider)
        {
            this.Provider = provider;
        }

        // strSQL += SELECT
        // strSQL += *
        // strSQL += FROM
        // strSQL += Tbl
        // strSQL = SELECT * FROM Tbl

        public Type ElementType => throw new NotImplementedException();

        public Expression Expression => Expression.Constant(this);

        public IQueryProvider Provider { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class MyProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }

}
