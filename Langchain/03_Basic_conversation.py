from dotenv import load_dotenv
import os
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain.messages import HumanMessage, AIMessage, SystemMessage

load_dotenv()

# Create the messages
messages = [
    SystemMessage(content="You are a math solver bot"),
    HumanMessage(content="What is 81 divided by 9?"),
]

model = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

response = model.invoke(messages)
print("Answer from AI =>", response.content)
