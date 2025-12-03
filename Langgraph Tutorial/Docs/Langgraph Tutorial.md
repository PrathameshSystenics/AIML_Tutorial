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

---
## Long-Term and Short Term Memory

**Short Term Memory** exists only during a single agent execution. It is stored in the variables, state and messages, context, and state object.

**Long term Memory** saved across multiple runs to help the agent recall past context. Stored in Vector Databases, Langgraph Checkpointer. 

Let's implement the store. Langgraph Stores long-term memories as JSON Documents in a store. Each memory is organized under a custom namespace (similar to a folder) and a distinct key (like a file name). Namespace is actually a tuple consisting of other labels that makes it easier to organize information. 

![[Pasted image 20251203044213.png]]
Below image is the illustration of the Store that how data is stored. 

When storing objects (e.g., memories) in the Store, we provide:
- The `namespace` for the object, a tuple (similar to directories)
- the object `key` (similar to filenames)
- the object `value` (similar to file contents)

We use the put method to save an object to the store by namespace and key.

```python
import sqlite3
from sqlite3 import Connection

connection: Connection = sqlite3.connect("store.db",check_same_thread=False,isolation_level=None)
# Initialize the SQLite store
store= SqliteStore(connection)
```
We will be creating the store of SQlite. Initialize the connection. 
```python
user_id = '12'
namespace_for_memory = (
    user_id,
    "chat_memory",
)  # Tuple representing user-specific namespace
key = "conversation_1"  # Unique key for storing in the store
value = {"food": "Pizza", "name": "John Doe"}  # Data to be stored
```
Sample data we gonna Save. 
```python
store.put(namespace_for_memory,key,value)
```
`put`method is used to store the data into the store. 

```python
store.list_namespaces()
```
List the Namespace present in the store. 
*Output:*
```txt
[('1', 'chat_memory'), ('12', 'chat_memory')]
```
Get the Values present for the namespace and key.
```python
store.get(namespace_for_memory,key)
```
*Output:*
```txt
Item(namespace=['12', 'chat_memory'], key='conversation_1', value={'food': 'Pizza', 'name': 'John Doe'}, created_at='2025-12-02T22:52:40', updated_at='2025-12-02T22:52:40')
```
You can even search the values in the store by providing the namespace to it. 
```python
store.search(namespace_for_memory,query="food")
```
*Output:*
```txt
[Item(namespace=['12', 'chat_memory'], key='conversation_1', value={'food': 'Pizza', 'name': 'John Doe'}, created_at='2025-12-02T22:52:40', updated_at='2025-12-02T22:52:40', score=None)]
```
If you need to access the store from the nodes or even tools you can access. 
**For Nodes:**
```python
def call_model(state: MessagesState, config: RunnableConfig, store: BaseStore):
    user_id = config["configurable"]["userid"]

    # Retrieve the memory from the store
    namespace_for_memory = (
        user_id,
        "chat_memory",
    )
    key = f"memory"
    existing_memory = store.get(namespace_for_memory, key)
    if existing_memory:
        memory_str = existing_memory.value.get("memory")
    else:
        memory_str = "No prior memory found."

    prompt_template = system_prompt_assistant.format(memory=memory_str)
    response = llm_model.invoke(
        [SystemMessage(content=prompt_template)] + state["messages"]
    )
    return {"messages": response}
```
Just pass the arguments as the `store`it will get the store

**For Tool:**
Need to get the runtime context and from there you can get the access to the store. 
### Complex Structure and Schema

In langchain or langgraph when we invoke the model with structured output there is possibility that it might get failed to extract the output, to encounter and solve these issue, langchain team created module named `trustcall` which patch the json output for huge and complex schema.

As we know that llm model can't structure the complex schemas, to solve these issue the trustcall module is used to extract the structured output. Let's see that in action.

Although the `trustcall`module still not consider inside of the ecosystem of the langchain. 

But their documentation suggests that for newer models use the response_format or structured_outputs only. You must use these module when you feel that LLM model are not giving the correct structured outputs. Although they are token expensive their internal retry logic for JSON Patch is little expensive when you are already working with expensive models. 

Create the Schema with the **Pydantic** model. 
```python
from pydantic import BaseModel, Field

class UserProfile(BaseModel):
    name: str = Field(description="Name of the User")
    location: str = Field(description="Location of the User")
    interests: list[str] = Field(description="Interests of the User")
```
Import the `create_extractor`method from the library **trustcall**.
```python
from trustcall import create_extractor
```
Create the extractor by providing the llm model, schema which you want to convert to output in forms of tools. 
```python
bound = create_extractor(
    llm=llm_model,
    tools=[UserProfile],
    tool_choice="UserProfile"
)
```
Creating the ChatPromptTemplate messages
```python
from langchain_core.prompts import ChatPromptTemplate

prompts = ChatPromptTemplate.from_messages(
    messages=[
        (
            "system",
            "Extract the user profile information from the following conversation.",
        ),
        ("human", "Hello my name is alice"),
        ("human", "I live in Wonderland and I love adventures."),
        ("human", "I love to eat cakes and explore new places."),
        ("human", "but i hate getting lost."),
    ]
)
```
Invoking the extractor
```python
from trustcall._base import ExtractionOutputs

response: ExtractionOutputs = bound.invoke(value)
response
```
It return the dictionary with the keys as `messages`,`responses`, `response_metadata` & `attempts`.
*Output:*
```txt
{'messages': [...],
 'responses': [UserProfile(name='alice', location='Wonderland', interests=['adventures', 'eating cakes', 'exploring new places'])],
 'response_metadata': [{'id': 'd5eeaebd-c1f0-4858-a75a-e89c54e56317'}],
 'attempts': 1}
```
From the output you can see that the `responses`contains the structured format returned by the extractor.

---
