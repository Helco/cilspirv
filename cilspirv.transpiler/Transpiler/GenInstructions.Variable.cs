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
            private void LoadArgument(int argI)
            {
                if (argI < 0)
                {
                    Stack.Add(new StackEntry(new ThisModule()));
                    return;
                }
                Stack.Add(Library.MapParameter(ILBody.Method.Parameters[argI], Function) switch
                {
                    TranspilerVarGroup varGroup => new StackEntry(varGroup),
                    TranspilerVariable variable => GetGlobalVariablePtr(variable),
                    TranspilerParameter parameter => new ValueStackEntry(parameter, context.IDOf(parameter), parameter.Type),
                    _ => throw new NotSupportedException("Unsupported parameter type")
                });
            }

            private void StoreArgument(int argI)
            {
                if (Pop() is not ValueStackEntry value)
                    throw new InvalidOperationException("Top of stack is not a value");

                var parameter = Library.MapParameter(ILBody.Method.Parameters[argI], Function);
                if (parameter is not TranspilerVariable variable)
                    throw new NotSupportedException("Real parameters cannot be written into");
                MarkVariableUsage(variable);
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

            private StackEntry PopAndGetFieldPtr(FieldReference fieldRef) => Pop() switch
            {
                ValueStackEntry value => GetFieldPtrFromStruct(value, fieldRef),

                var pseudo when pseudo.Tag is ThisModule => Library.MapField(fieldRef) switch
                {
                    TranspilerVariable variable => GetGlobalVariablePtr(variable),
                    TranspilerVarGroup varGroup => new StackEntry(varGroup),
                    _ => throw new NotSupportedException("Unsupported global field type")
                },
                var pseudo when pseudo.Tag is TranspilerVarGroup varGroup => GetVariableFromGroupPtr(varGroup, fieldRef),

                _ => throw new NotSupportedException("Unsupported field container")
            };

            private StackEntry GetVariableFromGroupPtr(TranspilerVarGroup varGroup, FieldReference fieldRef)
            {
                var variable = varGroup.Variables.First(v => v.Name == fieldRef.FullName);
                return GetGlobalVariablePtr(variable);
            }

            private StackEntry GetGlobalVariablePtr(TranspilerVariable variable)
            {
                MarkVariableUsage(variable);
                return new ValueStackEntry(variable, context.IDOf(variable), variable.PointerType);
            }

            private void MarkVariableUsage(TranspilerVariable variable)
            {
                if (Function is TranspilerEntryFunction entryFunction &&
                    (variable.StorageClass == StorageClass.Input || variable.StorageClass == StorageClass.Output))
                    entryFunction.Interface.Add(variable);
            }

            private StackEntry GetFieldPtrFromStruct(ValueStackEntry structValue, FieldReference fieldRef)
            {
                var resultId = context.CreateID();
                switch(structValue.Type)
                {
                    // TODO: Whether both these cases are actually possible

                    case SpirvPointerType structPointerType when structPointerType.Type is SpirvStructType structType:
                        var member = structType.Members.First(m => m.Name == fieldRef.Name);
                        var resultType = new SpirvPointerType()
                        {
                            Type = member.Type,
                            StorageClass = structPointerType.StorageClass
                        };
                        Add(new OpAccessChain()
                        {
                            Result = resultId,
                            ResultType = context.IDOf(resultType),
                            Base = structValue.ID,
                            Indexes = ImmutableArray.Create(
                                context.IDOf(
                                    new TranspilerNumericConstant(member.Index)))
                        });
                        return new ValueStackEntry(member, resultId, resultType);

                    case SpirvStructType structType:
                        member = structType.Members.First(m => m.Name == fieldRef.Name);
                        Add(new OpCompositeExtract()
                        {
                            Result = resultId,
                            ResultType = context.IDOf(member.Type),
                            Composite = structValue.ID,
                            Indexes = ImmutableArray.Create<LiteralNumber>(member.Index)
                        });
                        return new ValueStackEntry(member, resultId, member.Type);

                    default: throw new InvalidOperationException("Invalid structure type to get a field from");
                }

            }

            private void LoadField(FieldReference fieldRef)
            {
                var fieldEntry = PopAndGetFieldPtr(fieldRef);
                if (fieldEntry is not ValueStackEntry fieldValue)
                    throw new InvalidOperationException("Top of stack is not a SPIRV entry");
                if (fieldValue.Type is not SpirvPointerType pointerType)
                    throw new InvalidOperationException("Top of stack is not a pointer type");

                var resultId = context.CreateID();
                Add(new OpLoad()
                {
                    Result = resultId,
                    ResultType = context.IDOf(pointerType.Type!),
                    Pointer = fieldValue.ID
                });
                Stack.Add(new ValueStackEntry(fieldValue.Tag!, resultId, pointerType.Type!));
            }

            private void LoadFieldAddress(FieldReference fieldRef)
            {
                Stack.Add(PopAndGetFieldPtr(fieldRef));
            }

            private void StoreField(FieldReference fieldRef)
            {
                if (Pop() is not ValueStackEntry value)
                    throw new InvalidOperationException("Top of stack is not a value");

                var fieldEntry = PopAndGetFieldPtr(fieldRef);
                if (fieldEntry is not ValueStackEntry field)
                    throw new InvalidOperationException("Top of stack is not a SPIRV entry");
                if (field.Type is not SpirvPointerType)
                    throw new InvalidOperationException("Top of stack is not a pointer type");

                Add(new OpStore()
                {
                    Pointer = field.ID,
                    Object = value.ID
                });
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
