using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure.Enums
{
    /// <summary>
    /// Repressents the specific command that send.
    /// </summary>
    public enum CommandEnum : int
    {
        NewFileCommand,
        CloseCommand
    }
}
