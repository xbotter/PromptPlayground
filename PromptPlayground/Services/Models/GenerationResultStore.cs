using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptPlayground.Services.Models
{
    internal class GenerationResultStore
    {
        public long Id { get; set; }
        public required string FunctionPath { get; set; }
        public required string Text { get; set; }
        public required string RenderedPrompt { get; set; }
        public ResultTokenUsage? Usage { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan Elapsed { get; set; }
    }
}
