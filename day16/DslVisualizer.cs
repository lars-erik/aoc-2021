using System;
using System.Linq;
using System.Text;

namespace day16
{
    public class DslVisualizer
    {
        private const int Spacing = 2;

        public static void VisualizeDsl(Node node, StringBuilder builder, int indent = 0)
        {
            var indentStr = new String(' ', indent);
            builder.Append(indentStr);
            if (node is BinaryOperation binOp)
            {
                builder.Append(node.GetType().Name);
                builder.Append("(");
                if (binOp.Left is Literal && binOp.Right is Literal)
                {
                    builder.Append(binOp.Left.Evaluate());
                    builder.Append(", ");
                    builder.Append(binOp.Right.Evaluate());
                }
                else
                {
                    builder.AppendLine();
                    VisualizeDsl(binOp.Left, builder, indent + Spacing);
                    builder.AppendLine(", ");
                    VisualizeDsl(binOp.Right, builder, indent + Spacing);
                    builder.AppendLine();
                    builder.Append(indentStr);
                }
                builder.Append($")");
            }
            else if (node is CompositeOperation compOp)
            {
                builder.Append(node.GetType().Name);
                builder.Append("(");
                if (compOp.Children.All(x => x is Literal))
                {
                    builder.Append(String.Join(", ", compOp.Children.Select(x => x.Evaluate())));
                }
                else if (compOp.Children.Length == 1)
                {
                    VisualizeDsl(compOp.Children[0], builder);
                }
                else
                {
                    for (var i = 0; i < compOp.Children.Length; i++)
                    {
                        builder.AppendLine();
                        VisualizeDsl(compOp.Children[i], builder, indent + Spacing);
                        if (i < compOp.Children.Length - 1)
                            builder.Append(", ");
                    }
                    builder.AppendLine();
                    builder.Append(indentStr);
                }
                builder.Append(")");
            }
            else // Literal
            {
                builder.Append(node.Evaluate());
            }
        }
    }
}