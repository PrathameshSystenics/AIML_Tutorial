from dotenv import load_dotenv
import os
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_core.prompts import ChatPromptTemplate
from langchain.messages import HumanMessage, AIMessage, SystemMessage

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

# Invoke the prompt
prompt = chat_prompt.invoke({"topic": "computers"})

response = model.invoke(prompt)
print("Answer from AI =>", response.content)
print(model.get_num_tokens_from_messages(prompt.to_messages()))