## Subgraph
A subgraph is a graph that is used as a node in another graph. They are useful for building multi-agent systems. Re-using a set of nodes in multiple graphs. 

In these tutorial we are going to add the subgraph which is basic only without any LLM call. 

Create the Subgraph State with its output schema
```python
class GenerateReportAnalysis(TypedDict):
    report: str
    text: str
    
class GenerateReportAnalysisOutput(TypedDict):
    analysis: str
```
In the above code we have created the state for our graph and output schemas which will return these values. 

```python
def node_generate_report(state: GenerateReportAnalysis) -> GenerateReportAnalysisOutput:
    return {
        "analysis": f"This is an analysis of the report: {state['report']}"
    }
```
Creating the dummy node which just returns the analysis. 

```python
graph_1 = StateGraph(GenerateReportAnalysis, output_schema=GenerateReportAnalysisOutput)
graph_1.add_node("generate_report", node_generate_report)

graph_1.add_edge(START, "generate_report")
graph_1.add_edge("generate_report", END)

generate_report_subgraph = graph_1.compile()
```
Lets add together all of them node and compile the graph. 
![[Pasted image 20251129055821.png]]
The graph will look like these

Creating another seconds graph the step is similar to the above graph we created. 

let's create the parent graph with state. 
```python
class Reports(TypedDict):
    report: str
    text: str
    summary: str
    analysis: str
```
**Note**: In the parent graph we have those two variables also which the subgraph returns if you didn't mention them in the subgraph it will throw an exception. 

Adding the subgraph as the node.
```python
graph = StateGraph(Reports)
graph.add_node("get_reports", get_reports)
graph.add_node("generate_summary_subgraph", generate_summary_subgraph)
graph.add_node("generate_report_subgraph", generate_report_subgraph)
graph.add_node("send_to_mail", send_to_mail)

graph.add_edge(START, "get_reports")
graph.add_edge("get_reports", "generate_summary_subgraph")
graph.add_edge("get_reports", "generate_report_subgraph")
graph.add_edge(
    ["generate_summary_subgraph", "generate_report_subgraph"], "send_to_mail"
)
graph.add_edge("send_to_mail", END)


compile_graph = graph.compile()
```
We add the subgraph as node which is compiled. 
![[Pasted image 20251129060302.png]]
It looks like these.

Invoking the graph. We are streaming the graph to get the output of the subgraph you need to set the `subgraphs`variable as true.
```python
for chunk in compile_graph.stream({"text": "The langgraph library is a powerful tool for building stateful applications using language"},stream_mode=["custom","updates"],subgraphs=True):
    print(chunk)
```
*Output:*
```txt
((), 'updates', {'get_reports': {'report': 'Why to investigate about the langgraph library?'}})
(('generate_report_subgraph:47a9083e-ed3d-2b73-1ba9-fdf585993118',), 'updates', {'generate_report': {'analysis': 'This is an analysis of the report: Why to investigate about the langgraph library?'}})
((), 'updates', {'generate_report_subgraph': {'analysis': 'This is an analysis of the report: Why to investigate about the langgraph library?'}})
(('generate_summary_subgraph:1f41c556-3434-3ba5-1220-0f90b45309f5',), 'updates', {'generate_summary': {'summary': 'This is a summary of the report: Why to investigate about the langgraph library?'}})
((), 'updates', {'generate_summary_subgraph': {'summary': 'This is a summary of the report: Why to investigate about the langgraph library?'}})
((), 'custom', 'Sending report analysis to mail: This is an analysis of the report: Why to investigate about the langgraph library?\n')
((), 'updates', {'send_to_mail': {'report': 'Why to investigate about the langgraph library?', 'text': 'The langgraph library is a powerful tool for building stateful applications using language', 'summary': 'This is a summary of the report: Why to investigate about the langgraph library?', 'analysis': 'This is an analysis of the report: Why to investigate about the langgraph library?'}})
```

---
