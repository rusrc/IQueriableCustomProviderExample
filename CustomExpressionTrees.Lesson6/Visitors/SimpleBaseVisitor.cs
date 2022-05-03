using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CustomExpressionTrees.Lesson6
{
    public abstract class SimpleBaseVisitor : ExpressionVisitor
    {
        private StringBuilder _sb;

        private QueryTranslator _takeTranslator;
        private QueryTranslator _selectTranslator;
        private QueryTranslator _whereTranslator;
        private QueryTranslator _orderByTranslator;

        private readonly Expression node;

        protected SimpleBaseVisitor(Expression node)
        {
            this.node = node;
        }

        public abstract void Visit(string prefix);
        public virtual void Log(string msg)
        {
            Console.WriteLine(msg);
            // System.Diagnostics.Debug.WriteLine(msg);
        }

        public ExpressionType NodeType => this.node.NodeType;
        public static SimpleBaseVisitor CreateFromExpression(Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Constant:
                    return new ConstantVisitor((ConstantExpression)node);
                case ExpressionType.Lambda:
                    return new LambdaVisitor((LambdaExpression)node);
                case ExpressionType.Parameter:
                    return new ParameterVisitor((ParameterExpression)node);
                case ExpressionType.Add:
                    return new BinaryVisitor((BinaryExpression)node);
                default:
                    Console.Error.WriteLine($"Node not processed yet: {node.NodeType}");
                    return default(SimpleBaseVisitor);
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Type declaringType = node.Method.DeclaringType;
            if (declaringType != typeof(Queryable))
                throw new NotSupportedException(
                  "The type for the query operator is not Queryable!");
            switch (node.Method.Name)
            {
                case "Where":
                    // is this really a proper Where?
                    var whereLambda = GetLambdaWithParamCheck(node);
                    if (whereLambda == null)
                        break;
                    VisitWhere(node.Arguments[0], whereLambda);
                    break;
                case "OrderBy":
                case "ThenBy":
                    // is this really a proper Order By?
                    var orderLambda = GetLambdaWithParamCheck(node);
                    if (orderLambda == null)
                        break;
                    // VisitOrderBy(node.Arguments[0], orderLambda, OrderDirection.Ascending);
                    break;
                case "OrderByDescending":
                case "ThenByDescending":
                    // is this really a proper Order By Descending?
                    var orderDescLambda = GetLambdaWithParamCheck(node);
                    if (orderDescLambda == null)
                        break;
                    //VisitOrderBy(node.Arguments[0], orderDescLambda, OrderDirection.Descending);
                    break;
                case "Select":
                    // is this really a proper Select?
                    var selectLambda = GetLambdaWithParamCheck(node);
                    if (selectLambda == null)
                        break;
                    //VisitSelect(node.Arguments[0], selectLambda);
                    break;
                case "Take":
                    if (node.Arguments.Count != 2)
                        break;
                    //VisitTake(node.Arguments[0], node.Arguments[1]);
                    break;
                case "First":
                    // This custom provider does not support the use of a First operator
                    // that takes a predicate. Therefore we check to ensure that no more
                    // than one argument is provided.
                    if (node.Arguments.Count != 1)
                        break;
                    //VisitFirst(node.Arguments[0], false);
                    break;
                case "FirstOrDefault":
                    // This custom provider does not support the use of a FirstOrDefault
                    // operator that takes a predicate. Therefore we check to ensure that
                    // no more than one argument is provided.
                    if (node.Arguments.Count != 1)
                        break;
                    //VisitFirst(node.Arguments[0], true);
                    break;
                default:
                    return base.VisitMethodCall(node);
            }
            Visit2(node.Arguments[0]);
            return node;

            throw new NotImplementedException();
        }

        private void VisitWhere(Expression queryable, LambdaExpression predicate)
        {
            // this custom provider cannot support more
            // than one Where query operator in a LINQ query

            if (_whereTranslator != null)
                throw new NotSupportedException(
                   "You cannot have more than one Where operator in this expression");
            // _whereTranslator = new WhereTranslator(_model);
            _whereTranslator.Translate(predicate);

            throw new NotImplementedException();
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            _sb.Append("(");
            Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _sb.Append(" AND ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _sb.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    if (true /* IsComparingWithNull(b)*/)
                        _sb.Append(" IS ");
                    else
                        _sb.Append(" = ");
                    break;
                case ExpressionType.GreaterThan:
                    _sb.Append(" > ");
                    break;
                    // ...
            }
            Visit(b.Right);
            _sb.Append(")");
            return b;
        }

        private LambdaExpression GetLambdaWithParamCheck(Expression node)
        {
            throw new NotImplementedException();
        }

        public static Expression Visit2(Expression expression)
        {
            if (expression == null)
            {
                return expression;
            }
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                // return this.VisitBinary((BinaryExpression)expression);
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                //return this.VisitUnary((UnaryExpression)expression);
                case ExpressionType.Call:
                //return this.VisitMethodCall((MethodCallExpression)expression);
                case ExpressionType.Conditional:
                //return this.VisitConditional((ConditionalExpression)expression);
                case ExpressionType.Constant:
                //return this.VisitConstant((ConstantExpression)expression);
                case ExpressionType.Invoke:
                //return this.VisitInvocation((InvocationExpression)expression);
                case ExpressionType.Lambda:
                //return this.VisitLambda((LambdaExpression)expression);
                case ExpressionType.ListInit:
                //return this.VisitListInit((ListInitExpression)expression);
                case ExpressionType.MemberAccess:
                //return this.VisitMemberAccess((MemberExpression)expression);
                case ExpressionType.MemberInit:
                //return this.VisitMemberInit((MemberInitExpression)expression);
                case ExpressionType.New:
                //return this.VisitNew((NewExpression)expression);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                //return this.VisitNewArray((NewArrayExpression)expression);
                case ExpressionType.Parameter:
                //return this.VisitParameter((ParameterExpression)expression);
                case ExpressionType.TypeIs:
                //return this.VisitTypeIs((TypeBinaryExpression)expression);
                default:
                    throw new Exception(
                      string.Format("Unhandled expression type: '{0}'", expression.NodeType));
            }
        }

    }
}
