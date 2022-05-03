using System.Linq.Expressions;

namespace CustomExpressionTrees.Lesson6
{
    // Binary Expression Visitor:
    public class BinaryVisitor : SimpleBaseVisitor
    {
        private readonly BinaryExpression node;
        public BinaryVisitor(BinaryExpression node) : base(node)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            this.Log($"{prefix}This binary expression is a {NodeType} expression");
            var left = SimpleBaseVisitor.CreateFromExpression(node.Left);
            this.Log($"{prefix}The Left argument is:");
            left.Visit(prefix + "\t");
            var right = SimpleBaseVisitor.CreateFromExpression(node.Right);
            this.Log($"{prefix}The Right argument is:");
            right.Visit(prefix + "\t");
        }
    }
}
