using System.Linq;
using System.Net;

namespace MonoProgram.Package.Utils
{
    public static class HostUtils
    {
        public static IPAddress ResolveHostOrIPAddress(string hostOrIPAddress)
        {
            IPAddress result;
            if (IPAddress.TryParse(hostOrIPAddress, out result))
                return result;
            else
                return Dns.GetHostEntry(hostOrIPAddress).AddressList.First();
        }
    }
}