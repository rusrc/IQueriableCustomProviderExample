using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CustomExpressionTrees.Lesson4
{
    /// <summary>
    /// InterpretingExpressionsTrees
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            new MyQueryableCollection<int>(new MyQueryProvider()).Where(x => x == 1);


            Console.WriteLine("---------------------------------------");
            Console.ReadKey();
        }

    }

    public class MyQueryableCollection<T> : IQueryable, IQueryable<T>
    {
        IQueryProvider queryProvider;
        Expression _expression;
        public MyQueryableCollection(IQueryProvider provider)
        {
            queryProvider = provider;
            _expression = Expression.Constant(this);
        }

        public Type ElementType => throw new NotImplementedException();

        public Expression Expression => _expression;

        public IQueryProvider Provider => queryProvider;

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class MyQueryProvider : IQueryProvider
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
