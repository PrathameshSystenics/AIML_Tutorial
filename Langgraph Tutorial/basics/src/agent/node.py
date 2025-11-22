import os
from langchain_google_genai import ChatGoogleGenerativeAI
from .state import State
from .tools import tools
from langchain.messages import SystemMessage, AIMessage

system_message = SystemMessage("You are a helpful assistant")


llm = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GOOGLE_API_KEY"),
    include_thoughts=False,
)


llm_with_tools = llm.bind_tools(tools)


def assistant(state: State) -> State:
    messages = [system_message]
    messages.extend(state["messages"])
    response: AIMessage = llm_with_tools.invoke(messages)

    return {"messages": [response]}
