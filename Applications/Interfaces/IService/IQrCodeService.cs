using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Interfaces.IService
{
    public interface IQrCodeService
    {
        byte[] GenerateQr(string content);
    }
}
