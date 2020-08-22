// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;
using SE.Text.Parsing;

namespace SE.Text.Html
{
    public partial class HtmlParser
    {
        bool FinalizeRule(ProductionState fsmCommand)
        {
            switch (fsmCommand)
            {
                case ProductionState.Revert:
                case ProductionState.Success: return true;
                case ProductionState.Shift:
                case ProductionState.Reduce:
                case ProductionState.Preserve: return false;
                default:
                case ProductionState.Failure: BuilderState.Add(HtmlParserState.Failure);
                    return false;
            }
        }
    }
}
