// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Parallel.Processing;

namespace SE.Parallel
{
    public static class ParallelExtension
    {
        private static Adapter adapter;
        public static Adapter ParallelAdapter
        {
            get { return adapter; }
        }

        static ParallelExtension()
        {
            adapter = new Adapter(new CustomPoolingBehavior());
        }
    }
}