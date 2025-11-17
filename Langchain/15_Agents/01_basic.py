import datetime
from langchain.agents import create_agent
from dotenv import load_dotenv
import os
from langchain.tools import tool
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_core.prompts import ChatPromptTemplate

from pydantic import BaseModel, Field

load_dotenv()

llm = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

# creating the basic tools
@tool()
def current_datetime() -> str:
    """Gets the current datetime of the system

    Returns:
        str: Current Date and Time in the string Format of the system
    """
    return datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")

# Creating the tool with arguments
class AddArgs(BaseModel):
    a: int = Field(description="First Number to Add", examples=[1])
    b: int = Field(description="Second Number to Add", examples=[2])


@tool(args_schema=AddArgs)
def add_two_number(a: int, b: int) -> int:
    """Function to add two Number

    Args:
        a (int): First number
        b (int): Second Number

    Returns:
        int: Returns the addition of the first and second number
    """
    return a+b


system_prompt: str = """
You are helpful assistant and have the access to the following functions:
    - 'add_two_number' : for adding the two number
    - 'current_datetime' : For returning the current datetime.
"""


# creating the agent
agent = create_agent(
    model=llm,
    tools=[current_datetime, add_two_number],
    system_prompt=system_prompt
)


messages = ChatPromptTemplate.from_messages(messages=[
    ("human", "{input}")
])

# creating the chain
output = messages | agent 

# invoking the chain
response = output.invoke({"input": "Add two number 1 and 2"})

print(response)
