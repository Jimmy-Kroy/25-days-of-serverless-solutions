using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindImageApp.Helpers
{
    public interface IKeyVaultHelper
    {
        string GetCachedSecret(string secretName);
    }
}
