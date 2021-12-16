using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day16
{
    public record NodeId(int Version, int TypeId);

    public abstract record Node(NodeId Id)
    {
        public abstract long Evaluate();
        public abstract int VersionSum();
    }

    public record Literal(NodeId Id, long Value) : Node(Id)
    {
        public override long Evaluate() => Value;
        public override int VersionSum() => Id.Version;
    };

    public abstract record BinaryOperation(NodeId Id, Node Left, Node Right) : Node(Id)
    {
        public override int VersionSum() => Id.Version + Left.VersionSum() + Right.VersionSum();
    }

    public abstract record CompositeOperation(NodeId Id, Node[] Children) : Node(Id)
    {
        public override int VersionSum() => Id.Version + Children.Sum(x => x.VersionSum());
    }

    public record Sum(NodeId Id, Node[] Children) : CompositeOperation(Id, Children)
    {
        public override long Evaluate() => Children.Sum(x => x.Evaluate());
    }

    public record Product(NodeId Id, Node[] Children) : CompositeOperation(Id, Children)
    {
        public override long Evaluate() => Children.Aggregate((long)1, (prev, cur) => prev * cur.Evaluate());
    }

    public record Minimum(NodeId Id, Node[] Children) : CompositeOperation(Id, Children)
    {
        public override long Evaluate() => Children.Min(x => x.Evaluate());
    }

    public record Maximum(NodeId Id, Node[] Children) : CompositeOperation(Id, Children)
    {
        public override long Evaluate() => Children.Max(x => x.Evaluate());
    }

    public record GreaterThan(NodeId Id, Node Left, Node Right) : BinaryOperation(Id, Left, Right)
    {
        public override long Evaluate() => Left.Evaluate() > Right.Evaluate() ? 1 : 0;
    }

    public record LessThan(NodeId Id, Node Left, Node Right) : BinaryOperation(Id, Left, Right)
    {
        public override long Evaluate() => Left.Evaluate() < Right.Evaluate() ? 1 : 0;
    }

    public record Equal(NodeId Id, Node Left, Node Right) : BinaryOperation(Id, Left, Right)
    {
        public override long Evaluate() => Left.Evaluate() == Right.Evaluate() ? 1 : 0;
    }
}
