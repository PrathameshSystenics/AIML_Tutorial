from dotenv import load_dotenv
import os
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser
from langchain_core.runnables import RunnableLambda

load_dotenv()

# Load the Model
model = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

prompt_template = ChatPromptTemplate.from_messages([
    ("system", "You are a funny assistant that tells jokes."),  
    ("human", "Tell me a joke about {topic}")
])

# creating the custom runnable to uppercase the output
uppercase_output = RunnableLambda(lambda x: x.upper())

chain = prompt_template | model | StrOutputParser() | uppercase_output
# Invoke the chain
response = chain.invoke({"topic": "computers"})
print("Final Answer from AI =>", response)