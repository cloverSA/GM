﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaCrsQAInstaller.Helper
{
    public interface IPageViewModel
    {
        string Name { get; }
        bool SaveContent();
    }
}
