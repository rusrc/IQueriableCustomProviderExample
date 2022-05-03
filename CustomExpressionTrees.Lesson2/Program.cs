using System;
using System.Linq.Expressions;

namespace CustomExpressionTrees.Lesson2
{
    /// <summary>
    /// ParsingExpressionTrees
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<int, int>> func2 = x => x + 100;

            // Decompose the expression tree.  
            ParameterExpression param = (ParameterExpression)func2.Parameters[0];
            BinaryExpression operation = (BinaryExpression)func2.Body;
            ParameterExpression left = (ParameterExpression)operation.Left;
            ConstantExpression right = (ConstantExpression)operation.Right;

            Console.WriteLine("Decomposed expression: {0} => {1} {2} {3}",
                              param.Name, left.Name, operation.NodeType, right.Value);

            Console.ReadKey();
        }
    }
}
