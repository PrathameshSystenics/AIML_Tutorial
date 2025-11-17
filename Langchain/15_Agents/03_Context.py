from dataclasses import dataclass
from langchain.agents import create_agent
from dotenv import load_dotenv
import os
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain.tools import tool, ToolRuntime
from langchain_core.prompts import ChatPromptTemplate


from pydantic import BaseModel, Field

load_dotenv()

llm = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

system_prompt: str = """
You are helpful assistant helping to the user queries with only simple line without chitchat or extra bulky messages. 
Provided Tools:
    - get_user
"""

# creating the class for structured output
class UserResponse(BaseModel):
    query: str = Field(description="User Query")
    response: str = Field(description="Response from the ai model")

# creating the context
@dataclass
class UserContext():
    id: int = Field(description="User id")
    full_name: int = Field(description="user full name")



@tool
def get_user(runtime: ToolRuntime[UserContext]) -> str:
    """Get the user details

    Args:
        runtime (ToolRuntime[UserContext]): User details in string

    Returns:
        str: Userdetails serialized.
    """
    print(runtime.context)
    return f"userid={runtime.context.id}\tfull_name={runtime.context.full_name}"


# creating the agent
agent = create_agent(
    model=llm,
    system_prompt=system_prompt,
    response_format=UserResponse,
    tools=[get_user],
    # defining the context schema
    context_schema=UserContext
)


messages = ChatPromptTemplate.from_messages(messages=[
    ("human", "{input}")
])

# creating the chain
output = messages | agent

# invoking the chain
# defining the context values
response = agent.invoke(
    {"messages": [{"role": "user", "content": "give me the current user details"}]},
    context=UserContext(id=12, full_name="prathamesh dhande")  # Pass context here
)

print(response)
