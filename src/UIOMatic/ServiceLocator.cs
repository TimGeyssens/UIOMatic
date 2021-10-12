using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIOMatic
{
    public static class ServiceLocator
    {
        private static IServiceProviderProxy diProxy;

        public static IServiceProviderProxy ServiceProvider => diProxy ?? throw new Exception("You should Initialize the ServiceProvider before using it.");

        public static void Initialize(IServiceProviderProxy proxy)
        {
            diProxy = proxy;
        }
    }
}
