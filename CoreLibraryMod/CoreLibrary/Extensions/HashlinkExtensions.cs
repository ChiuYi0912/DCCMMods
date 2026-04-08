
namespace CoreLibrary.Core.Extensions
{
    public static unsafe class HashlinkExtensions
    {
        private static int HashFieldName(string fieldName)
        {
            fixed (char* pname = fieldName)
            {
                return Hashlink.HashlinkNative.hl_hash_gen(pname, false);
            }
        }

        public static double* GetHshlinkDoublePointer(nint HashlinkPtr,string fieldName)
        {
            var hash = HashFieldName(fieldName);
            Hashlink.HL_type* fieldType = null;
            var ptr = Hashlink.HashlinkNative.hl_obj_lookup((Hashlink.HL_vdynamic*)HashlinkPtr, hash, out fieldType);
            return (double*)ptr;
        }
    }
}