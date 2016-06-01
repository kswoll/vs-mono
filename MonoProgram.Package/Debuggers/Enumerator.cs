using Microsoft.VisualStudio;

namespace MonoProgram.Package.Debuggers
{
    public class Enumerator<T, I> where I : class
    {
        readonly T[] data;
        uint position;

        public Enumerator(T[] data)
        {
            this.data = data;
            position = 0;
        }

        public int Clone(out I ppEnum)
        {
            ppEnum = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetCount(out uint pcelt)
        {
            pcelt = (uint)data.Length;
            return VSConstants.S_OK;
        }

        public int Next(uint celt, T[] rgelt, out uint celtFetched)
        {
            return Move(celt, rgelt, out celtFetched);
        }

        public int Reset()
        {
            lock (this)
            {
                position = 0;

                return VSConstants.S_OK;
            }
        }

        public int Skip(uint celt)
        {
            uint celtFetched;

            return Move(celt, null, out celtFetched);
        }

        private int Move(uint celt, T[] rgelt, out uint celtFetched)
        {
            lock (this)
            {
                int hr = VSConstants.S_OK;
                celtFetched = (uint)data.Length - position;

                if (celt > celtFetched)
                {
                    hr = VSConstants.S_FALSE;
                }
                else if (celt < celtFetched)
                {
                    celtFetched = celt;
                }

                if (rgelt != null)
                {
                    for (int c = 0; c < celtFetched; c++)
                    {
                        rgelt[c] = data[position + c];
                    }
                }

                position += celtFetched;

                return hr;
            }
        }         
    }
}