using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GeometryTutorLib.FirstOrderLogic
{
    class Parser
    {
        private struct strOperators
        {
            public string name;
            public int precedence;
        };

        private static strOperators[] ops =
            new strOperators[]{
                new strOperators() { name = "->", precedence = 1 },
                new strOperators() { name = "=", precedence = 2 },
                new strOperators() { name = ">", precedence = 2 },
                new strOperators() { name = "<", precedence = 2 },
                new strOperators() { name = ">=", precedence = 2 },
                new strOperators() { name = "<=", precedence = 2 },
                new strOperators() { name = "+", precedence = 3 },
                new strOperators() { name = "-", precedence = 3 },
                new strOperators() { name = "*", precedence = 4 },
                new strOperators() { name = "/", precedence = 4 },
                new strOperators() { name = "^", precedence = 5 },
            };

        private static bool IsOperator(string text)
        {
            foreach (strOperators op in ops)
            {
                if (op.name == text)
                {
                    return true;
                }
            }
            return false;
        }
        private static int OperatorPriority(string text)
        {
            foreach (strOperators op in ops)
            {
                if (op.name == text)
                {
                    return op.precedence;
                }
            }
            return 0;
        }
        public static void SplitCorrections(ref string op1, ref string op2)
        {
            //some corrections after splitting:
            if (op1 == "-" && op2 == ">")
            {
                op1 = "->";
                op2 = "";
            }
            else if (op1 == "<" && op2 == "=")
            {
                op1 = "<=";
                op2 = "";
            }
            else if (op1 == ">" && op2 == "=")
            {
                op1 = ">=";
                op2 = "";
            }

        }
        public static ASTNode ParseText(string text)
        {
            //remove spaces
            text = Regex.Replace(text, @" ", @"");

            //split before & after ( , ) - + * / = > <
            string[] splitstr = Regex.Split(text, @"(?=[(,)-+*/=><])|(?<=[(,)-+*/=><])");

            //string lastElement = "";

            ASTNode root = new ASTNode("");
            ASTNode parent = root;

            for (int i = 0; i < splitstr.Length; i++)
            {
                if (i < splitstr.Length - 1)
                    SplitCorrections(ref splitstr[i], ref splitstr[i + 1]);

                //main parser:
                if (splitstr[i] == "(")
                {
                    parent = parent.GetLastChild();
                }
                else if (splitstr[i] == "," || splitstr[i] == "")
                {
                }
                else if (splitstr[i] == ")")
                {
                    parent = parent.GetParent();
                }
                else if (IsOperator(splitstr[i]))
                {
                    if (OperatorPriority(splitstr[i]) > OperatorPriority(parent.GetName()))
                    {
                        ASTNode opnode = new ASTNode(splitstr[i], parent);
                        ASTNode lastnode = parent.GetLastChild();
                        lastnode.SetParent(opnode); //change the parent to the operator
                        parent.AddChild(opnode);
                        parent = opnode;
                    }
                    else
                    {
                        ASTNode grandparent = parent.GetParent();
                        ASTNode opnode = new ASTNode(splitstr[i], grandparent);
                        parent.SetParent(opnode); //change the parent to the operator
                        grandparent.AddChild(opnode);
                        parent = opnode;
                    }
                }
                else
                {
                    ASTNode node = new ASTNode(splitstr[i], parent);
                    parent.AddChild(node);
                }
            }
            return root;
        }
    }
}