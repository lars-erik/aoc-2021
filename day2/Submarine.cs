using System;
using System.Collections.Generic;

namespace day2
{
    public class Submarine
    {
        public static readonly Dictionary<string, Func<(int pos, int depth, int aim), int, (int pos, int depth, int aim)>> SimpleStrategies = new()
        {
            { "forward", (l, x) =>
                {
                    l.pos += x;
                    return l;
                }
            },
            { "up", (l, x) =>
                {
                    l.depth -= x;
                    return l;
                }
            },
            { "down", (l, x) =>
                {
                    l.depth += x;
                    return l;
                }
            }
        };

        public static readonly Dictionary<string, Func<(int pos, int depth, int aim), int, (int pos, int depth, int aim)>> ComplexInstructions = new Dictionary<string, Func<(int pos, int depth, int aim), int, (int pos, int depth, int aim)>> {
            { 
                "forward", 
                (s, x) => {
                    s.pos += x;
                    s.depth += x * s.aim;
                    return s;
                } 
            },
            { "up", (s, x) =>
                {
                    s.aim -= x;
                    return s;
                }
            },
            { "down", (s, x) =>
                {
                    s.aim += x;
                    return s;
                }
            }
        };

        private Dictionary<string, Func<(int pos, int depth, int aim), int, (int pos, int depth, int aim)>> strategies;

        public Submarine(Dictionary<string, Func<(int pos, int depth, int aim), int, (int pos, int depth, int aim)>> strategies)
        {
            this.strategies = strategies;
        }

        public (int pos, int depth) ExecuteInstructions(
            (string instruction, int length)[] instructions
            )
        {
            var status = (pos: 0, depth: 0, aim: 0);
            foreach (var instr in instructions)
            {
                status = ExecuteInstruction(instr, status);
            }

            return (status.pos, status.depth);
        }

        public (int pos, int depth, int aim) ExecuteInstruction((string instruction, int length) instr, (int pos, int depth, int aim) status)
        {
            return strategies[instr.instruction](status, instr.length);
        }
    }
}