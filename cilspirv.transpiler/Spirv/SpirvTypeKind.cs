namespace cilspirv.Spirv
{
    public enum SpirvTypeKind
    {
        Void,
        Boolean,
        Integer,
        Floating,
        Vector,
        Matrix,
        Array,
        Structure,

        Sampler,
        Filter,
        RuntimeArray,
        Opaque,
        Pointer,
        Function,
        Event,
        DeviceEvent,
        ReserveId,
        Queue,
        Pipe
    }
}