using System;
using System.Collections.Generic;
using System.Text;
using SyntacticAnalyzer;

namespace MindMapMeaningRepresentation
{
    public class MRHelperClass
    {
        public static bool CheckRule(ParseNode node, params string[] ruleDefinition)
        {
            if (node.Children.Count == ruleDefinition.Length)
            {
                bool matched = true;
                for (int i = 0; i < node.Children.Count; i++)
                {
                    if (((ParseNode)node.Children[i]).Goal != ruleDefinition[i])
                    {
                        matched = false;
                        break;
                    }
                }
                return matched;
            }
            else
                return false;
        }

        public static void TestChekRuleFunction()
        {
            ParseNode node = new ParseNode();
            node.Goal = "Goal1";

            node.Children = new System.Collections.ArrayList();
            node.Children.Add(new ParseNode());
            node.Children.Add(new ParseNode());
            node.Children.Add(new ParseNode());

            ((ParseNode)node.Children[0]).Goal = "P1";
            ((ParseNode)node.Children[1]).Goal = "P2";
            ((ParseNode)node.Children[2]).Goal = "P3";

            
            bool b1 =(CheckRule(node, "P1", "P2","P3"));
            bool b2 = (CheckRule(node, "P1", "P2"));

 
        }
    }
}
