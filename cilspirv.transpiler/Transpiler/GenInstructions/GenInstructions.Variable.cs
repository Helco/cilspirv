using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using cilspirv.Spirv;
using cilspirv.Spirv.Ops;

using ILInstruction = Mono.Cecil.Cil.Instruction;
using SpirvInstruction = cilspirv.Spirv.Instruction;
using cilspirv.Transpiler.Declarations;

namespace cilspirv.Transpiler
{
    partial class GenInstructions
    {
        private class ValueContext : ITranspilerValueContext
        {
            private readonly GenInstructions gen;
            private readonly StackEntry? parent;
            private StackEntry? result;

            public TranspilerLibrary Library => gen.Library;
            public Module Module => gen.Module;
            public Function Function => gen.Function;
            public TranspilerOptions Options => gen.Options;
            public StackEntry Parent => parent ?? throw new InvalidOperationException("Current context has no parent value");

            public StackEntry Result
            {
                get => result ?? throw new InvalidOperationException("Result of field access was not set");
                private set => result = value;
            }
            StackEntry ITranspilerValueContext.Result
            {
                set
                {
                    if (result != null)
                        throw new InvalidOperationException("Result stack entry was already set");
                    Result = value;
                }
            }

            public ValueContext(GenInstructions gen, StackEntry parent) =>
                (this.gen, this.parent) = (gen, parent);

            public ValueContext(GenInstructions gen) => this.gen = gen;

            public ID CreateID() => gen.context.CreateID();
            public ID CreateIDFor(IInstructionGeneratable generatable) => gen.context.CreateIDFor(generatable);
            public ID IDOf(IInstructionGeneratable generatable) => gen.context.IDOf(generatable);
            public IEnumerable<T> OfType<T>() where T : IInstructionGeneratable => gen.context.OfType<T>();
        }

        private void LoadValueAddress(StackEntry? parent, ITranspilerValueBehaviour behaviour)
        {
            var context = parent == null
                ? new ValueContext(this)
                : new ValueContext(this, parent);
            var instructions = behaviour.LoadAddress(context)?.ToArray()
                ?? throw new InvalidOperationException("Either Load or LoadAddress have to be defined");
            if (context.Result is not ValueStackEntry resultValue)
                throw new InvalidOperationException("LoadAddress did not result in a value");
            if (resultValue.Type is not SpirvPointerType)
                throw new InvalidOperationException("LoadAddress did not result in a pointer");
            Block.Instructions.AddRange(instructions);
            Stack.Add(resultValue);
        }

        private void LoadValue(StackEntry? parent, ITranspilerValueBehaviour behaviour)
        {
            var context = parent == null
                ? new ValueContext(this)
                : new ValueContext(this, parent);
            var instructions = behaviour.Load(context)?.ToArray();
            if (instructions != null)
            {
                Block.Instructions.AddRange(instructions);
                Stack.Add(context.Result);
                return;
            }

            LoadValueAddress(parent, behaviour);
            var pointer = (ValueStackEntry)Pop();
            var pointerType = (SpirvPointerType)pointer.Type;
            var resultId = context.CreateID();
            Add(new OpLoad()
            {
                Result = resultId,
                ResultType = context.IDOf(pointerType.Type!),
                Pointer = pointer.ID
            });
            Stack.Add(new ValueStackEntry(behaviour, resultId, pointerType.Type!));
        }

        private void StoreValue(StackEntry? parent, ValueStackEntry value, ITranspilerValueBehaviour behaviour)
        {
            var context = parent == null
                ? new ValueContext(this)
                : new ValueContext(this, parent);
            var instructions = behaviour.Store(context, value)?.ToArray();
            if (instructions != null)
            {
                Block.Instructions.AddRange(instructions);
                return;
            }

            LoadValueAddress(parent, behaviour);
            var pointer = (ValueStackEntry)Pop();
            Add(new OpStore()
            {
                Pointer = pointer.ID,
                Object = value.ID
            });
        }

        private void LoadArgumentAddress(int argI)
        {
            if (argI < 0)
            {
                throw new NotSupportedException("Do we need address of this?");
            }

            var behaviour = Library.MapParameter(ILMethod.Parameters[argI], Function);
            LoadValueAddress(parent: null, behaviour);
        }

        private void LoadArgument(int argI)
        {
            if (argI < 0)
            {
                Stack.Add(new StackEntry(new ThisModuleTag()));
                return;
            }

            var behaviour = Library.MapParameter(ILMethod.Parameters[argI], Function);
            LoadValue(parent: null, behaviour);
        }

        private void StoreArgument(int argI)
        {
            var value = Pop() as ValueStackEntry ?? throw new InvalidOperationException("Top of stack is not a value");
            var behaviour = Library.MapParameter(ILMethod.Parameters[argI], Function);
            StoreValue(parent: null, value, behaviour);
        }

        private void LoadLocal(int variableI) =>
            LoadValue(parent: null, Function.Variables[variableI]);

        private void LoadLocalAddress(VariableReference ilVariableRef) =>
            LoadValueAddress(parent: null, Function.Variables[ilVariableRef.Index]);

        private void StoreLocal(int variableI)
        {
            // TODO: Coercing
            var value = Pop() as ValueStackEntry ?? throw new InvalidOperationException("Top of stack is not a value");
            var variable = Function.Variables[variableI];
            if (value.Type != variable.ElementType)
                throw new InvalidOperationException($"Cannot store {value.Type} in variable of type {variable.ElementType}");
            StoreValue(parent: null, value, variable);
        }

        private void LoadField(FieldReference fieldRef)
        {
            var parent = Pop();
            var fieldBehaviour = Library.MapField(fieldRef);
            LoadValue(parent, fieldBehaviour);
        }

        private void LoadFieldAddress(FieldReference fieldRef)
        {
            var parent = Pop();
            var fieldBehavior = Library.MapField(fieldRef);
            LoadValueAddress(parent, fieldBehavior);
        }

        private void StoreField(FieldReference fieldRef)
        {
            var value = Pop() as ValueStackEntry ?? throw new InvalidOperationException("Top of stack is not a value");
            var parent = Pop();
            var fieldBehavior = Library.MapField(fieldRef);
            StoreValue(parent, value, fieldBehavior);
        }

        private void LoadIndirect()
        {
            if (Pop() is not ValueStackEntry pointer)
                throw new InvalidOperationException("LoadObject source is not a value");
            if (pointer.Type is not SpirvPointerType pointerType)
                throw new InvalidOperationException("LoadObject source is not a pointer value");

            var resultId = context.CreateID();
            Add(new OpLoad()
            {
                Result = resultId,
                ResultType = context.IDOf(pointerType.Type!),
                Pointer = pointer.ID
            });
            Stack.Add(new ValueStackEntry(resultId, pointerType.Type!));
        }

        private void StoreIndirect()
        {
            if (Pop() is not ValueStackEntry value)
                throw new InvalidOperationException("StoreObject source is not a value");
            if (Pop() is not ValueStackEntry dest)
                throw new InvalidOperationException("StoreObject destination is not a value");
            if (dest.Type is not SpirvPointerType)
                throw new InvalidOperationException("StoreObject destination is not a pointer value");

            Add(new OpStore()
            {
                Pointer = dest.ID,
                Object = value.ID
            });
        }
    }
}
