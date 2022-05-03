using System.Linq.Expressions;

namespace CustomExpressionTrees.Lesson6
{
    // Constant visitor:
    public class ConstantVisitor : SimpleBaseVisitor
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
