from dotenv import load_dotenv
import os
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_openai import AzureChatOpenAI
from langchain_core.messages import BaseMessage
from langchain.messages import HumanMessage, SystemMessage

load_dotenv()


messages: list[BaseMessage] = [
    SystemMessage(
        content="You are a helpful assistant that translates English to French."),
    HumanMessage(
        content="Translate the following English text to French: 'Hello, how are you?'")
]

# Creating the first Gemini Chat Model
chatmodel = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY"))

# Invoke the Gemini model
response = chatmodel.invoke(messages)
print("Gemini Model Response =>", response.content)

# Creating the second Azure OpenAI Chat Model
chatmodel = AzureChatOpenAI(
    api_key=os.getenv("AZURE_API_KEY"),
    azure_endpoint=os.getenv("AZURE_ENDPOINT"),
    deployment_name="gpt-4.1-mini",
    model_name="gpt-4.1-mini",
    api_version=os.getenv("AZURE_API_VERSION"),
)

# Invoke the Azure OpenAI model
azure_response = chatmodel.invoke(messages)
print("Azure OpenAI Model Response =>", azure_response.content)