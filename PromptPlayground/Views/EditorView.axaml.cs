using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.SemanticKernel;
using MsBox.Avalonia;
using PromptPlayground.Messages;
using PromptPlayground.Services;
using PromptPlayground.ViewModels;
using PromptPlayground.ViewModels.ConfigViewModels;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TextMateSharp.Grammars;

namespace PromptPlayground.Views;

public partial class EditorView : UserControl, IRecipient<FunctionSelectedMessage>
{
    public EditorView()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void Receive(FunctionSelectedMessage message)
    {
        this.DataContext = message.Function;

        if (message.Function.PromptConfig.TemplateFormat == "handlebars")
        {
            var _editor = this.FindControl<TextEditor>("prompt");

            var _registryOptions = new RegistryOptions(ThemeName.LightPlus);

            var _textMateInstallation = _editor.InstallTextMate(_registryOptions);

            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".handlebars").Id));

        }
    }
}