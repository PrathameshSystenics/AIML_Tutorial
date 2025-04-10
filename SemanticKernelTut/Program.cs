using Microsoft.Extensions.Configuration;
using ProcessFramework;
using SemanticKernelTut;

#region Configuration Manager - JSON
// Json File Path
string path = "D:\\Training\\AIML\\SemanticKernelTut\\Appsettings.json";

// Make the JSON File Available to the Programs. 
IConfiguration config = new ConfigurationBuilder().AddJsonFile(path).Build();
#endregion

#region Ollama Model
/*_02_Ollama_Sample ollamamodel = new _02_Ollama_Sample();
await ollamamodel.RunModel();*/
#endregion

#region Ollama Model - Plugin   => Not Working / Needs to Check
/*_04_Ollama_Plugin ollamamodel = new _04_Ollama_Plugin();
await ollamamodel.RunModel();*/
#endregion

#region AzureOpenAI Connector 
/*_05_AzureOpenAI_Connector azureopenai = new _05_AzureOpenAI_Connector(config);
await azureopenai.RunModel();*/
#endregion

#region Hand Digit Recognition 
/*_06_ImagetoText_Sample imagetotext = new _06_ImagetoText_Sample(config);
await imagetotext.RunModel();*/
#endregion

#region Agent Framework
/*_07_AgentFramework agentframework = new _07_AgentFramework(config);
await agentframework.RunGeminiAgent();*/
#endregion

#region Process Framework
/*DocumentationProcess docprocess = new DocumentationProcess(config);
docprocess.BuildSimpleProcess();
await docprocess.RunBuildProcessAsync();
docprocess.GetProcessMermaidDiagram();*/
#endregion

#region Google Search Library
_08_GoogleSearch googlesearch = new _08_GoogleSearch(config);
await googlesearch.ReadAsTextFromHtml("what is agents in ai");
#endregion

