namespace Packaging.Targets.Rpm
{
    internal enum IndexType : uint
    {
        RPM_NULL_TYPE = 0,
        RPM_CHAR_TYPE = 1,
        RPM_INT8_TYPE = 2,
        RPM_INT16_TYPE = 3,
        RPM_INT32_TYPE = 4,
        RPM_INT64_TYPE = 5,
        RPM_STRING_TYPE = 6,
        RPM_BIN_TYPE = 7,
        RPM_STRING_ARRAY_TYPE = 8,
        RPM_I18NSTRING_TYPE = 9
    }
}
