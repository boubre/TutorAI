using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GeometryTutorLib.FirstOrderLogic
{    
    class Test
    {
        static void Main(string[] args)
        {
            ASTNode ast = Parser.ParseText("InMiddle(M, Segment(A, B)) -> Length(Segment(A, M)) + Length(Segment(M, B)) = Length(Segment(A, B))");
            Console.WriteLine(ast.Unparse());

/* Output:
   ->
      InMiddle
         M
         Segment
            A
            B
      =
         +
            Length
               Segment
                  A
                  M
            Length
               Segment
                  M
                  B
         Length
            Segment
               A
               B
 */

        }
    }

}
