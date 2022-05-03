using System;
using System.Linq.Expressions;

namespace ModifyExpressionTrees.Lesson3
{
    /// <summary>
    /// InterpretingExpressionsTrees
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<int, int>> sum = (x) => x + 100 + 200;

            var visitor = Visitor.CreateFromExpression(sum);

            visitor.Visit("|");

            Console.WriteLine("---------------------------------------");
            Console.ReadKey();
        }

    }


    // Base Visitor class:
    public abstract class Visitor
    {
        private readonly Expression node;

        protected Visitor(Expression node)
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
        public static Visitor CreateFromExpression(Expression node)
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
                    return default(Visitor);
            }
        }
    }

    // Lambda Visitor
    public class LambdaVisitor : Visitor
    {
        private readonly LambdaExpression node;
        public LambdaVisitor(LambdaExpression node) : base(node)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            this.Log($"{prefix}This expression is a {NodeType} expression type");
            this.Log($"{prefix}The name of the lambda is {((node.Name == null) ? "<null>" : node.Name)}");
            this.Log($"{prefix}The return type is {node.ReturnType}");
            this.Log($"{prefix}The expression has {node.Parameters.Count} argument(s). {(node.Parameters.Count > 0 ? "They are:" : "")}");
            // Visit each parameter:
            foreach (var argumentExpression in node.Parameters)
            {
                var argumentVisitor = Visitor.CreateFromExpression(argumentExpression);
                argumentVisitor.Visit(prefix + "\t");
            }
            this.Log($"{prefix}The expression body is:");
            // Visit the body:
            var bodyVisitor = Visitor.CreateFromExpression(node.Body);
            bodyVisitor.Visit(prefix + "\t");
        }
    }

    // Binary Expression Visitor:
    public class BinaryVisitor : Visitor
    {
        private readonly BinaryExpression node;
        public BinaryVisitor(BinaryExpression node) : base(node)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            this.Log($"{prefix}This binary expression is a {NodeType} expression");
            var left = Visitor.CreateFromExpression(node.Left);
            this.Log($"{prefix}The Left argument is:");
            left.Visit(prefix + "\t");
            var right = Visitor.CreateFromExpression(node.Right);
            this.Log($"{prefix}The Right argument is:");
            right.Visit(prefix + "\t");
        }
    }

    // Parameter visitor:
    public class ParameterVisitor : Visitor
    {
        private readonly ParameterExpression node;
        public ParameterVisitor(ParameterExpression node) : base(node)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            this.Log($"{prefix}This is an {NodeType} expression type");
            this.Log($"{prefix}Type: {node.Type.ToString()}, Name: {node.Name}, ByRef: {node.IsByRef}");
        }
    }

    // Constant visitor:
    public class ConstantVisitor : Visitor
    {
        private readonly ConstantExpression node;
        public ConstantVisitor(ConstantExpression node) : base(node)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            this.Log($"{prefix}This is an {NodeType} expression type");
            this.Log($"{prefix}The type of the constant value is {node.Type}");
            this.Log($"{prefix}The value of the constant value is {node.Value}");
        }
    }
}
