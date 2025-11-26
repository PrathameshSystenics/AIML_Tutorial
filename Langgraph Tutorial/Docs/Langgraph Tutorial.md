## Streaming Responses from the Graph
LangGraph implements a streaming system to surface real-time updates. Streaming is crucial for enhancing the responsiveness of applications built on LLMs. 

Supported Stream Modes:
- `values`- Streams the full value of the state after each step of the graph.
- `updates`- Streams the updates to the state after each step of the graph. 
- `custom`- Streams custom data from inside your graph nodes. i.e from Writer.
- `messages`- Streams 2 tuples (LLM token, metadata) from any graph nodes where an LLM is invoked. 

These modes are passed in the methods like `astream`and `stream`methods returned by the `CompileStateGraph`.

For these tutorial we have created simple Graph with Checkpointer. 
#### Usage:
```python
config = {"configurable": {"thread_id": "10"}}

# Start conversation
async for chunk in agent.astream({"messages": [HumanMessage(content="do you have feelings?")]}, config, stream_mode="updates"):
    print(chunk)
```
The Stream method returns an iterator the yields streamed outputs. Set `stream_mode=updates`to stream only the updates to the graph. 

```python
# Start conversation
async for chunk in agent.astream({"messages": [HumanMessage(content="can you dance?")]}, config, stream_mode="messages"):
    print(chunk)
```
In the above code we are streaming the messages which returns `(LLmTOken, metadata)` tuple. These just like how *ChatGPT*gives word by word. 
#### Stream Events:
You can also stream the events by using the method `astream_events`which returns all the state event llike `on_chain_start`,`on_chat_model_start` etc, with data. 
```python
async for chunk in agent.astream_events({"messages": [HumanMessage(content="can you dance?")]}, config, stream_mode="messages"):
    print(chunk)
```
#### Stream Multiple Modes:
Stream the multiple modes by just passing the list of modes to `stream`or `astream`
```python
# Start conversation
async for chunk in agent.astream({"messages": [HumanMessage(content="can you dance?")]}, config, stream_mode=["messages","values","updates"]):
    print(chunk)
```
> [!QUESTION] Do You Know?
> When you stream the messages from the graph, but you have not used the stream inside one or more nodes using the `invoke`method. Then How it gives the streaming responses. These all are handled by the **Langgraph** once you stream the messages it invokes the Model present in the LLM as streamable and return the response in the form of stream, token-by-token.

