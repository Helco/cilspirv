using System;
using System.Collections.Generic;
using cilspirv.SourceGen.CoreGrammar;

namespace cilspirv.SourceGen.CoreGrammar
{
    public class Rootobject
    {
        public string[] copyright { get; set; }
        public string magic_number { get; set; }
        public int major_version { get; set; }
        public int minor_version { get; set; }
        public int revision { get; set; }
        public Instruction_Printing_Class[] instruction_printing_class { get; set; }
        public Instruction[] instructions { get; set; }
        public Operand_Kinds[] operand_kinds { get; set; }
    }

    public class Instruction_Printing_Class
    {
        public string tag { get; set; }
        public string heading { get; set; }
    }

    public class Instruction
    {
        public string opname { get; set; }
        public string @class { get; set; }
        public int opcode { get; set; }
        public Operand[] operands { get; set; }
        public string[] capabilities { get; set; }
        public string lastVersion { get; set; }
        public string version { get; set; }
        public string[] extensions { get; set; }
    }

    public class Operand
    {
        public string kind { get; set; }
        public string name { get; set; }
        public string quantifier { get; set; }
    }

    public class Operand_Kinds
    {
        public string category { get; set; }
        public string kind { get; set; }
        public Enumerant[] enumerants { get; set; }
        public string doc { get; set; }
        public string[] bases { get; set; }
    }

    public class Enumerant
    {
        public string enumerant { get; set; }
        public object value { get; set; }
        public string[] capabilities { get; set; }
        public Parameter[] parameters { get; set; }
        public string version { get; set; }
        public string[] extensions { get; set; }
        public string lastVersion { get; set; }
    }

    public class Parameter
    {
        public string kind { get; set; }
        public string name { get; set; }
        public string quantifier { get; set; }
    }

}
