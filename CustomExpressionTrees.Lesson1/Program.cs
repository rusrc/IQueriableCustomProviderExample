using System;
using System.Linq.Expressions;

namespace CustomExpressionTrees.Lesson1
{
    /// <summary>
    /// ExpressionTreesApp
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var func1 = new Func<int, int>(x => x + 100);

            Expression<Func<int, int>> func2 = x => x + 100;

            #region Expression tree func3
            var parametersExp = Expression.Parameter(typeof(int), "x");
            var constNumberExp = Expression.Constant(100, typeof(int));
            var leftPlusRightBodyExp = Expression.Add(parametersExp, constNumberExp);
            var lambdaExp = Expression.Lambda<Func<int, int>>(leftPlusRightBodyExp, parametersExp);

            var func3 = lambdaExp.Compile();
            #endregion


            Console.WriteLine(func1(200));
            Console.WriteLine(func2.Compile().Invoke(200));
            Console.WriteLine(func3(200));
            Console.ReadKey();
        }
    }

}
