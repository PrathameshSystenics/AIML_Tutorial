from dotenv import load_dotenv
import os
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser

load_dotenv()

# Load the Model
model = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

chat_prompt = ChatPromptTemplate.from_messages([
    ("system", "You are a funny assistant that tells jokes."),
    ("human", "Tell me a joke about {topic}")
])

# chaining
chain = chat_prompt | model 

# converting to string directly using stroutputparser
formated_response= chain | StrOutputParser()

# Invoke the chain
response = chain.invoke({"topic": "computers"})
print("Answer from AI =>", response.content)
print("Formatted Answer from AI =>", formated_response.invoke({"topic": "computers"}))