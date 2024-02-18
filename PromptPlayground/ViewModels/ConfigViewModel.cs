using Azure.AI.OpenAI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using PromptPlayground.Messages;
using PromptPlayground.Services;
using PromptPlayground.ViewModels.ConfigViewModels;
using PromptPlayground.ViewModels.ConfigViewModels.LLM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PromptPlayground.ViewModels;

public partial class ConfigViewModel : ViewModelBase, IConfigAttributesProvider,
										IRecipient<ConfigurationRequestMessage>,
										IRecipient<RequestMessage<IConfigAttributesProvider>>
{
	private string[] RequiredAttributes =
   [
       #region LLM Config
       ConfigAttribute.AzureDeployment,
	   ConfigAttribute.AzureEndpoint,
	   ConfigAttribute.AzureSecret,
	   ConfigAttribute.BaiduClientId,
	   ConfigAttribute.BaiduSecret,
	   ConfigAttribute.BaiduModel,
	   ConfigAttribute.OpenAIApiKey,
	   ConfigAttribute.OpenAIModel,
	   ConfigAttribute.DashScopeApiKey,
	   ConfigAttribute.DashScopeModel,
	   ConfigAttribute.LlamaModelPath
       #endregion
   ];

	public List<ConfigAttribute> AllAttributes { get; set; } = [];

	[ObservableProperty]
	private int maxCount = 3;

	[ObservableProperty]
	private bool runStream = false;

	#region Model
	private int modelSelectedIndex = 0;
	private ProfileService<ConfigViewModel> _profile;

	public int ModelSelectedIndex
	{
		get => modelSelectedIndex; set
		{
			if (modelSelectedIndex != value)
			{
				modelSelectedIndex = value;
				OnPropertyChanged(nameof(ModelSelectedIndex));
				OnPropertyChanged(nameof(SelectedModel));
				OnPropertyChanged(nameof(ModelAttributes));
				OnPropertyChanged(nameof(SelectedModel.Name));
			}
		}
	}

	[JsonIgnore]
	public List<string> ModelLists => LLMs.Select(_ => _.Name).ToList();
	[JsonIgnore]
	public IList<ConfigAttribute> ModelAttributes => SelectedModel.SelectAttributes(this.AllAttributes);
	[JsonIgnore]
	public ILLMConfigViewModel SelectedModel => LLMs[ModelSelectedIndex];
	[JsonIgnore]
	private readonly List<ILLMConfigViewModel> LLMs = [];
	#endregion

	#region IConfigAttributesProvider
	IList<ConfigAttribute> IConfigAttributesProvider.AllAttributes => this.AllAttributes;
	public ILLMConfigViewModel GetLLM()
	{
		return this.SelectedModel;
	}
	#endregion

	public ConfigViewModel(bool requireLoadConfig = false) : this()
	{
		if (requireLoadConfig)
		{
			WeakReferenceMessenger.Default.RegisterAll(this);
			LoadConfigFromUserProfile();
		}
	}

	public ConfigViewModel()
	{
		this.AllAttributes = CheckAttributes(this.AllAttributes);

		LLMs.Add(new AzureOpenAIConfigViewModel(this));
		LLMs.Add(new BaiduConfigViewModel(this));
		LLMs.Add(new OpenAIConfigViewModel(this));
		LLMs.Add(new DashScopeConfigViewModel(this));
		// LLMs.Add(new LlamaSharpConfigViewModel(this));

		this._profile = new ProfileService<ConfigViewModel>("user.config");
	}

	private void LoadConfigFromUserProfile()
	{
		var vm = this._profile.Get();
		if (vm != null)
		{
			this.AllAttributes = CheckAttributes(vm.AllAttributes);

			this.MaxCount = vm.MaxCount;
			this.RunStream = vm.RunStream;
			this.ModelSelectedIndex = vm.ModelSelectedIndex;
		}
	}
	private List<ConfigAttribute> CheckAttributes(List<ConfigAttribute> list)
	{
		foreach (var item in RequiredAttributes)
		{
			if (!list.Any(_ => _.Name == item))
			{
				list.Add(new ConfigAttribute(item));
			}
		}
		return list;
	}

	private void SaveConfigToUserProfile()
	{
		this._profile.Save(this);
	}

	public void SaveConfig()
	{
		SaveConfigToUserProfile();
	}

	public void ReloadConfig()
	{
		this.LoadConfigFromUserProfile();
	}

	public void Receive(RequestMessage<IConfigAttributesProvider> message)
	{
		message.Reply(this);
	}

	public void Receive(ConfigurationRequestMessage request)
	{
		switch (request.Config)
		{
			case nameof(MaxCount):
				request.Reply(this.MaxCount.ToString());
				break;
			case nameof(RunStream):
				request.Reply(this.RunStream.ToString());
				break;
		}
	}
}
