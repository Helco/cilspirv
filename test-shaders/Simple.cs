﻿using System;
using System.Numerics;
using cilspirv.Library;
using cilspirv.Spirv;

namespace test_shaders
{
    [Capability(Capability.Shader)]
    [MemoryModel(AddressingModel.Logical, MemoryModel.GLSL450)]
    public class Simple
    {
        public struct VSInput
        {
            [Location(0)] public Vector3 pos;
            [Location(1)] public Vector2 uv;
            [Location(2)] public Vector4 color;
        }

        public struct FSInput
        {
            [Location(0)] public Vector2 uv;
            [Location(1)] public Vector4 color;
        }

        public struct Uniforms
        {
            public const int Size = (4 + 1 + 1 + 1) * sizeof(float);

            [Offset(0)] public Vector4 tint;
            [Offset(16)] public float vectorColorFactor;
            [Offset(20)] public float tintFactor;
            [Offset(24)] public float alphaReference;
        }

        private Vector4 WeighColor(Vector4 color, float factor) =>
            color * factor + new Vector4(1 - factor);

        [EntryPoint(ExecutionModel.Fragment)]
        public void Frag(
            [Input] in FSInput input,
            
            [Uniform, Binding(0, 5)] in Uniforms uniforms,

            [Output, Location(0)] out Vector4 output)
        {
            output = input.color
                * WeighColor(input.color, uniforms.vectorColorFactor)
                * WeighColor(uniforms.tint, uniforms.tintFactor);
            if (output.W < uniforms.alphaReference)
                Environment.Exit(0);
        }

        [EntryPoint(ExecutionModel.Vertex)]
        public void Vert(
            [Input] in VSInput input,

            [Uniform, Binding(0, 2)] in Matrix4x4 projection,
            [Uniform, Binding(0, 3)] in Matrix4x4 view,
            [Uniform, Binding(0, 4)] in Matrix4x4 world,

            [Output, BuiltIn(BuiltIn.Position)] out Vector4 position,
            [Output] out FSInput output)
        {
            position = new Vector4(input.pos, 1f);
            position = Vector4.Transform(position, world);
            position = Vector4.Transform(position, view);
            position = Vector4.Transform(position, projection);
            output.uv = input.uv;
            output.color = input.color;
        }
    }
}
