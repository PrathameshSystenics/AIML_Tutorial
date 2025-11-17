from langchain.agents import create_agent
from dotenv import load_dotenv
import os
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_core.prompts import ChatPromptTemplate
from langchain.agents.structured_output import ProviderStrategy

from pydantic import BaseModel, Field

load_dotenv()

llm = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

system_prompt: str = """
You are helpful assistant helping to the user queries with only simple line without chitchat or extra bulky messages. 
"""

# creating the class for structured output
class UserResponse(BaseModel):
    query: str = Field(description="User Query")
    response: str = Field(description="Response from the ai model")


# creating the agent
agent = create_agent(
    model=llm,
    system_prompt=system_prompt,
    response_format=ProviderStrategy(UserResponse),
)

messages = ChatPromptTemplate.from_messages(messages=[
    ("human", "{input}")
])

# creating the chain
output = messages | agent

# invoking the chain
response = output.invoke({"input": "Add two number 1 and 2"})

print(response)
