from typing import Annotated, TypedDict
from langchain.messages import AnyMessage
from langgraph.graph import add_messages

class State(TypedDict):
    messages:Annotated[list[AnyMessage],add_messages]