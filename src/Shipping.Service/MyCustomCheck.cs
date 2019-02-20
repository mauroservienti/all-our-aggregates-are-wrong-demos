using NServiceBus.CustomChecks;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Shipping.Service
{
    class MyCustomCheck : CustomCheck
    {
        public MyCustomCheck()
            : base("FileSystem-Check", "Integration", TimeSpan.FromSeconds(10))
        {

        }

        public override Task<CheckResult> PerformCheck()
        {
            return Directory.Exists(@"c:\temp\integration")
                ? CheckResult.Pass
                : CheckResult.Failed(@"Cannot find integration directory in C:\temp");
        }
    }
}
