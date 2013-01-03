using System.Diagnostics;

namespace CSharpCTFStarter.Messages
{
    [DebuggerDisplay("{__value__}")]
    public class Envelope<T>
    {
        public Envelope()
        {
        }

        public Envelope(T payload)
        {
            __class__ = typeof (T).Name;
            __value__ = payload;
        }

        public string __class__ ;
        public T __value__ ;
    }
}