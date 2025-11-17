from langchain_google_genai import ChatGoogleGenerativeAI, GoogleGenerativeAI
from langchain.agents import create_agent
import os
from dotenv import load_dotenv
load_dotenv()


GEMINI_API_KEY = os.getenv("GEMINI_API_KEY")

# Initialize the Gemini 2.5 model
llm = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=GEMINI_API_KEY,
    temperature=0.7,
)

# Create an agent with the Gemini model
agent = create_agent(llm,
                     system_prompt="You're a helpful assistant that can answer questions about general knowledge.")

# Invoke the agent with a question
response = agent.invoke({
    "messages": [
        {"role": "user", "content": "Who won the FIFA World Cup in 2018?"}
    ]
})

print(response)
