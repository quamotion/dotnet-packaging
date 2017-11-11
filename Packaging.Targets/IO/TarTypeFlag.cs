using System;
using System.Collections.Generic;
using System.Text;

namespace Packaging.Targets.IO
{
    internal enum TarTypeFlag : byte
    {
        RegType = (byte)'0',
        ARegType = (byte)'\0',
        LnkType = (byte)'1',
        SymType = (byte)'2',
        ChrType = (byte)'3',
        BlkType = (byte)'4',
        DirType = (byte)'5',
        FifoType = (byte)'6',
        ConttType = (byte)'7',
        ExtendedHeader = (byte)'x',
        GlobalExtendedHeader = (byte)'g',
        LongName = (byte)'L', // See https://www.gnu.org/software/tar/manual/html_node/Standard.html
        LongLink = (byte)'K' // See https://www.gnu.org/software/tar/manual/html_node/Standard.html
    }
}
