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
## Map - Reduce (SEND)
Map is used to break a task into smaller sub-tasks, processing each sub-task in parallel. **Reduce** aggregate the results across all of the computed, parallelized sub-tasks. 
These particularly also known as **Fan-In** & **Fan-out** 

In Software engineering, Fan-out is when one process or function sends its output to many other processes, while fan-in is when multiple processes or functions send their outputs to a single process. 

For Implementing these in Langgraph there is already api available called **Send** for dynamically creating worker executions. It lets a routing functions returns a list of Send objects. Each `Send`launch a separate run of `node_name`with its own state. The state must support the reducer as the `operator.add`. 

```python
def continue_to_jokes(state: OverallState):
    return [Send("generate_joke", {"subject": s}) for s in state["subjects"]]
```
These nodes creates the multiple workers, with `node_name`as generate_joke and sending its state with its. 

```python
graph = StateGraph(OverallState)
graph.add_node("generate_topics", generate_topics)
graph.add_node("generate_joke", generate_joke)
graph.add_node("best_joke", best_joke)

graph.add_edge(START, "generate_topics")
graph.add_conditional_edges("generate_topics", continue_to_jokes, ["generate_joke"])
graph.add_edge("generate_joke", "best_joke")
graph.add_edge("best_joke", END)

app = graph.compile()
Image(app.get_graph().draw_mermaid_png())
```
Here is the graph 
![[Pasted image 20251130141341.png]]
For the above code. 
*Output:*
```txt
{'generate_topics': {'subjects': ['pets', 'wildlife', 'marine life']}}
{'generate_joke': {'jokes': ['Why did the cat sit on the computer? To keep an eye on the mouse!']}}
{'generate_joke': {'jokes': ['Why did the fish get bad grades? Because it was below C-level!']}}
{'generate_joke': {'jokes': ["Why did the bear break up with the deer? Because they weren't on the same fawn-length!"]}}
{'best_joke': {'best_selected_joke': 'Why did the cat sit on the computer? To keep an eye on the mouse!'}}
```
You can see in the output that `generate_joke` have been created three times. 
