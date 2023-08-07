// Copyright (c) Microsoft. All rights reserved.

namespace PromptPlayground.Services.TemplateEngine.Abstractions.Blocks;

internal enum BlockTypes
{
    Undefined = 0,
    Text = 1,
    Code = 2,
    Variable = 3,
    Value = 4,
    FunctionId = 5,
}
