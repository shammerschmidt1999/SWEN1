using System;
using SWEN1_MCTG.Classes.HttpSvr;

namespace SWEN1_MCTG.Interfaces
{
    public interface IHandler
    {
        public Task<bool> HandleAsync(HttpSvrEventArgs e);
    }
}
