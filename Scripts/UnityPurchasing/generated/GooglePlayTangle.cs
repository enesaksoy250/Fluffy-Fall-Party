// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("VP4Ciygf8nhe2smYd2IfLrcm9G0cKyWP6MUlfF/X08/KVXS6kGadwVFiRwp84Fv+wgOtWTQIyViAg8j2vPFXxXUpXAxjU5RtGsMWxA06eq+AfTUPiUk3TgUTY7B9cVKxbH2FuaAAilnOuvUkXUEttQBMLE6mW/pkLfjX1EUZXHRuKQupxbxB+kK+bagGyP/Wr7uF8COZuiGej3Osx8+xx8BDTUJywENIQMBDQ0LmsM9ffBIDjQoNAIyRm+yO98fMqw7p690HtPQmUJS7nxQOtpyNYKThSh4kaAKP4IAWLeXYqIAbP3rQut9pI5U3WAF5csBDYHJPREtoxArEtU9DQ0NHQkHGm7tDQhzR5Hn5CfaSW9AFtJ4CcN1mgQPK3dCMPUBBQ0JD");
        private static int[] order = new int[] { 7,3,4,10,5,13,9,11,10,10,13,12,12,13,14 };
        private static int key = 66;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
