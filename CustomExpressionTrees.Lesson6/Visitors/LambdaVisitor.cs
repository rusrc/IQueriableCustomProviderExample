using System.Linq.Expressions;

namespace CustomExpressionTrees.Lesson6
{
    public class LambdaVisitor : SimpleBaseVisitor
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
            this.Log($"{prefix}The expression has {node.Parameters.Count} argument(s). They are:");
            // Visit each parameter:
            foreach (var argumentExpression in node.Parameters)
            {
                var argumentVisitor = SimpleBaseVisitor.CreateFromExpression(argumentExpression);
                argumentVisitor.Visit(prefix + "\t");
            }
            this.Log($"{prefix}The expression body is:");
            // Visit the body:
            var bodyVisitor = SimpleBaseVisitor.CreateFromExpression(node.Body);
            bodyVisitor.Visit(prefix + "\t");
        }
    }
}
