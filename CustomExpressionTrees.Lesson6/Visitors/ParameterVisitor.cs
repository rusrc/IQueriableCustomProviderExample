using System.Linq.Expressions;

namespace CustomExpressionTrees.Lesson6
{
    // Parameter visitor:
    public class ParameterVisitor : SimpleBaseVisitor
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
}
