import os
from dotenv import load_dotenv
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_core.prompts import ChatPromptTemplate
from langchain.messages import SystemMessage, HumanMessage, AIMessage
from langchain_core.output_parsers import StrOutputParser

load_dotenv()

llm = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY"),
)

messages = ChatPromptTemplate.from_messages([
    SystemMessage(
        "You are helpful assistant helps the user without extra chitchat")
])

while True:
    print("Enter User Query")
    query: str = input("Enter : ")
    messages.append(HumanMessage(query))

    chain = messages | llm | StrOutputParser()
    response: AIMessage = chain.invoke({"Query": query})

    print("AI : ",response)
    messages.append(AIMessage(response))
