## Time Travel
When working with systems that make model-based decisions agents powered by LLMs. It can be useful to examine their decision-making process in detail
- Understanding Reasoning: Analyze the steps that led to a successful result.
- Debug Mistakes: Identify where and why errors occurred.
- Explore Alternatives: Test different parts to uncover better solutions. 

Langgraph provides time travel functionality to support these use cases. Specifically, you can resume execution from a  prior checkout either replaying the same state or modifying it to explore alternatives. In all cases, resuming past execution produces a new fork in the history

Create the graph and state required to us. 
```python
workflow = StateGraph(State)

# Add nodes
workflow.add_node("generate_topic", generate_topic)
workflow.add_node("write_joke", write_joke)

# Add edges to connect nodes
workflow.add_edge(START, "generate_topic")
workflow.add_edge("generate_topic", "write_joke")
workflow.add_edge("write_joke", END)

# Compile
checkpointer = InMemorySaver()
graph = workflow.compile(checkpointer=checkpointer)
```
For make these work make sure you have checkpointer or memory added. 
```python
config: RunnableConfig = {"configurable": {"thread_id": uuid.uuid1()}}
```
Created the config with unique `thread_id`. To get the previous checkpoints or `StateSnapshots`we need to pass the config correctly. 

```python
state = graph.invoke({}, config)
print(state)
```
Invoke the graph. 
Get the previous chat history from the state by passing the config. 
```python
states = graph.get_state_history(config)
for state in states:
    print(state.values)
    print("Next = ",state.next)
    print("checkpoint ID = ",state.config["configurable"]["checkpoint_id"])
    print("-----")
```
`get_state_history()`method returns the `StateSnapShots`when config is passed. 
```txt
True


{'configurable': {'thread_id': UUID('8ed777e0-cbb8-11f0-89a1-b48c9d911a92')}}
{'topic': 'Okay, ...'}
Next =  ()
checkpoint ID =  1f0cbb8b-3237-60d8-8006-c36d2df925d6
-----
{'topic': '..."'}
Next =  ('write_joke',)
checkpoint ID =  1f0cbb8a-f632-60b0-8005-c7818e09e437
```
Update the State by selecting the state you want to continue with it. 
```python
selected_state = list(graph.get_state_history(config))[-3]
```
Update the value of the state using `update_state`method
```python
new_config= graph.update_state(selected_state.config,values={"topic":"computer"})
```
Invoke the graph with the new config you got when updated the graph state. 
```python
async for chunk in graph.astream(None,new_config,stream_mode="updates"):
    print(chunk)
```
*Output:*
```txt
{'write_joke': {'joke': 'Why was the computer cold?\n\nBecause it left its Windows open!'}}
```
So it does not return the `generate_topic`because there was already that state present in the `StateSnapShots`so either running all the nodes from starting it just updated the state and continuing working. These were called as **Time Travel**. 

---

