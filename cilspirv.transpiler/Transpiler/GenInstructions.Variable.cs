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

namespace cilspirv.Transpiler
{
    partial class Transpiler
    {
        partial class GenInstructions
        {
            private class FieldContext : ITranspilerFieldContext
            {
                private readonly GenInstructions gen;
                private StackEntry? result;

                public TranspilerLibrary Library => gen.Library;
                public TranspilerModule Module => gen.Module;
                public TranspilerFunction Function => gen.Function;
                public TranspilerOptions Options => gen.Options;
                public StackEntry Parent { get; }

                public StackEntry Result
                {
                    get => result ?? throw new InvalidOperationException("Result of field access was not set");
                    private set => result = value;
                }
                StackEntry ITranspilerFieldContext.Result
                {
                    set
                    {
                        if (result != null)
                            throw new InvalidOperationException("Result stack entry was already set");
                        Result = value;
                    }
                }

                public FieldContext(GenInstructions gen, StackEntry parent) =>
                    (this.gen, Parent) = (gen, parent);

                public ID CreateID() => gen.context.CreateID();
                public ID CreateIDFor(IInstructionGeneratable generatable) => gen.context.CreateIDFor(generatable);
                public ID IDOf(IInstructionGeneratable generatable) => gen.context.IDOf(generatable);
                public IEnumerable<T> OfType<T>() where T : IInstructionGeneratable => gen.context.OfType<T>();
            }

            private void LoadArgument(int argI)
            {
                if (argI < 0)
                {
                    Stack.Add(new StackEntry(new ThisModuleTag()));
                    return;
                }
                Stack.Add(Library.MapParameter(ILBody.Method.Parameters[argI], Function) switch
                {
                    TranspilerVarGroup varGroup => new StackEntry(varGroup),
                    TranspilerVariable variable => GetGlobalVariable(variable),
                    TranspilerParameter parameter => new ValueStackEntry(parameter, context.IDOf(parameter), parameter.Type),
                    _ => throw new NotSupportedException("Unsupported parameter type")
                });

                StackEntry GetGlobalVariable(TranspilerVariable variable)
                {
                    variable.MarkUsageIn(Function);
                    return new ValueStackEntry(variable, context.IDOf(variable), variable.PointerType);
                }
            }

            private void StoreArgument(int argI)
            {
                if (Pop() is not ValueStackEntry value)
                    throw new InvalidOperationException("Top of stack is not a value");

                var parameter = Library.MapParameter(ILBody.Method.Parameters[argI], Function);
                if (parameter is not TranspilerVariable variable)
                    throw new NotSupportedException("Real parameters cannot be written into");
                variable.MarkUsageIn(Function);
                Add(new OpStore()
                {
                    Pointer = context.IDOf(variable),
                    Object = value.ID
                });
            }

            private void LoadLocal(int variableI)
            {
                var id = context.CreateID();
                var spirvVariable = Function.Variables[variableI];
                Add(new OpLoad()
                {
                    Result = id,
                    ResultType = context.IDOf(spirvVariable.ElementType),
                    Pointer = context.IDOf(spirvVariable)
                });
                Stack.Add(new ValueStackEntry(spirvVariable, id, spirvVariable.ElementType));
            }

            private void PushLocalAddress(VariableReference ilVariableRef)
            {
                var spirvVariable = Function.Variables[ilVariableRef.Index];
                Stack.Add(new ValueStackEntry(spirvVariable, context.IDOf(spirvVariable), spirvVariable.PointerType));
            }

            private void StoreLocal(int variableI)
            {
                // TODO: Coercing
                var spirvVariable = Function.Variables[variableI];
                if (Pop() is not ValueStackEntry value)
                    throw new InvalidOperationException($"Stack top is not a SPIRV entry");
                if (value.Type != spirvVariable.ElementType)
                    throw new InvalidOperationException($"Cannot store {value.Type} in variable of type {spirvVariable.ElementType}");

                Add(new OpStore()
                {
                    Pointer = context.IDOf(spirvVariable),
                    Object = value.ID
                });
            }

            private ITranspilerFieldBehavior GetFieldBehavior(StackEntry parent, FieldReference fieldRef) =>
                parent.Tag is TranspilerVarGroup varGroup ? varGroup.Variables.First(v => v.Name == fieldRef.FullName)
                : Library.MapField(fieldRef) is ITranspilerFieldBehavior fieldBehavior ? fieldBehavior
                : throw new NotSupportedException("Unsupported field parent or field type");

            private void LoadField(FieldReference fieldRef)
            {
                var parent = Pop();
                var fieldBehavior = GetFieldBehavior(parent, fieldRef);
                var context = new FieldContext(this, parent);
                var instructions = fieldBehavior.Load(context);

                StackEntry result;
                if (instructions == null)
                {
                    instructions = fieldBehavior.LoadAddress(context = new FieldContext(this, parent))?.ToArray()
                        ?? throw new InvalidOperationException("Either Load or LoadAddress have to be defined");
                    if (context.Result is not ValueStackEntry resultValue)
                        throw new InvalidOperationException("Field LoadAddress did not result in a value");
                    if (resultValue.Type is not SpirvPointerType resultPointerType)
                        throw new InvalidOperationException("Field LoadAddress did not result in a pointer");

                    var newResultId = context.CreateID();
                    result = new ValueStackEntry(fieldBehavior, newResultId, resultPointerType.Type!);
                    Block.Instructions.AddRange(instructions);
                    Add(new OpLoad()
                    {
                        Result = newResultId,
                        ResultType = context.IDOf(resultPointerType.Type!),
                        Pointer = resultValue.ID
                    });
                }
                else
                {
                    Block.Instructions.AddRange(instructions);
                    result = context.Result;
                }

                Stack.Add(result);
            }

            private void LoadFieldAddress(FieldReference fieldRef)
            {
                var parent = Pop();
                var fieldBehavior = GetFieldBehavior(parent, fieldRef);
                var context = new FieldContext(this, parent);
                var instructions = fieldBehavior.LoadAddress(new FieldContext(this, parent))?.ToArray()
                    ?? throw new InvalidOperationException($"LoadAddress is not supported for field {fieldRef.FullName}");

                Block.Instructions.AddRange(instructions);
                Stack.Add(context.Result);
            }

            private void StoreField(FieldReference fieldRef)
            {
                var value = Pop() as ValueStackEntry ?? throw new InvalidOperationException("Top of stack is not a value");

                var parent = Pop();
                var fieldBehavior = GetFieldBehavior(parent, fieldRef);
                var context = new FieldContext(this, parent);
                var instructions = fieldBehavior.Store(context, value);
                if (instructions != null)
                {
                    Block.Instructions.AddRange(instructions);
                    return;
                }

                instructions = fieldBehavior.LoadAddress(context = new FieldContext(this, parent))?.ToArray()
                    ?? throw new InvalidOperationException("Either Load or LoadAddress have to be defined");
                if (context.Result is not ValueStackEntry resultValue)
                    throw new InvalidOperationException("Field LoadAddress did not result in a value");
                if (resultValue.Type is not SpirvPointerType resultPointerType)
                    throw new InvalidOperationException("Field LoadAddress did not result in a pointer");

                Block.Instructions.AddRange(instructions);
                Add(new OpStore()
                {
                    Pointer = resultValue.ID,
                    Object = value.ID
                });
            }

            private void LoadObject()
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

            private void StoreObject()
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
}
