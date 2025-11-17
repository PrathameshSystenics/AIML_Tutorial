from dataclasses import dataclass
import sys
from langchain.agents import create_agent
from dotenv import load_dotenv
import os
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain.agents.middleware import HumanInTheLoopMiddleware
from langchain.tools import tool
from langgraph.checkpoint.memory import InMemorySaver
from langgraph.types import Command

from pydantic import BaseModel, Field

load_dotenv()

def print_agent_response_simple(response: dict):
    """Simple version - print messages only."""
    print("\n" + "="*80)
    
    for msg in response.get("messages", []):
        msg_type = type(msg).__name__
        
        if msg_type == "HumanMessage":
            print(f"👤 User: {msg.content}")
        
        elif msg_type == "AIMessage":
            if msg.content:
                print(f"🤖 AI: {msg.content}")
            
            if msg.tool_calls:
                print(f"🔧 Tool Calls:")
                for tc in msg.tool_calls:
                    print(f"   - {tc['name']}: {tc['args']}")
        
        elif msg_type == "ToolMessage":
            print(f"📤 Tool Response ({msg.name}): {msg.content}")
    
    # Print final structured response
    if "structured_response" in response:
        sr = response["structured_response"]
        print(f"\n✅ Final Response: {sr.response}")
    
    print("="*80 + "\n")


llm = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

system_prompt: str = """
You are helpful assistant helping to the user queries with only simple line without chitchat or extra bulky messages. 
Provided Tools:
    - create_file_tool
"""

# creating the class for structured output
class UserResponse(BaseModel):
    query: str = Field(description="User Query")
    response: str = Field(description="Response from the ai model")

# Tool which require human intervention
@tool(parse_docstring=True)
def create_file_tool(text: str) -> bool:
    """Creates the File with the provided text

    Args:
        text (str): text to write in the file

    Returns:
        bool: Returns true if the file is created successfully
    """
    with open("file.txt","w") as f:
        f.write(text)


# creating the agent
agent = create_agent(
    model=llm,
    system_prompt=system_prompt,
    response_format=UserResponse,
    tools=[create_file_tool],
    checkpointer=InMemorySaver(),
    middleware=[
        HumanInTheLoopMiddleware(
            interrupt_on={
                "create_file_tool": {
                    "allowed_decisions": ["approve", "reject"],
                    "description": "Permissions required for Creating the New file"
                },
            },
            description_prefix="Tool Execution Pending approval"
        )
    ]
)

config = {"configurable": {"thread_id": "123"}}

# invoking the chain
response = agent.invoke(
    {"messages": [{"role": "user", "content": "create the file with txt hello these is prathamesh"}]},
    config=config
)

print_agent_response_simple(response)


# allow the approval
response = agent.invoke(
    Command(
        resume={
            "decisions": [
                {
                    "type": "approve",
                }
            ]
        }
    ),
    config=config  # Same thread ID to resume the paused conversation
)

print_agent_response_simple(response)