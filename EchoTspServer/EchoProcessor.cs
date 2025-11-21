using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    public interface IEchoProcessor
    {
        byte[] Process(byte[] input, int count);
    }

    public class EchoProcessor : IEchoProcessor
    {
        public byte[] Process(byte[] input, int count)
        {
            byte[] output = new byte[count];
            Array.Copy(input, output, count);
            return output;
        }
    }
}

