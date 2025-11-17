import os
from dotenv import load_dotenv
from langchain_core.messages.ai import AIMessage
from langchain_google_genai import ChatGoogleGenerativeAI

load_dotenv()

# Basic Chat model
model = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

# invoke the chat
response: AIMessage = model.invoke("What is Your name")
print(response.content)
print(response)