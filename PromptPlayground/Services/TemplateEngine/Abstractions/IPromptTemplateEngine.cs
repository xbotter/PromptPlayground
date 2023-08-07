﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Orchestration;
using PromptPlayground.Services.TemplateEngine.Abstractions.Blocks;

namespace PromptPlayground.Services.TemplateEngine.Abstractions;

/// <summary>
/// Prompt template engine interface.
/// </summary>
public interface IPromptTemplateEngine
{
    /// <summary>
    /// Given a prompt template string, extract all the blocks (text, variables, function calls)
    /// </summary>
    /// <param name="templateText">Prompt template (see skprompt.txt files)</param>
    /// <param name="validate">Whether to validate the blocks syntax, or just return the blocks found, which could contain invalid code</param>
    /// <returns>A list of all the blocks, ie the template tokenized in text, variables and function calls</returns>
    IList<Block> ExtractBlocks(
        string? templateText,
        bool validate = true);

    /// <summary>
    /// Given a prompt template, replace the variables with their values and execute the functions replacing their
    /// reference with the function result.
    /// </summary>
    /// <param name="templateText">Prompt template (see skprompt.txt files)</param>
    /// <param name="context">Access into the current kernel execution context</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The prompt template ready to be used for an AI request</returns>
    Task<string> RenderAsync(
        string templateText,
        SKContext context,
        CancellationToken cancellationToken = default);
}
