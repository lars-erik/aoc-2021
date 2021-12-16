using System;
using System.Text;

namespace day16
{
    public class SyntaxTreeAsciiVisualizer
    {
        private const int Spacing = 2;

        public static void PrintNode(Node node, StringBuilder builder, bool drawMeta = true, string prefix = "", int indent = 0)
        {
            string indentStr = "";
            if (indent > 0)
            { 
                indentStr = new String(' ', indent - Spacing)
                            + "+"
                            + new String('-', Spacing - 1);
            }
            builder.Append($"{indentStr}{prefix} ");
            if (drawMeta || node.Id.TypeId != 4) builder.Append($"{node.GetType().Name} ");
            if (drawMeta) builder.Append($"V:{node.Id.Version} ");
            if (drawMeta) builder.Append("= ");
            builder.AppendLine($"{node.Evaluate()}");

            var childIndent = indent + Spacing;
            if (node is BinaryOperation binOp)
            {

                PrintNode(binOp.Left, builder, drawMeta, "A:", childIndent);
                PrintNode(binOp.Right, builder, drawMeta, "B:", childIndent);
            }
            else if (node is CompositeOperation compOp)
            {

                for (var index = 0; index < compOp.Children.Length; index++)
                {
                    var child = compOp.Children[index];
                    PrintNode(child, builder, drawMeta, $"{index}:", childIndent);
                }
            }
        }
    }
}