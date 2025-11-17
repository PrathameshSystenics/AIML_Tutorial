from dotenv import load_dotenv
import os
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain.messages import HumanMessage, AIMessage, SystemMessage

load_dotenv()

# Create the messages
messages = [
    SystemMessage(content="You are a helpul assistant. Do not chitchat only direct message"),
]

model = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

while True:
    query = input("Enter :")
    messages.append(HumanMessage(content=query))
    
    # Invoke the model
    response = model.invoke(messages)
    print("Answer from AI =>", response.content)
    
    messages.append(AIMessage(content=response.content))