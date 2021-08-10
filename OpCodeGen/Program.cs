using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OpCodeGen
{
    class Program
    {
        private static string opCategory;

        static IEnumerable<OpCode> GenOps()
        {
            // Load from Json
            {
                var json = JObject.Parse(File.ReadAllText("spirv.json"));
                Console.WriteLine(json);

                foreach (var t in json.SelectToken("OpCodes"))
                {
                    // Category 
                    {
                        var category = t.SelectToken("Category").ToString();
                        category = category.Replace(" ", "");
                        category = category.Replace("-", "");
                        opCategory = category;
                    }
                    
                    // Name and Description
                    var name = t.SelectToken("Name").ToString().Substring("Op".Length).Trim();
                    var description = t.SelectToken("DescriptionPlain").ToString();
                    
                    var op = Op(name);

                    op.Cmt(description);

                    // Operands
                    {
                        var operands = t.SelectToken("Operands");

                        foreach (var operand in operands)
                        {
                            var type = operand.SelectToken("Type").ToString();
                            var operandName = operand.SelectToken("Name").ToString();
                            switch (type)
                            {
                                case "ID":
                                    op.Fields.Add(Id(operandName));
                                    break;              
                                case "LiteralNumber":
                                    op.Fields.Add(Nr(operandName));
                                    break;
                                case "LiteralString":
                                    op.Fields.Add(Str(operandName));
                                    break;
                                case "LiteralNumber[]":
                                    op.Fields.Add(NrArray(operandName));
                                    break;
                                case "ID[]":
                                    op.Fields.Add(IdArray(operandName));
                                    break;
                                case "ID?":
                                    op.Fields.Add(IdOpt(operandName));
                                    break;
                                case "Pair<LiteralNumber,ID>[]":
                                    op.Fields.Add(PairArray(Nr("Literal"), Id("Label"), operandName));
                                    break;
                                default:
                                    op.Fields.Add(Typed(type, operandName));
                                    break;
                            }
                        }
                    }

                    // Capabilites
                    {
                        var caps = t.SelectToken("Capabilities");

                        foreach (var cap in caps)
                            op.Compat(cap.ToString());
                    }

                    yield return op;

                }


            }
        }

        class OpField
        {
            public string Type;
            public string Name;
            public string Init;
            public Func<string, string[]> ReadCode;
            public Func<string, string[]> WriteCode;
        }

        class OpCode
        {
            public string Name; // without Op
            public readonly string Cat = opCategory;
            private string comment;

            public List<OpField> Fields;

            private readonly List<string> compatibilities = new List<string>();

            public OpCode Compat(string compat)
            {
                compatibilities.Add(compat);
                return this;
            }

            public OpCode Cmt(string cmt)
            {
                // TODO multiline => generate it with /// correctly
                comment = cmt;
                return this;
            }

            public IEnumerable<string> CreateLines()
            {
                yield return "using System;";
                yield return "using System.Collections.Generic;";
                yield return "using System.Linq;";
                yield return "using System.Text;";
                yield return "using System.Threading.Tasks;";
                yield return "using SpirvNet.Spirv.Enums;";
                yield return "";
                yield return "// This file is auto-generated and should not be modified manually.";
                yield return "";
                yield return "namespace SpirvNet.Spirv.Ops." + Cat;
                yield return "{";
                yield return "    /// <summary>";
                if (string.IsNullOrEmpty(comment))
                    yield return "    /// TODO: Copy comment from https://www.khronos.org/registry/spir-v/specs/1.0/SPIRV.pdf";
                else
                    foreach (var line in comment.Split('\n'))
                        yield return "    /// " + line.Replace("<id>", " ID").Replace("  ID", " ID");
                yield return "    /// </summary>";
                if (compatibilities.Count > 0)
                    yield return string.Format("    [DependsOn({0})]", compatibilities.Select(c => "LanguageCapability." + c).Aggregate((s1, s2) => s1 + " | " + s2));
                yield return string.Format("    public sealed class Op{0} : {1}Instruction", Name, Cat);
                yield return "    {";
                yield return string.Format("        public override bool Is{0} => true;", Cat);
                yield return string.Format("        public override OpCode OpCode => OpCode.{0};", Name);
                if (Fields.Any(f => f.Name == "Result"))
                    yield return string.Format("        public override ID? ResultID => Result;");
                if (Fields.Any(f => f.Name == "ResultType"))
                    yield return string.Format("        public override ID? ResultTypeID => ResultType;");
                if (Fields.Count > 0)
                {
                    yield return "";
                    foreach (var field in Fields)
                        yield return string.Format("        public {0} {1}{2};", field.Type, field.Name, string.IsNullOrEmpty(field.Init) ? "" : " = " + field.Init);
                }
                yield return "";
                yield return "        #region Code";
                yield return string.Format("        public override string ToString() => \"(\" + OpCode + \"(\" + (int)OpCode + \")\"{0} + \")\";", Fields.Count == 0 ? "" : Fields.Select(f => f.Name).Aggregate("", (s1, s2) => string.Format("{0} + \", \" + StrOf({1})", s1, s2)));
                yield return string.Format("        public override string ArgString => {0};", !Fields.Any(f => f.Name != "Result" && f.Name != "ResultType") ? "\"\"" : Fields.Where(f => f.Name != "Result" && f.Name != "ResultType").Select(f => "\"" + f.Name + ": \" + StrOf(" + f.Name + ")").Aggregate((s1, s2) => string.Format("{0} + \", \" + {1}", s1, s2)));

                //public abstract string ArgString { get; }

                yield return "";
                yield return "        protected override void FromCode(uint[] codes, int start)";
                yield return "        {";
                yield return string.Format("            System.Diagnostics.Debug.Assert((codes[start] & 0x0000FFFF) == (uint)OpCode.{0});", Name);
                if (Fields.Count> 0)
                {
                    yield return "            var i = start + 1;";
                    foreach (var field in Fields)
                        foreach (var c in field.ReadCode(field.Name))
                            yield return "            " + c;
                }
                yield return "        }";
                yield return "";
                yield return "        protected override void WriteCode(List<uint> code)";
                yield return "        {";
                if (Fields.Count > 0)
                {
                    foreach (var field in Fields)
                        foreach (var c in field.WriteCode(field.Name))
                            yield return "            " + c;
                }
                else
                    yield return "            // no-op";
                yield return "        }";
                yield return "";
                yield return "        public override IEnumerable<ID> AllIDs";
                yield return "        {";
                yield return "            get";
                yield return "            {";
                var anyID = false;
                foreach (var field in Fields)
                {
                    switch (field.Type)
                    {
                        case "ID":
                            anyID = true;
                            yield return string.Format("                yield return {0};", field.Name);
                            break;
                        case "ID?":
                            anyID = true;
                            yield return string.Format("                if ({0}.HasValue)", field.Name);
                            yield return string.Format("                    yield return {0}.Value;", field.Name);
                            break;
                        case "ID[]":
                            anyID = true;
                            yield return string.Format("                if ({0} != null)", field.Name);
                            yield return string.Format("                    foreach (var id in {0})", field.Name);
                            yield return string.Format("                        yield return id;");
                            break;
                        case "Pair<LiteralNumber, ID>[]":
                            anyID = true;
                            yield return string.Format("                if ({0} != null)", field.Name);
                            yield return string.Format("                    foreach (var p in {0})", field.Name);
                            yield return string.Format("                        yield return p.Second;");
                            break;
                    }
                }
                if (!anyID)
                    yield return "                yield break;";
                yield return "            }";
                yield return "        }";
                yield return "        #endregion";

                yield return "    }";
                yield return "}";
            }
        }

        static OpField Id(string name)
        {
            return new OpField
            {
                Type = "ID",
                Name = name,
                ReadCode = n => new[] { string.Format("{0} = new ID(codes[i++]);", n) },
                WriteCode = n => new[] { string.Format("code.Add({0}.Value);", n) }
            };
        }
        static OpField IdArray(string name)
        {
            return new OpField
            {
                Type = "ID[]",
                Init = "{ }",
                Name = name,
                ReadCode = n => new[]
                {
                    "var length = WordCount - (i - start);",
                    string.Format("{0} = new ID[length];", n),
                    "for (var k = 0; k < length; ++k)",
                    string.Format("    {0}[k] = new ID(codes[i++]);", n),
                },
                WriteCode = n => new[]
                {
                    string.Format("if ({0} != null)", n),
                    string.Format("    foreach (var val in {0})", n),
                    string.Format("        code.Add(val.Value);")
                }
            };
        }
        static OpField IdOpt(string name)
        {
            return new OpField
            {
                Type = "ID?",
                Name = name,
                ReadCode = n => new[]
                {
                    "if (i - start < WordCount)",
                    string.Format("    {0} = new ID(codes[i++]);", n),
                    "else",
                    string.Format("    {0} = null;", n),
                },
                WriteCode = n => new[]
                {
                    string.Format("if ({0}.HasValue)", n),
                    string.Format("    code.Add({0}.Value.Value);", n)
                }
            };
        }
        static OpField Nr(string name)
        {
            return new OpField
            {
                Type = "LiteralNumber",
                Name = name,
                ReadCode = n => new[] { string.Format("{0} = new LiteralNumber(codes[i++]);", n) },
                WriteCode = n => new[] { string.Format("code.Add({0}.Value);", n) }
            };
        }
        static OpField NrArray(string name)
        {
            return new OpField
            {
                Type = "LiteralNumber[]",
                Init = "{ }",
                Name = name,
                ReadCode = n => new[]
                {
                    "var length = WordCount - (i - start);",
                    string.Format("{0} = new LiteralNumber[length];", n),
                    "for (var k = 0; k < length; ++k)",
                    string.Format("    {0}[k] = new LiteralNumber(codes[i++]);", n),
                },
                WriteCode = n => new[]
                {
                    string.Format("if ({0} != null)", n),
                    string.Format("    foreach (var val in {0})", n),
                    string.Format("        code.Add(val.Value);")
                }
            };
        }
        static OpField Str(string name)
        {
            return new OpField
            {
                Type = "LiteralString",
                Name = name,
                ReadCode = n => new[] { string.Format("{0} = LiteralString.FromCode(codes, ref i);", n) },
                WriteCode = n => new[] { string.Format("{0}.WriteCode(code);", n) }
            };
        }
        static OpField PairArray(OpField op1, OpField op2, string name)
        {
            return new OpField
            {
                Type = string.Format("Pair<{0}, {1}>[]", op1.Type, op2.Type),
                Init = "{ }",
                Name = name,
                ReadCode = n => new[]
                {
                    "var length = (WordCount - (i - start)) / 2;",
                    string.Format("{0} = new Pair<{1}, {2}>[length];", n, op1.Type, op2.Type),
                    "for (var k = 0; k < length; ++k)",
                    "{",
                    string.Format("    var f = new {0}(codes[i++]);", op1.Type),
                    string.Format("    var s = new {0}(codes[i++]);", op2.Type),
                    string.Format("    {0}[k] = new Pair<{1}, {2}>(f, s);", n, op1.Type, op2.Type),
                    "}",
                },
                WriteCode = n => new[]
                {
                    string.Format("if ({0} != null)", n),
                    string.Format("    foreach (var val in {0})", n),
                    string.Format("    {{"),
                    string.Format("        code.Add(val.First.Value);"),
                    string.Format("        code.Add(val.Second.Value);"),
                    string.Format("    }}"),
                }
            };
        }

        static OpField Typed(string typeAndName) => Typed(typeAndName, typeAndName);
        static OpField Typed(string type, string name)
        {
            return new OpField
            {
                Type = type,
                Name = name,
                ReadCode = n => new[] { string.Format("{0} = ({1})codes[i++];", n, type) },
                WriteCode = n => new[] { string.Format("code.Add((uint){0});", n) }
            };
        }
        static OpField TypedArray(string typeAndName) => TypedArray(typeAndName, typeAndName);
        static OpField TypedArray(string type, string name)
        {
            return new OpField
            {
                Type = type + "[]",
                Init = "{ }",
                Name = name,
                ReadCode = n => new[]
                {
                    "var length = WordCount - (i - start);",
                    string.Format("{0} = new {1}[length];", n, type),
                    "for (var k = 0; k < length; ++k)",
                    string.Format("    {0}[k] = ({1})codes[i++];", n, type),
                },
                WriteCode = n => new[]
                {
                    string.Format("if ({0} != null)", n),
                    string.Format("    foreach (var val in {0})", n),
                    string.Format("        code.Add((uint)val);")
                }
            };
        }

        static OpCode Op(string name, params OpField[] fields)
        {
            return new OpCode
            {
                Name = name,
                Fields = new List<OpField>(fields)
            };
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: path/to/SpirvNet/Spirv");
                return;
            }

            var path = Path.Combine(args[0], "Ops");
            if (!Directory.Exists(path))
            {
                Console.WriteLine(path + " does not exist. calling from wrong dir?");
                return;
            }

            var cats = new HashSet<string>();

            var ops = GenOps();
            foreach (var op in ops)
            {
                var filename = Path.Combine(path, op.Cat, string.Format("Op{0}.cs", op.Name));
                //new FileInfo(filename).Directory?.Create();

                cats.Add(op.Cat);

                File.WriteAllLines(filename, op.CreateLines());
                Console.WriteLine("Wrote " + filename);
            }

            foreach (var cat in cats)
            {
                var catname = cat + "Instruction";
                var filename = Path.Combine(path, cat, string.Format("{0}.cs", catname));

                var lines = new[]
                {
                    "namespace SpirvNet.Spirv.Ops." + cat,
                    "{",
                    string.Format("    public abstract class {0}Instruction : Instruction", cat),
                    "    {",
                    "        // intentionally empty",
                    "    }",
                    "}"
                };

                File.WriteAllLines(filename, lines);
                Console.WriteLine("Wrote " + filename);
            }
        }
    }
}
