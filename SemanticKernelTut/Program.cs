using SemanticKernelTut;

#region Ollama Model
/*_02_Ollama_Sample ollamamodel = new _02_Ollama_Sample();
await ollamamodel.RunModel();*/
#endregion

#region Ollama Model - Plugin   => Not Working / Needs to Check
/*_04_Ollama_Plugin ollamamodel = new _04_Ollama_Plugin();
await ollamamodel.RunModel();*/
#endregion

#region AzureOpenAI Connector 
_05_AzureOpenAI_Connector azureopenai = new _05_AzureOpenAI_Connector();
await azureopenai.RunModel();
#endregion

