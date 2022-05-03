using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CustomExpressionTrees.Lesson6
{
    // 1. Create implementation of IQueryable<T>
    // 2. Create implementation of IQueryProvider<T>
    // 3. Create GetEnumerator() that returns IEnumerator<T>

    public class SomeClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var myProvider = new MyProvider<SomeClass>();
            var queryable = new MyCollection<SomeClass>(myProvider);

            var query = queryable.Where(x => x.Id == 1);
            query = query.Where(x => x.Name == "Test");


            var result = query.ToList();

            Console.WriteLine(result);
            Console.WriteLine("___________");
            Console.ReadKey();
        }
    }

    
    public static class MyCollectionExtesion
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

    public class MyCollection<TSomeThing> 
        : IQueryable<TSomeThing>, IQueryable, IEnumerable<TSomeThing>, IEnumerable
    {
        private IQueryProvider _provider;
        private Expression _expression;

        public MyCollection(IQueryProvider provider)
        {
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public MyCollection(IQueryProvider provider, Expression expression)
        {
            _provider = provider;
            _expression = expression;
        }

        public Type ElementType => typeof(TSomeThing); // get only

        public Expression Expression => this._expression; // 2 step

        // 1 step
        public IQueryProvider Provider => _provider;

        public IEnumerator<TSomeThing> GetEnumerator()
        {
            return ((IEnumerable<TSomeThing>)_provider.Execute(_expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();
        }
    }

    public class MyProvider<TEntity> : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (
                  IQueryable)Activator.CreateInstance(typeof(MyCollection<>).MakeGenericType(
                    elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {

            
            return new MyCollection<TElement>(this, expression);
        }

        // ToList, ToSingle
        public object Execute(Expression expression)
        {
            // translate
            var queryText = this.GetTranslatedQueryText(expression);
            // execute
            return Execute(queryText);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            // translate
            var queryText = this.GetTranslatedQueryText(expression);
            // execute
            return (TResult)Execute(queryText);
        }

        private object Execute(string queryText)
        {
            return new List<SomeClass>() {
                new SomeClass(),
                new SomeClass(),
                new SomeClass()
            };

            // get a database connection
            DbConnection connection = null; // As example
            try
            {
                // execute the query
                DbCommand command = connection.CreateCommand();
                command.CommandText = queryText;

                DbDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult);

                // map the database rows into objects (entities)
                // ...

                // return the mapped entities
                // ...
                return new { }; // Let's say we return 
            }
            finally
            {
                connection.Close();
            }
        }

        private string GetTranslatedQueryText(Expression expression)
        {
            return new QueryTranslator().Translate(expression);
        }
    }

    internal class QueryTranslator : ExpressionVisitor
    {
        StringBuilder sb;

        internal QueryTranslator() { }

        internal string Translate(Expression expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);
            return this.sb.ToString();
        }

        /// <summary>
        /// Задача – вырезать любые узлы ExpressionType.Quote из аргументов метода
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Where" && m.Method.DeclaringType == typeof(Queryable))
            {
                sb.Append("SELECT * FROM (");
                this.Visit(m.Arguments[0]);
                sb.Append(") AS T WHERE ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                return m;
            }

            throw new NotSupportedException($"The method '{m.Method.Name}' is not supported");
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format(
                      "The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");
            this.Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    sb.Append(" AND ");
                    break;
                case ExpressionType.Or:
                    sb.Append(" OR");
                    break;
                case ExpressionType.Equal:
                    sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(
                      string.Format(
                        "The binary operator '{0}' is not supported", b.NodeType));
            }
            this.Visit(b.Right);
            sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;
            if (q != null)
            {
                // assume constant nodes w/ IQueryables are table references
                sb.Append("SELECT * FROM ");
                sb.Append(q.ElementType.Name);
            }
            else if (c.Value == null)
            {
                sb.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)c.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(
                          string.Format(
                          "The constant for '{0}' is not supported", c.Value));
                    default:
                        sb.Append(c.Value);
                        break;
                }
            }
            return c;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null
              && m.Expression.NodeType == ExpressionType.Parameter)
            {
                sb.Append(m.Member.Name);
                return m;
            }
            throw new NotSupportedException(
              string.Format("The member '{0}' is not supported", m.Member.Name));
        }
    }

    internal static class TypeSystem
    {
        internal static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }
        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }
    }
}
